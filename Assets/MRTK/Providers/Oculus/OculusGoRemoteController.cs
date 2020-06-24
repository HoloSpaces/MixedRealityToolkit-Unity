// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UInput = UnityEngine.Input;
using UnityEngine.EventSystems;

namespace HoloSpaces.MixedReality.Input
{
    [MixedRealityController(
        SupportedControllerType.OculusGoRemote,
        new[] { Handedness.Both },
        "StandardAssets/Textures/OculusGoRemoteController")]
    public class OculusGoRemoteController : GenericOculusAndroidController
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="trackingState"></param>
        /// <param name="controllerHandedness"></param>
        /// <param name="inputSource"></param>
        /// <param name="interactions"></param>
        public OculusGoRemoteController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
                : base(trackingState, controllerHandedness, inputSource, interactions)
        {
        }

        private bool isTouchPadPressed;
        private bool isTeleportEnabled;

        private IMixedRealityTeleportPointer teleportPointer;
        private IMixedRealityTeleportPointer TeleportPointer => teleportPointer != null ? teleportPointer : teleportPointer = PointerUtils.GetPointer<IMixedRealityTeleportPointer>(Handedness.Any);

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultInteractions => new[]
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, new MixedRealityInputAction(4, "Pointer Pose", AxisType.SixDof)),
            new MixedRealityInteractionMapping(1, "Trigger", AxisType.SingleAxis, DeviceInputType.TriggerPress, new MixedRealityInputAction(1, "Select", AxisType.Digital), axisCodeX: ControllerMappingLibrary.AXIS_3),
            new MixedRealityInteractionMapping(2, "Back", AxisType.Digital, DeviceInputType.ButtonPress, new MixedRealityInputAction(2, "Menu", AxisType.Digital), KeyCode.Joystick1Button1),
            new MixedRealityInteractionMapping(3, "PrimaryTouchpad Touch", AxisType.Digital, DeviceInputType.TouchpadTouch, KeyCode.JoystickButton17),
            new MixedRealityInteractionMapping(4, "PrimaryTouchpad Click", AxisType.Digital, DeviceInputType.TouchpadPress, KeyCode.JoystickButton9),
            new MixedRealityInteractionMapping(5, "PrimaryTouchpad Axis", AxisType.DualAxis, DeviceInputType.DirectionalPad, axisCodeX: ControllerMappingLibrary.AXIS_4, axisCodeY: ControllerMappingLibrary.AXIS_5, invertYAxis: true)
        };

        private readonly MixedRealityInputAction teleportInputAction = new MixedRealityInputAction(5, "Teleport Direction", AxisType.DualAxis);

        /// <inheritdoc />
        public override void SetupDefaultInteractions()
        {
            AssignControllerMappings(DefaultInteractions);
        }

        protected override void UpdateButtonData(MixedRealityInteractionMapping interactionMapping)
        {
            base.UpdateButtonData(interactionMapping);
            if (interactionMapping.InputType == DeviceInputType.TouchpadPress)
            {
                isTouchPadPressed = interactionMapping.BoolData;
                if (isTouchPadPressed)
                {
                    isTeleportEnabled = true;
                }
            }
        }

        /// <summary>
        /// This is copy of the underlying implementation with a specific adjustment for Oculus Go
        /// </remarks>
        protected override void UpdateSingleAxisData(MixedRealityInteractionMapping interactionMapping)
        {
            using (UpdateSingleAxisDataPerfMarker.Auto())
            {
                Debug.Assert(interactionMapping.AxisType == AxisType.SingleAxis);

                var singleAxisValue = UInput.GetAxisRaw(interactionMapping.AxisCodeX);

                if (interactionMapping.InputType == DeviceInputType.TriggerPress)
                {
                    interactionMapping.BoolData = Mathf.Abs(singleAxisValue).Equals(1);

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        // rwr TODO: this only occurs on the selected InputField of the Keyboard
                        // the selectedObject gets deselected and is recognized as a button up by the UnityInput System - gets set to 1 on true release on joystick
                        EventSystem currentEventSystem = EventSystem.current;
                        if (currentEventSystem.currentSelectedGameObject != null) currentEventSystem.SetSelectedGameObject(null);

                        // Raise input system event if it's enabled
                        if (interactionMapping.BoolData)
                        {
                            CoreServices.InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                        }
                        else
                        {
                            CoreServices.InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                        }
                    }
                }
                else
                {
                    // Update the interaction data source
                    interactionMapping.FloatData = singleAxisValue;

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        // Raise input system event if it's enabled
                        CoreServices.InputSystem?.RaiseFloatInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, interactionMapping.FloatData);
                    }
                }
            }
        }

        protected override void UpdateDualAxisData(MixedRealityInteractionMapping interactionMapping)
        {
            if (interactionMapping.InputType != DeviceInputType.DirectionalPad)
            {
                base.UpdateDualAxisData(interactionMapping);
                return;
            }

            if (isTouchPadPressed)
            {
                Debug.Assert(interactionMapping.AxisType == AxisType.DualAxis);

                Vector2 dualAxisPosition;

                dualAxisPosition.x = UInput.GetAxisRaw(interactionMapping.AxisCodeX);
                dualAxisPosition.y = UInput.GetAxisRaw(interactionMapping.AxisCodeY);

                if (dualAxisPosition.sqrMagnitude < Mathf.Pow(5f, 2f))
                {
                    dualAxisPosition.Normalize();
                }

                // Update the interaction data source
                interactionMapping.Vector2Data = dualAxisPosition;

                CoreServices.InputSystem?.RaisePositionInputChanged(InputSource, ControllerHandedness, teleportInputAction, interactionMapping.Vector2Data);
            }
            else if (isTeleportEnabled)
            {
                isTeleportEnabled = false;
                interactionMapping.Vector2Data = Vector2.zero;

                CoreServices.InputSystem?.RaisePositionInputChanged(InputSource, ControllerHandedness, teleportInputAction, interactionMapping.Vector2Data);
            }
            else
            {
                base.UpdateDualAxisData(interactionMapping);
            }
        }
    }
}
