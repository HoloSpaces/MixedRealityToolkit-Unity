using System;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;

namespace HoloSpaces.Extensions
{
	[MixedRealityServiceProfile(typeof(IControllerColliderAttacherService))]
	[CreateAssetMenu(fileName = "ControllerColliderAttacherServiceProfile", menuName = "MixedRealityToolkit/ControllerColliderAttacherService Configuration Profile")]
	public class ControllerColliderAttacherServiceProfile : BaseMixedRealityProfile
	{
		// Store config data in serialized fields
	}
}