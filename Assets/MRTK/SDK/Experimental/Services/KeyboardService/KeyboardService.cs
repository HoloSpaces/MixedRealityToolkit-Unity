using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
	/// <summary>
	/// KeyboardService keeps the keyboard instance as persitant one across the scenes
	/// This service need to be manually registered using the Mixed Reality Toolkit configuration inspector.
	/// https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/Extensions/ExtensionServices.html
	/// </summary>
	[MixedRealityExtensionService((SupportedPlatforms)(-1))]
	public class KeyboardService : BaseExtensionService, IKeyboardService, IMixedRealityExtensionService
	{
		private KeyboardServiceProfile keyboardServiceProfile;
		private HoloNonNativeKeyboard keyboardInstance;

		public KeyboardService(string name,  uint priority,  BaseMixedRealityProfile profile) : base(name, priority, profile) 
		{
			keyboardServiceProfile = (KeyboardServiceProfile)profile;
		}

		public static KeyboardService GetKeyboardServiceInstance() => MixedRealityServiceRegistry.TryGetService(out KeyboardService service) ? service : null;

		public HoloNonNativeKeyboard GetKeyboardInstance()
		{
			if (keyboardInstance.IsNull())
			{
				keyboardInstance = Object.Instantiate(keyboardServiceProfile.nonNativeKeyboardPrefab, Vector3.zero, Quaternion.identity);
				Object.DontDestroyOnLoad(keyboardInstance.gameObject); // persistant across the app
				Debug.Log("creating new keyboad");
			}

			return keyboardInstance;
		}	
	}
}
