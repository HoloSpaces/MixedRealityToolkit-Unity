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
    /// Manages Open VR Devices using unity's input system.
    /// </summary>
    [MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        SupportedPlatforms.Android)]
    public class OculusAndroidDeviceManager : UnityJoystickManager
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

            Handedness controllingHand;

            if (joystickName.Contains("Left"))
            {
                controllingHand = Handedness.Left;
            }
            else if (joystickName.Contains("Right"))
            {
                controllingHand = Handedness.Right;
            }
            else
            {
                controllingHand = Handedness.None;
            }

            var currentControllerType = GetCurrentControllerType(joystickName);
            Type controllerType;

            switch (currentControllerType)
            {
                case SupportedControllerType.GenericAndroid:
                    controllerType = typeof(GenericOculusAndroidController);
                    break;
                case SupportedControllerType.OculusGoRemote:
                    controllerType = typeof(OculusGoRemoteController);
                    break;
                default:
                    return null;
            }

            var pointers = RequestPointers(currentControllerType, controllingHand);
            var inputSource = CoreServices.InputSystem?.RequestNewGenericInputSource($"{currentControllerType} Controller {controllingHand}", pointers);
            var detectedController = Activator.CreateInstance(controllerType, TrackingState.NotTracked, controllingHand, inputSource, null) as GenericJoystickController;

            if (detectedController == null)
            {
                Debug.LogError($"Failed to create {controllerType.Name} controller");
                return null;
            }

            if (!detectedController.SetupConfiguration(controllerType))
            {
                // Controller failed to be setup correctly.
                Debug.LogError($"Failed to Setup {controllerType.Name} controller");
                // Return null so we don't raise the source detected.
                return null;
            }

            for (int i = 0; i < detectedController.InputSource?.Pointers?.Length; i++)
            {
                detectedController.InputSource.Pointers[i].Controller = detectedController;
            }

            ActiveControllers.Add(joystickName, detectedController);
            return detectedController;
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
