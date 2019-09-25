// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Defines a system, feature, or manager to be registered with as a <see cref="IMixedRealityExtensionService"/> on startup.
    /// </summary>
    [Serializable]
    public struct MixedRealityServiceConfiguration : IMixedRealityServiceConfiguration
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="componentType">The concrete type for the system, feature or manager.</param>
        /// <param name="componentName">The simple, human readable name for the system, feature, or manager.</param>
        /// <param name="priority">The priority this system, feature, or manager will be initialized in.</param>
        /// <param name="runtimePlatform">The runtime platform(s) to run this system, feature, or manager on.</param>
        /// <param name="configurationProfile">The configuration profile for the service.</param>
        public MixedRealityServiceConfiguration(
            SystemType componentType,
            string componentName,
            uint priority,
            SupportedPlatforms runtimePlatform,
            IPlatformSupport[] customizedRuntimePlatform,
            BaseMixedRealityProfile configurationProfile)
        {
            this.componentType = componentType;
            this.componentName = componentName;
            this.priority = priority;
            this.runtimePlatform = runtimePlatform;
            this.customizedRuntimePlatform = customizedRuntimePlatform.Convert();
            this._customizedRuntimePlatform = customizedRuntimePlatform;
            this.configurationProfile = configurationProfile;
        }

        [SerializeField]
        [Implements(typeof(IMixedRealityExtensionService), TypeGrouping.ByNamespaceFlat)]
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
        private BaseMixedRealityProfile configurationProfile;

        /// <summary>
        /// The configuration profile for the service.
        /// </summary>
        public BaseMixedRealityProfile ConfigurationProfile => configurationProfile;
    }
}