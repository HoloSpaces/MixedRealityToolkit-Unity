using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
	[MixedRealityServiceProfile(typeof(IKeyboardService))]
	[CreateAssetMenu(fileName = "KeyboardServiceProfile", menuName = "MixedRealityToolkit/KeyboardService Configuration Profile")]
	public class KeyboardServiceProfile : BaseMixedRealityProfile
	{
		public HoloNonNativeKeyboard nonNativeKeyboardPrefab = default;
		// Store config data in serialized fields
	}
}