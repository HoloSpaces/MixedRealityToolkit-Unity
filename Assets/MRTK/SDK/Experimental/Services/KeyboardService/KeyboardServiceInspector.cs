#if UNITY_EDITOR
using System;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Editor;
using UnityEngine;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{	
	[MixedRealityServiceInspector(typeof(IKeyboardService))]
	public class KeyboardServiceInspector : BaseMixedRealityServiceInspector
	{
		public override void DrawInspectorGUI(object target)
		{
			KeyboardService service = (KeyboardService)target;
			
			// Draw inspector here
		}
	}
}

#endif