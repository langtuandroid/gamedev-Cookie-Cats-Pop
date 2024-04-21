using System;

namespace TactileModules.Placements
{
	public class PlacementSystem
	{
		public PlacementSystem(IPlacementRunner placementRunner, IPlacementRunnableRegistry registry)
		{
			this.PlacementRunnableRegistry = registry;
			this.PlacementRunner = placementRunner;
		}

		public IPlacementRunner PlacementRunner { get; private set; }

		public IPlacementRunnableRegistry PlacementRunnableRegistry { get; private set; }
	}
}
