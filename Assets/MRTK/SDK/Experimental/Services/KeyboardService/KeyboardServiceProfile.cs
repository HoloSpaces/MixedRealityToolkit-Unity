using System;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Experimental.UI;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
	[MixedRealityServiceProfile(typeof(IKeyboardService))]
	[CreateAssetMenu(fileName = "KeyboardServiceProfile", menuName = "MixedRealityToolkit/KeyboardService Configuration Profile")]
	public class KeyboardServiceProfile : BaseMixedRealityProfile
	{
		public GameObject nonNativeKeyboardPrefab = default;
		// Store config data in serialized fields
	}
}