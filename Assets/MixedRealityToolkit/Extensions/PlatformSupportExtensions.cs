using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Linq;

public static class PlatformSupportExtension
{
    public static bool IsPlatformSupported(this IPlatformSupport[] platformSupports)
    {
        if (platformSupports == null)
        {
            return true;
        }

        foreach (var item in platformSupports)
        {
            if (item.IsEditorOrRuntimePlatform())
                return true;
        }

        return false;
    }

    public static IPlatformSupport[] Convert(this SystemType[] systemTypes)
    {
        var supportedPlatforms = new IPlatformSupport[systemTypes.Length];

        for (int i = 0; i < systemTypes.Length; i++)
        {
            supportedPlatforms[i] = Activator.CreateInstance(systemTypes[i]) as IPlatformSupport;
        }

        return supportedPlatforms;
    }

    public static SystemType[] Convert(this IPlatformSupport[] supportedPlatforms)
    {
        var systemTypes = new SystemType[supportedPlatforms.Length];

        for (int i = 0; i < supportedPlatforms.Length; i++)
        {
            systemTypes[i] = new SystemType(supportedPlatforms[i].GetType());
        }

        return systemTypes;
    }

    public static Type[] GetSupportedPlatformTypes()
    {
#if UNITY_EDITOR
        return (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypes()
                where typeof(IPlatformSupport).IsAssignableFrom(type)
                where type != typeof(IPlatformSupport)
                orderby type.Name
                select type).ToArray();
#else
        return null;
#endif
    }

    public static string[] GetSupportedPlatformNames()
    {
#if UNITY_EDITOR
        return (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypes()
                where typeof(IPlatformSupport).IsAssignableFrom(type)
                where type != typeof(IPlatformSupport)
                orderby type.Name
                select type.Name).ToArray();
#else
        return null;
#endif
    }
}
