﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information. 

using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEngine;
using UnityEditor;
using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.SpatialAwareness.Editor
{
    [CustomEditor(typeof(MixedRealitySpatialAwarenessSystemProfile))]
    public class MixedRealitySpatialAwarenessSystemProfileInspector : BaseMixedRealityToolkitRuntimePlatformConfigurationProfileInspector
    {
        private static readonly GUIContent AddObserverContent = new GUIContent("+ Add Spatial Observer", "Add Spatial Observer");
        private static readonly GUIContent RemoveObserverContent = new GUIContent("-", "Remove Spatial Observer");

        private static readonly GUIContent ComponentTypeContent = new GUIContent("Type");
        private static readonly GUIContent RuntimePlatformContent = new GUIContent("Supported Platform(s)");

        private SerializedProperty observerConfigurations;

        private const string ProfileTitle = "Spatial Awareness System Settings";
        private const string ProfileDescription = "The Spatial Awareness System profile allows developers to configure cross-platform environmental awareness.";

        private static bool[] observerFoldouts;

        protected override void OnEnable()
        {
            base.OnEnable();

            observerConfigurations = serializedObject.FindProperty("observerConfigurations");

            if (observerFoldouts == null || observerFoldouts.Length != observerConfigurations.arraySize)
            {
                observerFoldouts = new bool[observerConfigurations.arraySize];
            }

            GatherSupportedPlatforms(observerConfigurations);
        }

        public override void OnInspectorGUI()
        {
            RenderProfileHeader(ProfileTitle, ProfileDescription, target);

            using (new GUIEnabledWrapper(!IsProfileLock((BaseMixedRealityProfile)target)))
            {
                serializedObject.Update();

                using (new EditorGUI.IndentLevelScope())
                {
                    RenderList(observerConfigurations);
                }

                serializedObject.ApplyModifiedProperties();
            }
        }

        protected override bool IsProfileInActiveInstance()
        {
            var profile = target as BaseMixedRealityProfile;
            return MixedRealityToolkit.IsInitialized && profile != null &&
                   MixedRealityToolkit.Instance.HasActiveProfile &&
                   profile == MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessSystemProfile;
        }

        private void RenderList(SerializedProperty list)
        {
            bool changed = false;

            using (new EditorGUILayout.VerticalScope())
            {
                if (InspectorUIUtility.RenderIndentedButton(AddObserverContent, EditorStyles.miniButton))
                {
                    list.InsertArrayElementAtIndex(list.arraySize);
                    SerializedProperty observer = list.GetArrayElementAtIndex(list.arraySize - 1);

                    SerializedProperty observerName = observer.FindPropertyRelative("componentName");
                    observerName.stringValue = $"New spatial observer {list.arraySize - 1}";

                    SerializedProperty runtimePlatform = observer.FindPropertyRelative("runtimePlatform");
                    runtimePlatform.intValue = -1;

                    SerializedProperty customizedRuntimePlatform = observer.FindPropertyRelative("customizedRuntimePlatform");
                    customizedRuntimePlatform.objectReferenceValue = null;

                    SerializedProperty configurationProfile = observer.FindPropertyRelative("observerProfile");
                    configurationProfile.objectReferenceValue = null;

                    serializedObject.ApplyModifiedProperties();

                    SystemType observerType = ((MixedRealitySpatialAwarenessSystemProfile)serializedObject.targetObject).ObserverConfigurations[list.arraySize - 1].ComponentType;
                    observerType.Type = null;

                    observerFoldouts = new bool[list.arraySize];
                    return;
                }

                if (list == null || list.arraySize == 0)
                {
                    EditorGUILayout.HelpBox("The Mixed Reality Spatial Awareness System requires one or more observers.", MessageType.Warning);
                    return;
                }

                for (int i = 0; i < list.arraySize; i++)
                {
                    SerializedProperty observer = list.GetArrayElementAtIndex(i);
                    SerializedProperty observerName = observer.FindPropertyRelative("componentName");
                    SerializedProperty observerType = observer.FindPropertyRelative("componentType");
                    SerializedProperty observerProfile = observer.FindPropertyRelative("observerProfile");
                    SerializedProperty runtimePlatform = observer.FindPropertyRelative("runtimePlatform");
                    SerializedProperty customizedRuntimePlatform = observer.FindPropertyRelative("customizedRuntimePlatform");

                    using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            observerFoldouts[i] = EditorGUILayout.Foldout(observerFoldouts[i], observerName.stringValue, true);

                            if (GUILayout.Button(RemoveObserverContent, EditorStyles.miniButtonRight, GUILayout.Width(24f)))
                            {
                                list.DeleteArrayElementAtIndex(i);
                                serializedObject.ApplyModifiedProperties();
                                changed = true;
                                break;
                            }
                        }

                        if (observerFoldouts[i])
                        {
                            System.Type serviceType = null;
                            if (observerProfile.objectReferenceValue != null)
                            {
                                EditorGUI.BeginChangeCheck();
                                EditorGUILayout.PropertyField(observerType, ComponentTypeContent);
                                if (EditorGUI.EndChangeCheck())
                                {
                                    serializedObject.ApplyModifiedProperties();
                                    System.Type type = ((MixedRealitySpatialAwarenessSystemProfile)serializedObject.targetObject).ObserverConfigurations[i].ComponentType.Type;
                                    ApplyObserverConfiguration(type, observerName, observerProfile, customizedRuntimePlatform, i);
                                    break;
                                }

                                EditorGUI.BeginChangeCheck();
                                RenderSupportedPlatforms(runtimePlatform, customizedRuntimePlatform, i, RuntimePlatformContent);
                                changed |= EditorGUI.EndChangeCheck();

                                if (observerProfile.objectReferenceValue != null)
                                {
                                    serviceType = (target as MixedRealitySpatialAwarenessSystemProfile).ObserverConfigurations[i].ComponentType;
                                }
                            }

                            changed |= RenderProfile(observerProfile, null, true, false, serviceType);

                            serializedObject.ApplyModifiedProperties();
                        }
                    }
                }

                if (changed && MixedRealityToolkit.IsInitialized)
                {
                    EditorApplication.delayCall += () => MixedRealityToolkit.Instance.ResetConfiguration(MixedRealityToolkit.Instance.ActiveProfile);
                }
            }
        }

        private void ApplyObserverConfiguration(
            System.Type type, 
            SerializedProperty observerName,
            SerializedProperty configurationProfile,
            SerializedProperty runtimePlatform,
            int index)
        {
            if (type != null)
            {
                MixedRealityDataProviderAttribute observerAttribute = MixedRealityDataProviderAttribute.Find(type) as MixedRealityDataProviderAttribute;
                if (observerAttribute != null)
                {
                    observerName.stringValue = !string.IsNullOrWhiteSpace(observerAttribute.Name) ? observerAttribute.Name : type.Name;
                    configurationProfile.objectReferenceValue = observerAttribute.DefaultProfile;
                    ApplyMaskToProperty(runtimePlatform, runtimePlatformMasks[index]);

                }
                else
                {
                    observerName.stringValue = type.Name;
                }

                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}