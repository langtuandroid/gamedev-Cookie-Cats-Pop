using System;

namespace TactileModules.SideMapButtons
{
	public class SideMapButtonSystem
	{
		public SideMapButtonSystem(SideMapButtonControllerProviderRegistry registry, ISideButtonsAreaLifecycleHandler sideButtonsAreaLifecycleHandler)
		{
			this.Registry = registry;
			this.LifecycleHandler = sideButtonsAreaLifecycleHandler;
		}

		public SideMapButtonControllerProviderRegistry Registry { get; private set; }

		public ISideButtonsAreaLifecycleHandler LifecycleHandler { get; private set; }
	}
}
