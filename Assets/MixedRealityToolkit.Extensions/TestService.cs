using System;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Extensions
{
	[MixedRealityExtensionService(SupportedPlatforms.WindowsStandalone|SupportedPlatforms.MacStandalone|SupportedPlatforms.LinuxStandalone|SupportedPlatforms.Android)]
	public class TestService : BaseExtensionService, ITestService, IMixedRealityExtensionService
	{
		private TestServiceProfile testServiceProfile;

		public TestService(IMixedRealityServiceRegistrar registrar,  string name,  uint priority,  BaseMixedRealityProfile profile) : base(registrar, name, priority, profile) 
		{
			testServiceProfile = (TestServiceProfile)profile;
		}

		public override void Initialize()
		{
			// Do service initialization here.
		}

		public override void Update()
		{
			// Do service updates here.
		}
	}
}