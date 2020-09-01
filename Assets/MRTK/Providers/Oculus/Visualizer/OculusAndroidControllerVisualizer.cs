// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

namespace HoloSpaces.MixedReality.Input
{
#if !UNITY_EDITOR && UNITY_ANDROID
    [RequireComponent(typeof(OVRControllerHelper))]
#endif
    public abstract class OculusAndroidControllerVisualizer : ControllerPoseSynchronizer, IMixedRealityControllerVisualizer
    {
        public GameObject GameObjectProxy => gameObject;

        public override IMixedRealityController Controller
        {
            get => base.Controller;
            set
            {
                base.Controller = value;
#if UNITY_ANDROID
                OVRInput.Update();
#endif
            }
        }
    }
}
