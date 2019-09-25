﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;
#endif

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Attribute that defines the properties of a Mixed Reality Toolkit extension service.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MixedRealityExtensionServiceAttribute : Attribute
    {
        /// <summary>
        /// The friendly name for this service.
        /// </summary>
        public virtual string Name { get; }

        /// <summary>
        /// The runtime build target platform(s) to run this service.
        /// </summary>
        public virtual SupportedPlatforms RuntimePlatforms { get; }

        /// <summary>
        /// More specific target platform(s).
        /// </summary>
        public virtual Type[] CustomizedRuntimePlatforms { get; }

        /// <summary>
        /// The runtime platform(s) to run this service.
        /// </summary>
        public virtual SupportedApplicationModes RuntimeModes { get; }

        /// <summary>
        /// The file path to the default profile asset relative to the package folder.
        /// </summary>
        public virtual string DefaultProfilePath { get; }

        /// <summary>
        /// The package where the default profile asset resides.
        /// </summary>
        public virtual string PackageFolder { get; }

        /// <summary>
        /// The default profile.
        /// </summary>
        public virtual BaseMixedRealityProfile DefaultProfile
        {
            get
            {
#if UNITY_EDITOR
                MixedRealityToolkitModuleType moduleType = MixedRealityToolkitFiles.GetModuleFromPackageFolder(PackageFolder);

                if (moduleType != MixedRealityToolkitModuleType.None)
                {
                    string folder = MixedRealityToolkitFiles.MapModulePath(moduleType);
                    if (!string.IsNullOrWhiteSpace(folder))
                    {
                        return AssetDatabase.LoadAssetAtPath<BaseMixedRealityProfile>(System.IO.Path.Combine(folder, DefaultProfilePath));
                    }
                }
                else
                {
                    string folder;
                    if (EditorProjectUtilities.FindRelativeDirectory(PackageFolder, out folder))
                    {
                        return AssetDatabase.LoadAssetAtPath<BaseMixedRealityProfile>(System.IO.Path.Combine(folder, DefaultProfilePath));
                    }
                }

                // If we get here, there was an issue finding the profile.
                Debug.LogError("Unable to find or load the profile.");
#endif  
                return null;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="runtimePlatforms">The platforms on which the extension service is supported.</param>
        /// <param name="defaultProfilePath">The relative path to the default profile asset.</param>
        /// <param name="packageFolder">The package folder to which the path is relative.</param>
        /// <param name="runtimeModes">The runtime modes which the extension service is supported and can run. Default is support on all runtime modes</param>
        public MixedRealityExtensionServiceAttribute(
            SupportedPlatforms runtimePlatforms,
            Type[] customizedRuntimePlatforms = null,
            string name = "",
            string defaultProfilePath = "",
            string packageFolder = "MixedRealityToolkit",
            SupportedApplicationModes runtimeModes = (SupportedApplicationModes)(-1))
        {
            Name = name;
            RuntimePlatforms = runtimePlatforms;
            CustomizedRuntimePlatforms = customizedRuntimePlatforms;
            DefaultProfilePath = defaultProfilePath;
            PackageFolder = packageFolder;
            RuntimeModes = runtimeModes;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Convenience function for retrieving the attribute given a certain class type.
        /// </summary>
        /// <remarks>
        /// This function is only available in a UnityEditor context.
        /// </remarks>
        public static MixedRealityExtensionServiceAttribute Find(Type type)
        {
            return type.GetCustomAttributes(typeof(MixedRealityExtensionServiceAttribute), true).FirstOrDefault() as MixedRealityExtensionServiceAttribute;
        }
#endif // UNITY_EDITOR
    }
}
