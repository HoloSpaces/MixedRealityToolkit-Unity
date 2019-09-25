﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Defines a system, feature, or manager to be registered with as a <see cref="IMixedRealityExtensionService"/> on startup.
    /// </summary>
    public interface IMixedRealityServiceConfiguration
    {
        /// <summary>
        /// The concrete type for the system, feature or manager.
        /// </summary>
        SystemType ComponentType { get; }

        /// <summary>
        /// The simple, human readable name for the system, feature, or manager.
        /// </summary>
        string ComponentName { get; }

        /// <summary>
        /// The priority this system, feature, or manager will be initialized in.
        /// </summary>
        uint Priority { get; }

        /// <summary>
        /// The runtime build target platform(s) to run this service.
        /// </summary>
        SupportedPlatforms RuntimePlatform { get; }

        /// <summary>
        /// The runtime environment modes (i.e editor) to run this service. None is not a valid value
        /// </summary>
        SupportedApplicationModes RuntimeModes { get; }
    }
}