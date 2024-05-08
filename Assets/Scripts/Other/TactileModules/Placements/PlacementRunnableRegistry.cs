using System;
using System.Collections.Generic;
using System.Diagnostics;
using TactileModules.Analytics.Interfaces;

namespace TactileModules.Placements
{
	public class PlacementRunnableRegistry : IPlacementRunnableRegistry
	{
		public PlacementRunnableRegistry(IConfigPropertyGetter<PlacementConfigData> configPropertyGetter, IPlacementEnumerator placementEnumerator)
		{
			this.configPropertyGetter = configPropertyGetter;
			this.placementEnumerator = placementEnumerator;
			this.registeredPlacementRunnables = new Dictionary<string, IPlacementRunnable>();
			this.defaultRunnablePlacements = new Dictionary<IPlacementRunnable, PlacementRunnableRegistry.PlacementData>();
		}

		public void RegisterRunnable(IPlacementRunnable runnable)
		{
			if (this.registeredPlacementRunnables.ContainsKey(runnable.ID))
			{
				throw new Exception(string.Format("An IPlacementRunnable with ID {0} is already registered", runnable.ID));
			}
			this.registeredPlacementRunnables.Add(runnable.ID, runnable);
		}

		public void RegisterRunnable(IPlacementRunnable runnable, PlacementIdentifier defaultPlacement, PlacementBehavior defaultBehavior)
		{
			this.RegisterRunnable(runnable);
			this.defaultRunnablePlacements.Add(runnable, new PlacementRunnableRegistry.PlacementData
			{
				placement = defaultPlacement,
				behavior = defaultBehavior
			});
		}

		public List<IPlacementRunnable> GetRunnables(PlacementIdentifier placement, PlacementBehavior behavior)
		{
			List<IPlacementRunnable> list = new List<IPlacementRunnable>();
			PlacementConfigData placementConfigData = this.configPropertyGetter.Get(placement.ID);
			if (placementConfigData == null)
			{
				string data = "Could not locate config for placement " + placement.ID;
				return list;
			}
			List<PlacementConfigData.PlacementRunnableData> list2 = (behavior != PlacementBehavior.Skippable) ? placementConfigData.UnskippableRunnables : placementConfigData.SkippableRunnables;
			foreach (PlacementConfigData.PlacementRunnableData placementRunnableData in list2)
			{
				if (placementRunnableData.Enabled && this.registeredPlacementRunnables.ContainsKey(placementRunnableData.ID))
				{
					list.Add(this.registeredPlacementRunnables[placementRunnableData.ID]);
				}
			}
			list.AddRange(this.GetRunnablesWithNoConfigAndMatchingDefaultPlacement(placement, behavior));
			return list;
		}

		private List<IPlacementRunnable> GetRunnablesWithNoConfigAndMatchingDefaultPlacement(PlacementIdentifier placement, PlacementBehavior behavior)
		{
			List<IPlacementRunnable> list = new List<IPlacementRunnable>();
			List<IPlacementRunnable> allRunnablesInConfig = this.GetAllRunnablesInConfig();
			foreach (KeyValuePair<string, IPlacementRunnable> keyValuePair in this.registeredPlacementRunnables)
			{
				IPlacementRunnable value = keyValuePair.Value;
				if (!allRunnablesInConfig.Contains(value) && this.DoesRunnableHaveMatchingDefaultPlacement(value, placement, behavior))
				{
					list.Add(value);
				}
			}
			return list;
		}

		private bool DoesRunnableHaveMatchingDefaultPlacement(IPlacementRunnable runnable, PlacementIdentifier placement, PlacementBehavior behavior)
		{
			return this.defaultRunnablePlacements.ContainsKey(runnable) && this.defaultRunnablePlacements[runnable].placement == placement && this.defaultRunnablePlacements[runnable].behavior == behavior;
		}

		private List<IPlacementRunnable> GetAllRunnablesInConfig()
		{
			List<IPlacementRunnable> result = new List<IPlacementRunnable>();
			IEnumerable<PlacementIdentifier> placements = this.placementEnumerator.GetPlacements();
			foreach (PlacementIdentifier placementIdentifier in placements)
			{
				PlacementConfigData placementConfigData = this.configPropertyGetter.Get(placementIdentifier.ID);
				if (placementConfigData != null)
				{
					this.AddRegisteredRunnablesToList(placementConfigData.UnskippableRunnables, result);
					this.AddRegisteredRunnablesToList(placementConfigData.SkippableRunnables, result);
				}
			}
			return result;
		}

		private void AddRegisteredRunnablesToList(List<PlacementConfigData.PlacementRunnableData> runnableDatas, List<IPlacementRunnable> result)
		{
			foreach (PlacementConfigData.PlacementRunnableData placementRunnableData in runnableDatas)
			{
				if (this.registeredPlacementRunnables.ContainsKey(placementRunnableData.ID))
				{
					IPlacementRunnable item = this.registeredPlacementRunnables[placementRunnableData.ID];
					if (!result.Contains(item))
					{
						result.Add(item);
					}
				}
			}
		}

		private readonly IConfigPropertyGetter<PlacementConfigData> configPropertyGetter;

		private readonly IPlacementEnumerator placementEnumerator;

		private readonly Dictionary<string, IPlacementRunnable> registeredPlacementRunnables;

		private readonly Dictionary<IPlacementRunnable, PlacementRunnableRegistry.PlacementData> defaultRunnablePlacements;

		private struct PlacementData
		{
			public PlacementIdentifier placement;

			public PlacementBehavior behavior;
		}
	}
}
