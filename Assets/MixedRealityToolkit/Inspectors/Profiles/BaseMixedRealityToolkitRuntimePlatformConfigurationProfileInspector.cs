using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BaseMixedRealityToolkitRuntimePlatformConfigurationProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
{
    private static readonly GUIContent MinusButtonContent = new GUIContent("<-", "Revert to regular platforms");

    protected static string[] runtimePlatformNames;
    protected static Type[] runtimePlatformTypes;
    protected static List<int> runtimePlatformMasks = new List<int>();

    protected override bool IsProfileInActiveInstance()
    {
        return MixedRealityToolkit.IsInitialized &&
               MixedRealityToolkit.Instance.HasActiveProfile;
    }

    protected static void DefaultRuntimeSettings(SerializedProperty dataProviderConfiguration)
    {
        var runtimePlatform = dataProviderConfiguration.FindPropertyRelative("runtimePlatform");
        runtimePlatform.intValue = 1 << (int)(SupportedPlatforms.Custom);

        var runtimeModes = dataProviderConfiguration.FindPropertyRelative("runtimeModes");
        runtimeModes.intValue = -1;
    }

    protected static void GatherSupportedPlatforms(SerializedProperty mixedRealitySpatialObserverConfigurations)
    {
        runtimePlatformTypes = PlatformSupportExtension.GetSupportedPlatformTypes();
        runtimePlatformNames = PlatformSupportExtension.GetSupportedPlatformNames();
        for (int i = 0; i < mixedRealitySpatialObserverConfigurations.arraySize; i++)
        {
            CreateRuntimePlatformMask(mixedRealitySpatialObserverConfigurations, i);
        }
    }

    protected static void CreateRuntimePlatformMask(SerializedProperty mixedRealitySpatialObserverConfigurationsProvider, int index)
    {
        string platformName;
        SerializedProperty supportedPlatformsArray = mixedRealitySpatialObserverConfigurationsProvider.GetArrayElementAtIndex(index).FindPropertyRelative("customizedRuntimePlatform");

        if (index < runtimePlatformMasks.Count)
        {
            runtimePlatformMasks[index] = 0;
        }
        else
        {
            runtimePlatformMasks.Add(0);
        }

        for (int j = 0; j < runtimePlatformTypes.Length; j++)
        {
            platformName = SystemType.GetReference(runtimePlatformTypes[j]);
            for (int k = 0; k < supportedPlatformsArray.arraySize; k++)
            {
                if (platformName.Equals(supportedPlatformsArray.GetArrayElementAtIndex(k).FindPropertyRelative("reference").stringValue))
                {
                    runtimePlatformMasks[index] |= 1 << j;
                }
            }
        }
    }

    protected static void RenderSupportedPlatforms(SerializedProperty runtimePlatform, SerializedProperty customRuntimePlatform, int index, GUIContent runtimePlatformContent = null)
    {
        EditorGUILayout.PropertyField(runtimePlatform, runtimePlatformContent);

        if ((runtimePlatform.intValue & (int)(SupportedPlatforms.Custom)) != 0)
        {
            runtimePlatformMasks[index] = EditorGUILayout.MaskField(runtimePlatformContent, runtimePlatformMasks[index], runtimePlatformNames);
            ApplyMaskToProperty(customRuntimePlatform, runtimePlatformMasks[index]);
        }
    }

    protected static void ApplyMaskToProperty(SerializedProperty runtimePlatform, int runtimePlatformBitMask)
    {
        runtimePlatform.arraySize = MathExtensions.CountBits(runtimePlatformBitMask);
        int arrayIndex = 0;
        for (int i = 0; i < runtimePlatformTypes.Length; i++)
        {
            if ((runtimePlatformBitMask & 1 << i) != 0)
            {
                runtimePlatform.GetArrayElementAtIndex(arrayIndex++).FindPropertyRelative("reference").stringValue = SystemType.GetReference(runtimePlatformTypes[i]);
            }
        }
    }
}
