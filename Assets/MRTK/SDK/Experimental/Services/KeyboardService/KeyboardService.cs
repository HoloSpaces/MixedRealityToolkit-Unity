using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit;

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
		private NonNativeKeyboard keyboardInstance;

		public KeyboardService(string name,  uint priority,  BaseMixedRealityProfile profile) : base(name, priority, profile) 
		{
			keyboardServiceProfile = (KeyboardServiceProfile)profile;
		}

		public override void Initialize()
		{
			base.Initialize();
		}

		public override void Update()
		{
			base.Update();
		}

		public NonNativeKeyboard GetKeyboardInstance()
		{
			if (keyboardInstance.IsNull())
			{
				GameObject keyboardObject = Object.Instantiate(keyboardServiceProfile.nonNativeKeyboardPrefab, Vector3.zero, Quaternion.identity);
				Object.DontDestroyOnLoad(keyboardObject); // persistant across the app

				keyboardInstance = keyboardObject.GetComponent<NonNativeKeyboard>();
				keyboardInstance.SliderEnabled = false;
				keyboardInstance.CloseOnSubmit = false;
				keyboardInstance.CloseOnInactivity = false;

				Debug.Log("creating new keyboad");
			}

			return keyboardInstance;
		}	
	}
}
