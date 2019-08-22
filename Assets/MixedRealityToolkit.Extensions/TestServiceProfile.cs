using System;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Extensions
{
	[MixedRealityServiceProfile(typeof(ITestService))]
	[CreateAssetMenu(fileName = "TestServiceProfile", menuName = "MixedRealityToolkit/TestService Configuration Profile")]
	public class TestServiceProfile : BaseMixedRealityProfile
	{
		// Store config data in serialized fields
	}
}