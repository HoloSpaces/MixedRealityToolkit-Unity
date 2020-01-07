#if UNITY_EDITOR
using System;
using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit;
using UnityEngine;
using UnityEditor;

namespace HoloSpaces.Extensions.Editor
{	
	[MixedRealityServiceInspector(typeof(IControllerColliderAttacherService))]
	public class ControllerColliderAttacherServiceInspector : BaseMixedRealityServiceInspector
	{
		public override void DrawInspectorGUI(object target)
		{
			ControllerColliderAttacherService service = (ControllerColliderAttacherService)target;
			
			// Draw inspector here
		}
	}
}

#endif