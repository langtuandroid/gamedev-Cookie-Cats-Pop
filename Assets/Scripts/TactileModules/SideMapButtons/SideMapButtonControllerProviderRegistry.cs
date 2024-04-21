using System;
using System.Collections.Generic;

namespace TactileModules.SideMapButtons
{
	public class SideMapButtonControllerProviderRegistry : ISideMapButtonControllerProviderRegistry
	{
		public List<ISideMapButtonControllerProvider> Providers
		{
			get
			{
				return this.providers;
			}
		}

		public void Register(ISideMapButtonControllerProvider controllerProvider)
		{
			this.providers.Add(controllerProvider);
		}

		private readonly List<ISideMapButtonControllerProvider> providers = new List<ISideMapButtonControllerProvider>();
	}
}
