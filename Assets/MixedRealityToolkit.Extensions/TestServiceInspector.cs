#if UNITY_EDITOR
using System;
using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit;
using UnityEngine;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Extensions.Editor
{	
	[MixedRealityServiceInspector(typeof(TestService))]
	public class TestServiceInspector : BaseMixedRealityServiceInspector
	{
		public override void DrawInspectorGUI(object target)
		{
			TestService service = (TestService)target;
			
			// Draw inspector here
		}
	}
}

#endif