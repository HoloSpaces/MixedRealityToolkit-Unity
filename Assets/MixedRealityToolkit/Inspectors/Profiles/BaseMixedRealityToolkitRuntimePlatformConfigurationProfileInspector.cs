using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using UnityEditor;
using UnityEngine;

public class BaseMixedRealityToolkitRuntimePlatformConfigurationProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
{
    private static readonly GUIContent MinusButtonContent = new GUIContent("<-", "Revert to regular platforms");

    protected static string[] runtimePlatformNames;
    protected static Type[] runtimePlatformTypes;
    protected static int[] runtimePlatformMasks;

    protected override bool IsProfileInActiveInstance()
    {
        return MixedRealityToolkit.IsInitialized &&
               MixedRealityToolkit.Instance.HasActiveProfile;
    }

    protected void GatherSupportedPlatforms(SerializedProperty serializedProperty)
    {
        runtimePlatformTypes = PlatformSupportExtension.GetSupportedPlatformTypes();
        runtimePlatformNames = PlatformSupportExtension.GetSupportedPlatformNames();

        runtimePlatformMasks = new int[serializedProperty.arraySize];
        SerializedProperty supportedPlatformsArray;
        string platformName;
        for (int i = 0; i < serializedProperty.arraySize; i++)
        {
            supportedPlatformsArray = serializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("customizedRuntimePlatform");

            for (int j = 0; j < runtimePlatformTypes.Length; j++)
            {
                platformName = SystemType.GetReference(runtimePlatformTypes[j]);
                for (int k = 0; k < supportedPlatformsArray.arraySize; k++)
                {
                    if (platformName.Equals(supportedPlatformsArray.GetArrayElementAtIndex(k).FindPropertyRelative("reference").stringValue))
                    {
                        runtimePlatformMasks[i] |= 1 << j;
                    }
                }
            }
        }
    }

    protected static void RenderSupportedPlatforms(SerializedProperty runtimePlatform, SerializedProperty customRuntimePlatform, int index, UnityEngine.GUIContent runtimePlatformContent = null)
    {
        EditorGUILayout.PropertyField(runtimePlatform, runtimePlatformContent);

        if ((runtimePlatform.intValue & (int)SupportedPlatforms.Custom) != 0)
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
                runtimePlatform.GetArrayElementAtIndex(arrayIndex).FindPropertyRelative("reference").stringValue = SystemType.GetReference(runtimePlatformTypes[i]);
                arrayIndex++;
            }
        }
    }
}
