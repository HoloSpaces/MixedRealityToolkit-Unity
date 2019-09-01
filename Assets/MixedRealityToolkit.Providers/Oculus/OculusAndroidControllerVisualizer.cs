// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

namespace Microsoft.MixedReality.Toolkit.Providers.OculusAndroid
{
    [RequireComponent(typeof(OVRControllerHelper))]
    public abstract class OculusAndroidControllerVisualizer : ControllerPoseSynchronizer, IMixedRealityControllerVisualizer
    {
        public GameObject GameObjectProxy => gameObject;

        public override IMixedRealityController Controller
        {
            get => base.Controller;
            set
            {
                base.Controller = value;
                OVRInput.Update();
            }
        }
    }
}
