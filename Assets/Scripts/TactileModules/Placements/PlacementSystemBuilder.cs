using System;
using TactileModules.Analytics.Interfaces;
using TactileModules.PuzzleGames.Configuration;

namespace TactileModules.Placements
{
	public static class PlacementSystemBuilder
	{
		public static PlacementSystem Build(IConfigurationManager configurationManager, IUIViewManager uiViewManager, IAnalytics analytics)
		{
			ConfigGetter<PlacementsConfig> configGetter = new ConfigGetter<PlacementsConfig>(configurationManager);
			ConfigPropertyGetter<PlacementsConfig, PlacementConfigData> configPropertyGetter = new ConfigPropertyGetter<PlacementsConfig, PlacementConfigData>(configGetter);
			PlacementRunnableRegistry registry = new PlacementRunnableRegistry(configPropertyGetter, new PlacementEnumerator(), analytics);
			PlacementViewMediator viewMediator = new PlacementViewMediator(uiViewManager);
			PlacementRunner placementRunner = new PlacementRunner(registry, viewMediator, configPropertyGetter, analytics);
			return new PlacementSystem(placementRunner, registry);
		}
	}
}
