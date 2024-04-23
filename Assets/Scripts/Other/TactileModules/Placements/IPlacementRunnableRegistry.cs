using System;
using System.Collections.Generic;

namespace TactileModules.Placements
{
	public interface IPlacementRunnableRegistry
	{
		void RegisterRunnable(IPlacementRunnable runnable);

		void RegisterRunnable(IPlacementRunnable runnable, PlacementIdentifier defaultPlacement, PlacementBehavior defaultBehavior);

		List<IPlacementRunnable> GetRunnables(PlacementIdentifier placement, PlacementBehavior behavior);
	}
}
