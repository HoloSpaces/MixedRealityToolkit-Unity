﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Input.UnityInput;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Providers.OculusAndroid
{
    /// <summary>
    /// Manages Oculus Android native devices.
    /// </summary>
    [MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        SupportedPlatforms.Android,
        "Oculus Device Manager")]
    public class OculusAndroidDeviceManager : UnityJoystickManager, IMixedRealityCapabilityCheck
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public OculusAndroidDeviceManager(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealityInputSystem inputSystem,
            string name,
            uint priority,
            BaseMixedRealityProfile profile) : base(registrar, inputSystem, name, priority, profile) { }

#region IMixedRealityCapabilityCheck Implementation

        public bool CheckCapability(MixedRealityCapability capability) => capability == MixedRealityCapability.MotionController;
        
#endregion IMixedRealityCapabilityCheck Implementation

        #region Controller Utilities

        /// <inheritdoc />
        protected override GenericJoystickController GetOrAddController(string joystickName)
        {
            Debug.Log($"Found joystick:{joystickName}");

            // If a device is already registered with the ID provided, just return it.
            if (ActiveControllers.ContainsKey(joystickName))
            {
                var controller = ActiveControllers[joystickName];
                Debug.Assert(controller != null);
                return controller;
            }

            GenericJoystickController detectedController = CreateDetectedController(joystickName);

            for (int i = 0; i < detectedController.InputSource?.Pointers?.Length; i++)
            {
                detectedController.InputSource.Pointers[i].Controller = detectedController;
            }

            ActiveControllers.Add(joystickName, detectedController);
            return detectedController;
        }

        private GenericJoystickController CreateDetectedController(string joystickName)
        {
            Handedness controllingHand = GetHandedness(joystickName);
            SupportedControllerType currentControllerType = GetCurrentControllerType(joystickName);
            Type controllerType = GetControllerType(currentControllerType);

            IMixedRealityPointer[] pointers = RequestPointers(currentControllerType, controllingHand);
            IMixedRealityInputSource inputSource = CoreServices.InputSystem?.RequestNewGenericInputSource($"{currentControllerType} Controller {controllingHand}", pointers);
            GenericJoystickController detectedController = Activator.CreateInstance(controllerType, TrackingState.NotTracked, controllingHand, inputSource, null) as GenericJoystickController;

            if (detectedController == null)
            {
                Debug.LogError($"Failed to create {controllerType.Name} controller");
                return null;
            }

            if (!detectedController.SetupConfiguration(controllerType))
            {
                Debug.LogError($"Failed to Setup {controllerType.Name} controller");
                return null;
            }

            return detectedController;
        }

        private Handedness GetHandedness(string deviceName) => deviceName.Contains("Left") ? Handedness.Left : deviceName.Contains("Right") ? Handedness.Right : Handedness.None;

        private Type GetControllerType(SupportedControllerType supportedControllerType)
        {
            switch (supportedControllerType)
            {
                case SupportedControllerType.GenericAndroid:
                    return typeof(GenericOculusAndroidController);
                case SupportedControllerType.OculusGoRemote:
                    return typeof(OculusGoRemoteController);
                default:
                    return null;
            }
        }

        /// <inheritdoc />
        protected override SupportedControllerType GetCurrentControllerType(string joystickName)
        {
            if (joystickName.Contains("Oculus Tracked Remote"))
            {
                return SupportedControllerType.OculusGoRemote;
            }

            Debug.Log($"{joystickName} does not have a defined controller type, falling back to generic controller type");

            return SupportedControllerType.GenericAndroid;
        }

        #endregion Controller Utilities
    }
}
