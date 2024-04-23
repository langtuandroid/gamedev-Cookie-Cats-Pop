using System;
using Tactile;
using TactileModules.Analytics.Interfaces;

namespace TactileModules.Inventory
{
	public class InventoryEventLogger
	{
		public InventoryEventLogger(InventoryManager inventoryManager, IAnalytics analyticsInstance, IAdjustInventoryTracking adjustInventoryTracking)
		{
			inventoryManager.InventoryChanged += this.HandleInventoryChanged;
			this.analytics = analyticsInstance;
			this.adjustInventoryTracking = adjustInventoryTracking;
		}

		private void HandleInventoryChanged(InventoryManager.ItemChangeInfo info)
		{
			if (info.ProductOrCause == null)
			{
				return;
			}
			if (info.Item == "Coin")
			{
				if (info.ChangeByAmount < 0)
				{
					this.LogCoinsUsed(info);
				}
				else
				{
					this.LogCoinsReceived(info);
				}
			}
			else if (info.ChangeByAmount < 0)
			{
				this.LogItemsUsed(info);
			}
			else
			{
				this.LogItemsReceived(info);
			}
		}

		private void LogCoinsReceived(InventoryManager.ItemChangeInfo info)
		{
			this.analytics.LogEvent(new CoinsReceivedEvent(info), -1.0, null);
		}

		private void LogCoinsUsed(InventoryManager.ItemChangeInfo info)
		{
			this.analytics.LogEvent(new CoinsUsedEvent(info), -1.0, null);
			this.adjustInventoryTracking.TrackAdjustCoinsUsed();
		}

		private void LogItemsReceived(InventoryManager.ItemChangeInfo info)
		{
			this.analytics.LogEvent(new ItemsReceivedEvent(info), -1.0, null);
		}

		private void LogItemsUsed(InventoryManager.ItemChangeInfo info)
		{
			this.analytics.LogEvent(new ItemsUsedEvent(info), -1.0, null);
		}

		private readonly IAnalytics analytics;

		private readonly IAdjustInventoryTracking adjustInventoryTracking;
	}
}
