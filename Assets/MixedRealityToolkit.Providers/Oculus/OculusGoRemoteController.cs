// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

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

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultInteractions => new[]
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, new MixedRealityInputAction(4, "Pointer Pose", AxisType.SixDof)),
            new MixedRealityInteractionMapping(1, "Trigger", AxisType.Digital, DeviceInputType.ButtonPress, new MixedRealityInputAction(1, "Select", AxisType.Digital), KeyCode.JoystickButton14),
            new MixedRealityInteractionMapping(2, "Back", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton6),
            new MixedRealityInteractionMapping(3, "PrimaryTouchpad Touch", AxisType.Digital, DeviceInputType.TouchpadTouch, KeyCode.JoystickButton16),
            new MixedRealityInteractionMapping(4, "PrimaryTouchpad Click", AxisType.Digital, DeviceInputType.TouchpadPress, KeyCode.JoystickButton8),
            new MixedRealityInteractionMapping(5, "PrimaryTouchpad Axis", AxisType.DualAxis, DeviceInputType.DirectionalPad, ControllerMappingLibrary.AXIS_4, ControllerMappingLibrary.AXIS_5)
        };

        /// <inheritdoc />
        public override void SetupDefaultInteractions(Handedness controllerHandedness)
        {
            AssignControllerMappings(DefaultInteractions);
        }
    }
}
