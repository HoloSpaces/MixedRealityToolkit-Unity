// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.XR;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input.UnityInput;
using UnityEngine;

namespace HoloSpaces.MixedReality.Input
{
    [MixedRealityController(
        SupportedControllerType.GenericAndroid,
        new[] { Handedness.Left, Handedness.Right },
        flags: MixedRealityControllerConfigurationFlags.UseCustomInteractionMappings)]
    public class GenericOculusAndroidController : GenericJoystickController
    {
        public GenericOculusAndroidController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions)
        {
            nodeType = controllerHandedness == Handedness.Left ? XRNode.LeftHand : XRNode.RightHand;
        }

        protected readonly XRNode nodeType;
        protected TrackingState lastTrackingState = TrackingState.NotTracked;

        /// <summary>
        /// The current source state reading for this OpenVR Controller.
        /// </summary>
        public XRNodeState LastXrNodeStateReading { get; protected set; }

        /// <summary>
        /// Tracking states returned from the InputTracking state tracking manager
        /// </summary>
        private readonly List<XRNodeState> nodeStates = new List<XRNodeState>();

        /// <inheritdoc />
        public override void UpdateController()
        {
            if (!Enabled) { return; }

            InputTracking.GetNodeStates(nodeStates);

            for (int i = 0; i < nodeStates.Count; i++)
            {
                if (nodeStates[i].nodeType == nodeType)
                {
                    var xrNodeState = nodeStates[i];
                    UpdateControllerData(xrNodeState);
                    LastXrNodeStateReading = xrNodeState;
                    break;
                }
            }

            base.UpdateController();
        }

        /// <summary>
        /// Update the "Controller" input from the device
        /// </summary>
        /// <param name="state"></param>
        protected virtual void UpdateControllerData(XRNodeState state)
        {
            lastTrackingState = TrackingState;

            LastControllerPose = CurrentControllerPose;

            switch (nodeType)
            {
                case XRNode.LeftHand:
                case XRNode.RightHand:
                    // The source is either a hand or a controller that supports pointing.
                    // We can now check for position and rotation.
                    IsPositionAvailable = state.TryGetPosition(out CurrentControllerPosition);
                    IsPositionApproximate = false;

                    Quaternion rotation;
                    IsRotationAvailable = state.TryGetRotation(out rotation);

                    // Devices are considered tracked if we receive position OR rotation data from the sensors.
                    TrackingState = (IsPositionAvailable || IsRotationAvailable) ? TrackingState.Tracked : TrackingState.NotTracked;

                    if (IsPositionAvailable)
                    {
                        CurrentControllerPosition = MixedRealityPlayspace.TransformPoint(CurrentControllerPosition);
                        CurrentControllerPose.Position = CurrentControllerPosition;
                    }

                    if (IsRotationAvailable)
                    {
                        CurrentControllerRotation = MixedRealityPlayspace.Rotation * rotation;
                        CurrentControllerPose.Rotation = CurrentControllerRotation;
                    }

                    if (TrackingState == TrackingState.Tracked && LastControllerPose != CurrentControllerPose)
                    {
                        if (IsPositionAvailable && IsRotationAvailable)
                        {
                            CoreServices.InputSystem?.RaiseSourcePoseChanged(InputSource, this, CurrentControllerPose);
                        }
                        else if (IsPositionAvailable && !IsRotationAvailable)
                        {
                            CoreServices.InputSystem?.RaiseSourcePositionChanged(InputSource, this, CurrentControllerPosition);
                        }
                        else if (!IsPositionAvailable && IsRotationAvailable)
                        {
                            CoreServices.InputSystem?.RaiseSourceRotationChanged(InputSource, this, CurrentControllerRotation);
                        }
                    }

                    break;
                default:
                    // The input source does not support tracking.
                    TrackingState = TrackingState.NotApplicable;
                    break;
            }
        }
    }
}