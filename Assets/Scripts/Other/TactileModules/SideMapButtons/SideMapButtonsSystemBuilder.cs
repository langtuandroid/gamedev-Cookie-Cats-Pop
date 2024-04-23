using System;

namespace TactileModules.SideMapButtons
{
	public static class SideMapButtonsSystemBuilder
	{
		public static SideMapButtonSystem Build()
		{
			SideMapButtonControllerProviderRegistry registry = new SideMapButtonControllerProviderRegistry();
			SideButtonsAreaLifecycleHandler sideButtonsAreaLifecycleHandler = new SideButtonsAreaLifecycleHandler(registry);
			return new SideMapButtonSystem(registry, sideButtonsAreaLifecycleHandler);
		}
	}
}
