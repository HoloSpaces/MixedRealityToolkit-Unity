﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialAwareness
{
    [Serializable]
    public struct MixedRealitySpatialObserverConfiguration : IMixedRealityServiceConfiguration
    {
        [SerializeField]
        [Implements(typeof(IMixedRealitySpatialAwarenessObserver), TypeGrouping.ByNamespaceFlat)]
        private SystemType componentType;

        /// <inheritdoc />
        public SystemType ComponentType => componentType;

        [SerializeField]
        private string componentName;

        /// <inheritdoc />
        public string ComponentName => componentName;

        [SerializeField]
        private uint priority;

        /// <inheritdoc />
        public uint Priority => priority;

        [SerializeField]
        [EnumFlags]
        private SupportedPlatforms runtimePlatform;

        /// <inheritdoc />
        public SupportedPlatforms RuntimePlatform => runtimePlatform;

        [SerializeField]
        [Implements(typeof(IPlatformSupport), TypeGrouping.ByNamespaceFlat)]
        private SystemType[] customizedRuntimePlatform;

        /// <inheritdoc />
        private IPlatformSupport[] _customizedRuntimePlatform;

        /// <inheritdoc />
        public IPlatformSupport[] CustomizedRuntimePlatform
        {
            get
            {
                if (_customizedRuntimePlatform == null)
                {
                    _customizedRuntimePlatform = customizedRuntimePlatform.Convert();
                }

                return _customizedRuntimePlatform;
            }
        }

        [SerializeField]
        private BaseSpatialAwarenessObserverProfile observerProfile;

        /// <summary>
        /// 
        /// </summary>
        public BaseSpatialAwarenessObserverProfile ObserverProfile => observerProfile;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="componentType">The <see cref="Microsoft.MixedReality.Toolkit.Utilities.SystemType"/> of the observer.</param>
        /// <param name="componentName">The friendly name of the observer.</param>
        /// <param name="priority">The load priority of the observer.</param>
        /// <param name="runtimePlatform">The runtime platform(s) supported by the observer.</param>
        /// <param name="configurationProfile">The configuration profile for the observer.</param>
        public MixedRealitySpatialObserverConfiguration(
            SystemType componentType,
            string componentName,
            uint priority,
            SupportedPlatforms runtimePlatform,
            IPlatformSupport[] customizedRuntimePlatform,
            BaseSpatialAwarenessObserverProfile configurationProfile)
        {
            this.componentType = componentType;
            this.componentName = componentName;
            this.priority = priority;
            this.runtimePlatform = runtimePlatform;
            this.customizedRuntimePlatform = customizedRuntimePlatform.Convert();
            this._customizedRuntimePlatform = customizedRuntimePlatform;
            this.observerProfile = configurationProfile;
        }
    }
}
