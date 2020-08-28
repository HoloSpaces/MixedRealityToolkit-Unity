// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;

namespace HoloSpaces.MixedReality.Input
{
    /// <summary>
    /// Responsible for synchronizing the user's current input with Oculus Quest controller models.
    /// </summary>
    /// <seealso cref="OculusGoControllerVisualizer"/>
    public class OculusGoControllerVisualizer : OculusAndroidControllerVisualizer
    {
        public override IMixedRealityController Controller
        {
            get => base.Controller;
            set
            {
                base.Controller = value;
#if !UNITY_EDITOR && UNITY_ANDROID
                GetComponent<OVRControllerHelper>().m_controller = (value.ControllerHandedness & Microsoft.MixedReality.Toolkit.Utilities.Handedness.Left) != 0 ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch;
#endif
            }
        }
    }
}