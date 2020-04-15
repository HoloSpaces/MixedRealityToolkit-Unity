using System;
using Microsoft.MixedReality.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
	public interface IKeyboardService : IMixedRealityExtensionService
	{
		INonNativeKeyboard GetKeyboardInstance();
	}
}