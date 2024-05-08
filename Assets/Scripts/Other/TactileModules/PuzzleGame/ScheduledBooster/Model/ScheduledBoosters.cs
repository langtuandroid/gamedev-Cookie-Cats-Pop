using System;
using System.Collections.Generic;
using System.Diagnostics;
using TactileModules.Analytics.Interfaces;
using TactileModules.FeatureManager.Interfaces;
using TactileModules.PuzzleGame.ScheduledBooster.Analytics;
using TactileModules.PuzzleGame.ScheduledBooster.Data;
using TactileModules.PuzzleGame.ScheduledBooster.Views;

namespace TactileModules.PuzzleGame.ScheduledBooster.Model
{
	public class ScheduledBoosters : IScheduledBoosters
	{
		public ScheduledBoosters(IScheduledBoosterProvider provider, IScheduledBoosterViewProvider viewProvider, IScheduledBoosterInventoryProvider inventoryProvider)
		{
			this.Provider = provider;
			this.InventoryProvider = inventoryProvider;
		}

		public IScheduledBoosterProvider Provider { get; private set; }

		public IScheduledBoosterInventoryProvider InventoryProvider { get; private set; }

		public void AddBooster(IScheduledBooster booster)
		{
			if (!this.scheduledBoosters.ContainsKey(booster.Type))
			{
				this.scheduledBoosters.Add(booster.Type, booster);
			}
			else
			{
				string errorName = "[ScheduledBoosters] Trying to add two scheduled boosters with the same key.";
				StackTrace stackTrace = new StackTrace();
			}
		}

		public void RemoveBooster(string boosterType)
		{
			this.scheduledBoosters.Remove(boosterType);
		}

		public IScheduledBooster GetBooster(string boosterType)
		{
			IScheduledBooster result;
			this.scheduledBoosters.TryGetValue(boosterType, out result);
			return result;
		}

		public IScheduledBooster GetBoosterForLocation(ScheduledBoosterLocation location)
		{
			if (this.HasBoosterForLocation(location))
			{
				return this.GetBoosters(location)[0];
			}
			return null;
		}

		public List<IScheduledBooster> GetBoosters()
		{
			List<IScheduledBooster> list = new List<IScheduledBooster>();
			foreach (KeyValuePair<string, IScheduledBooster> keyValuePair in this.scheduledBoosters)
			{
				list.Add(keyValuePair.Value);
			}
			return list;
		}

		public bool UseScheduledBoosterIfPossible(IScheduledBooster booster, string levelSessionId, string context)
		{
			if (booster != null && booster.IsActive)
			{
				this.UseBooster(booster, levelSessionId, context);
				return true;
			}
			return false;
		}

		private void UseBooster(IScheduledBooster booster, string levelSessionId, string context)
		{
			ScheduledBoosterAnalytics.LogLimitedBoosterUsed(booster.Type, booster.IsFree(), booster.GetSecondsLeft());
			booster.GetInstanceCustomData().NumberOfBoostersUsed++;
			booster.Deactivate();
		}

		public bool HasBoosterForLocation(ScheduledBoosterLocation location, ILevelProxy levelProxy)
		{
			return this.IsLocationValid(location, levelProxy) && this.HasBoosterForLocation(location);
		}

		private bool IsLocationValid(ScheduledBoosterLocation location, ILevelProxy levelProxy)
		{
			return location != ScheduledBoosterLocation.PreGame || !this.Provider.IsTutorialLevel(levelProxy);
		}

		private bool HasBoosterForLocation(ScheduledBoosterLocation location)
		{
			return this.GetBoosters(location).Count<IScheduledBooster>() > 0;
		}

		private List<IScheduledBooster> GetBoosters(ScheduledBoosterLocation location)
		{
			List<IScheduledBooster> list = new List<IScheduledBooster>();
			foreach (KeyValuePair<string, IScheduledBooster> keyValuePair in this.scheduledBoosters)
			{
				if (keyValuePair.Value.GetInstanceCustomData() != null)
				{
					if (keyValuePair.Value.Definition.location == location)
					{
						list.Add(keyValuePair.Value);
					}
				}
			}
			return list;
		}

		public bool CanBuyBooster(string boosterType)
		{
			IScheduledBooster booster = this.GetBooster(boosterType);
			return this.InventoryProvider.GetCoins() >= booster.Price;
		}

		private readonly IFeatureManager featureManager;

		private readonly IScheduledBoosterDefinitions definitionsUtility;

		private readonly Dictionary<string, IScheduledBooster> scheduledBoosters = new Dictionary<string, IScheduledBooster>();
	}
}
