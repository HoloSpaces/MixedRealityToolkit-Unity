// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UInput = UnityEngine.Input;

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
        private bool isTriggerPressed;
        private bool isTeleportEnabled;
        private bool isGrabbingExpected;

        private IMixedRealityTeleportPointer teleportPointer;
        private IMixedRealityTeleportPointer TeleportPointer => teleportPointer != null ? teleportPointer : teleportPointer = PointerUtils.GetPointer<IMixedRealityTeleportPointer>(Handedness.Any);

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultInteractions => new[]
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, new MixedRealityInputAction(4, "Pointer Pose", AxisType.SixDof)),
            new MixedRealityInteractionMapping(1, "Trigger", AxisType.SingleAxis, DeviceInputType.TriggerPress, new MixedRealityInputAction(1, "Select", AxisType.Digital), axisCodeX: ControllerMappingLibrary.AXIS_3),
            new MixedRealityInteractionMapping(2, "Back", AxisType.Digital, DeviceInputType.ButtonPress, new MixedRealityInputAction(2, "Menu", AxisType.Digital), KeyCode.JoystickButton7),
            new MixedRealityInteractionMapping(3, "PrimaryTouchpad Touch", AxisType.Digital, DeviceInputType.TouchpadTouch, KeyCode.JoystickButton17),
            new MixedRealityInteractionMapping(4, "PrimaryTouchpad Click", AxisType.Digital, DeviceInputType.TouchpadPress, KeyCode.JoystickButton9),
            new MixedRealityInteractionMapping(5, "PrimaryTouchpad Axis", AxisType.DualAxis, DeviceInputType.DirectionalPad, axisCodeX: ControllerMappingLibrary.AXIS_4, axisCodeY: ControllerMappingLibrary.AXIS_5, invertYAxis: true)
        };

        private readonly MixedRealityInputAction teleportInputAction = new MixedRealityInputAction(5, "Teleport Direction", AxisType.DualAxis);
        private readonly MixedRealityInputAction poseTransformationInputAction = new MixedRealityInputAction(3, "Grip Pose", AxisType.SixDof);

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

        protected override void UpdateSingleAxisData(MixedRealityInteractionMapping interactionMapping)
        {
            base.UpdateSingleAxisData(interactionMapping);
            if (interactionMapping.InputType == DeviceInputType.TriggerPress)
            {
                isTriggerPressed = interactionMapping.BoolData;
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
                if (isTriggerPressed && !isTeleportEnabled)
                {
                    interactionMapping = new MixedRealityInteractionMapping(
                        interactionMapping.Id,
                        interactionMapping.Description,
                        interactionMapping.AxisType,
                        interactionMapping.InputType,
                        poseTransformationInputAction,
                        keyCode: interactionMapping.KeyCode,
                        axisCodeX: interactionMapping.AxisCodeX,
                        axisCodeY: interactionMapping.AxisCodeY,
                        invertXAxis: interactionMapping.InvertXAxis,
                        invertYAxis: interactionMapping.InvertYAxis
                    );
                }

                base.UpdateDualAxisData(interactionMapping);
            }
        }
    }
}
