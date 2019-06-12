using UnityEngine;

public interface IPlatformSupport
{
    bool IsEditorOrRuntimePlatform();

    bool IsCurrentRuntimePlatformSameAs(RuntimePlatform runtimePlatform);
}