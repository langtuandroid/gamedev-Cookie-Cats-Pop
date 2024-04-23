using System;
using Tactile;
using TactileModules.Analytics.Interfaces;
using TactileModules.Inventory;

namespace TactileModules.PuzzleGame.Inventory
{
	public class UnlimitedItemsAnalytics : IUnlimitedItemsAnalytics
	{
		public UnlimitedItemsAnalytics(IAnalytics analytics)
		{
			this.analytics = analytics;
		}

		public void LogUnlimitedItemActivated(InventoryManager.ItemChangeInfo info, int previouslyAvailableUnlimitedSeconds)
		{
			this.analytics.LogEvent(new UnlimitedItemActivatedEvent(info, previouslyAvailableUnlimitedSeconds), -1.0, null);
		}

		private readonly IAnalytics analytics;
	}
}
