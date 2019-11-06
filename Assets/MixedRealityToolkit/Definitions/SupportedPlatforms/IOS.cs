#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class IOS : IPlatformSupport
{
    public bool IsEditorOrRuntimePlatform()
    {
#if UNITY_EDITOR
        return EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS;
#else
        return Application.platform == RuntimePlatform.IPhonePlayer;
#endif
    }

    public bool IsCurrentRuntimePlatformSameAs(RuntimePlatform runtimePlatform)
    {
        return runtimePlatform == RuntimePlatform.IPhonePlayer && IsEditorOrRuntimePlatform();
    }
}
