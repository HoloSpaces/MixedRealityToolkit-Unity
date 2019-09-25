﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Attribute that defines the properties of a Mixed Reality Toolkit data provider.
    /// </summary>
    [AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
    public class MixedRealityDataProviderAttribute : MixedRealityExtensionServiceAttribute
    {
        /// <summary>
        /// The interface type of the IMixedRealityService for which the data provider is supported.
        /// </summary>
        public Type ServiceInterfaceType { get; }

        /// <inheritdoc/>
        public MixedRealityDataProviderAttribute(
            Type serviceInterfaceType,
            SupportedPlatforms runtimePlatforms,
            Type[] customizedRuntimePlatform,
            string name = "",
            string profilePath = "",
            string packageFolder = "MixedRealityToolkit",
            SupportedApplicationModes runtimeModes = (SupportedApplicationModes)(-1)
            ) : base(runtimePlatforms, customizedRuntimePlatform, name, profilePath, packageFolder, runtimeModes)
        {
            ServiceInterfaceType = serviceInterfaceType;
        }
    }
}
