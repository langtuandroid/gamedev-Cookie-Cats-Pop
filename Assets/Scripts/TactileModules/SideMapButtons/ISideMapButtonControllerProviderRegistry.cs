using System;
using System.Collections.Generic;

namespace TactileModules.SideMapButtons
{
	public interface ISideMapButtonControllerProviderRegistry
	{
		List<ISideMapButtonControllerProvider> Providers { get; }

		void Register(ISideMapButtonControllerProvider controllerProvider);
	}
}
