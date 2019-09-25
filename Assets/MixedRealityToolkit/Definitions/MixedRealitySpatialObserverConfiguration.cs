﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialAwareness
{
    [Serializable]
    public class MixedRealitySpatialObserverConfiguration : IMixedRealityServiceConfiguration
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
        private SupportedPlatforms runtimePlatform = (SupportedPlatforms)(-1);

        /// <inheritdoc />
        public SupportedPlatforms RuntimePlatform => runtimePlatform;

        [EnumFlags]
        [SerializeField]
        private SupportedApplicationModes runtimeModes = (SupportedApplicationModes)(-1);

        /// <inheritdoc />
        public SupportedApplicationModes RuntimeModes
        {
            get
            {
                // Flag cannot be none
                if (runtimeModes == (SupportedApplicationModes)0)
                {
                    runtimeModes = (SupportedApplicationModes)(-1);
                }

                return runtimeModes;
            }
        }

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
        /// The Observer profile containing the revelant 
        /// </summary>
        public BaseSpatialAwarenessObserverProfile ObserverProfile => observerProfile;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="componentType">The <see cref="Microsoft.MixedReality.Toolkit.Utilities.SystemType"/> of the observer.</param>
        /// <param name="componentName">The friendly name of the observer.</param>
        /// <param name="priority">The load priority of the observer.</param>
        /// <param name="runtimePlatform">The runtime build target platform(s) supported by the observer.</param>
        /// <param name="applicationModes">The runtime environment mode(s) supported by the observer</param>
        /// <param name="configurationProfile">The configuration profile for the observer.</param>
        public MixedRealitySpatialObserverConfiguration(
            SystemType componentType,
            string componentName,
            uint priority,
            SupportedPlatforms runtimePlatform,
            IPlatformSupport[] customizedRuntimePlatform,
            SupportedApplicationModes applicationModes,
            BaseSpatialAwarenessObserverProfile configurationProfile)
        {
            this.componentType = componentType;
            this.componentName = componentName;
            this.priority = priority;
            this.runtimePlatform = runtimePlatform;
            this.runtimeModes = applicationModes;
            this.customizedRuntimePlatform = customizedRuntimePlatform.Convert();
            this._customizedRuntimePlatform = customizedRuntimePlatform;
            this.observerProfile = configurationProfile;
        }
    }
}
