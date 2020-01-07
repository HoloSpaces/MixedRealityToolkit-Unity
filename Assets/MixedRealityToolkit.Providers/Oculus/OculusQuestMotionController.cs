// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.XR;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

namespace HoloSpaces.MixedReality.Input
{
    [MixedRealityController(
        SupportedControllerType.OculusQuestRemote,
        new[] { Handedness.Left, Handedness.Right },
        "StandardAssets/Textures/OculusControllersTouch")]
    public class OculusQuestMotionController : GenericOculusAndroidController
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="trackingState"></param>
        /// <param name="controllerHandedness"></param>
        /// <param name="inputSource"></param>
        /// <param name="interactions"></param>
        public OculusQuestMotionController(TrackingState trackingState, Handedness controllerHandedness,
            IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions)
        {
        }

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultLeftHandedInteractions => new[]
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, new MixedRealityInputAction(4, "Pointer Pose", AxisType.SixDof)),
            new MixedRealityInteractionMapping(1, "Spatial Grip", AxisType.SixDof, DeviceInputType.SpatialGrip, new MixedRealityInputAction(3, "Grip Pose", AxisType.SixDof)),
            new MixedRealityInteractionMapping(2, "Axis1D.PrimaryIndexTrigger", AxisType.SingleAxis, DeviceInputType.Trigger, new MixedRealityInputAction(6, "Trigger", AxisType.SingleAxis), axisCodeX: ControllerMappingLibrary.AXIS_9),
            new MixedRealityInteractionMapping(3, "Axis1D.PrimaryIndexTrigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch, KeyCode.JoystickButton14),
            new MixedRealityInteractionMapping(4, "Axis1D.PrimaryIndexTrigger Near Touch", AxisType.SingleAxis, DeviceInputType.TriggerNearTouch, ControllerMappingLibrary.AXIS_13),
            new MixedRealityInteractionMapping(5, "Axis1D.PrimaryIndexTrigger Press", AxisType.Digital, DeviceInputType.TriggerPress, new MixedRealityInputAction(1, "Select", AxisType.Digital), KeyCode.None, axisCodeX: ControllerMappingLibrary.AXIS_9),
            new MixedRealityInteractionMapping(6, "Axis1D.PrimaryHandTrigger Press", AxisType.SingleAxis, DeviceInputType.TriggerPress, new MixedRealityInputAction(7, "Grip Press", AxisType.SingleAxis), axisCodeX: ControllerMappingLibrary.AXIS_11),
            new MixedRealityInteractionMapping(7, "Axis2D.PrimaryThumbstick", AxisType.DualAxis, DeviceInputType.ThumbStick, new MixedRealityInputAction(5, "Teleport Direction", AxisType.DualAxis), axisCodeX: ControllerMappingLibrary.AXIS_1, axisCodeY: ControllerMappingLibrary.AXIS_2, invertYAxis: true),
            new MixedRealityInteractionMapping(8, "Button.PrimaryThumbstick Touch", AxisType.Digital, DeviceInputType.ThumbStickTouch, KeyCode.JoystickButton16),
            new MixedRealityInteractionMapping(9, "Button.PrimaryThumbstick Near Touch", AxisType.Digital, DeviceInputType.ThumbNearTouch, ControllerMappingLibrary.AXIS_15),
            new MixedRealityInteractionMapping(10, "Button.PrimaryThumbstick Press", AxisType.Digital, DeviceInputType.ThumbStickPress, KeyCode.JoystickButton8),
            new MixedRealityInteractionMapping(11, "Button.Three Press", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton2),
            new MixedRealityInteractionMapping(12, "Button.Four Press", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton3),
            new MixedRealityInteractionMapping(13, "Button.Start Press", AxisType.Digital, DeviceInputType.ButtonPress, new MixedRealityInputAction(2, "Menu", AxisType.Digital), KeyCode.JoystickButton6),
            new MixedRealityInteractionMapping(14, "Button.Three Touch", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton12),
            new MixedRealityInteractionMapping(15, "Button.Four Touch", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton13)
        };

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultRightHandedInteractions => new[]
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, new MixedRealityInputAction(4, "Pointer Pose", AxisType.SixDof)),
            new MixedRealityInteractionMapping(1, "Spatial Grip", AxisType.SixDof, DeviceInputType.SpatialGrip, new MixedRealityInputAction(3, "Grip Pose", AxisType.SixDof)),
            new MixedRealityInteractionMapping(2, "Axis1D.SecondaryIndexTrigger", AxisType.SingleAxis, DeviceInputType.Trigger, new MixedRealityInputAction(6, "Trigger", AxisType.SingleAxis), axisCodeX:  ControllerMappingLibrary.AXIS_10),
            new MixedRealityInteractionMapping(3, "Axis1D.SecondaryIndexTrigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch, KeyCode.JoystickButton15),
            new MixedRealityInteractionMapping(4, "Axis1D.SecondaryIndexTrigger Near Touch", AxisType.SingleAxis, DeviceInputType.TriggerNearTouch, ControllerMappingLibrary.AXIS_14),
            new MixedRealityInteractionMapping(5, "Axis1D.SecondaryIndexTrigger Press", AxisType.Digital, DeviceInputType.TriggerPress, new MixedRealityInputAction(1, "Select", AxisType.Digital), KeyCode.None, axisCodeX: ControllerMappingLibrary.AXIS_10),
            new MixedRealityInteractionMapping(6, "Axis1D.SecondaryHandTrigger Press", AxisType.SingleAxis, DeviceInputType.TriggerPress, new MixedRealityInputAction(7, "Grip Press", AxisType.SingleAxis), axisCodeX: ControllerMappingLibrary.AXIS_12),
            new MixedRealityInteractionMapping(7, "Axis2D.SecondaryThumbstick", AxisType.DualAxis, DeviceInputType.ThumbStick, new MixedRealityInputAction(5, "Teleport Direction", AxisType.DualAxis), axisCodeX: ControllerMappingLibrary.AXIS_4, axisCodeY: ControllerMappingLibrary.AXIS_5, invertYAxis: true),
            new MixedRealityInteractionMapping(8, "Button.SecondaryThumbstick Touch", AxisType.Digital, DeviceInputType.ThumbStickTouch, KeyCode.JoystickButton17),
            new MixedRealityInteractionMapping(9, "Button.SecondaryThumbstick Near Touch", AxisType.Digital, DeviceInputType.ThumbNearTouch, ControllerMappingLibrary.AXIS_16),
            new MixedRealityInteractionMapping(10, "Button.SecondaryThumbstick Press", AxisType.Digital, DeviceInputType.ThumbStickPress, KeyCode.JoystickButton9),
            new MixedRealityInteractionMapping(11, "Button.One Press", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton0),
            new MixedRealityInteractionMapping(12, "Button.Two Press", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton1),
            new MixedRealityInteractionMapping(13, "Button.One Touch", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton10),
            new MixedRealityInteractionMapping(14, "Button.Two Touch", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton11)
        };

        /// <inheritdoc />
        public override void SetupDefaultInteractions(Handedness controllerHandedness) => AssignControllerMappings(controllerHandedness == Handedness.Left ? DefaultLeftHandedInteractions : DefaultRightHandedInteractions);
    }
}