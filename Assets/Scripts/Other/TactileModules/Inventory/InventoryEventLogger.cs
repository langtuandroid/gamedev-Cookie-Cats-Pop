using System;
using Tactile;
using TactileModules.Analytics.Interfaces;

namespace TactileModules.Inventory
{
	public class InventoryEventLogger
	{
		public InventoryEventLogger(InventoryManager inventoryManager, IAdjustInventoryTracking adjustInventoryTracking)
		{
			inventoryManager.InventoryChanged += this.HandleInventoryChanged;
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
		}

		private void LogCoinsUsed(InventoryManager.ItemChangeInfo info)
		{
			this.adjustInventoryTracking.TrackAdjustCoinsUsed();
		}

		private void LogItemsReceived(InventoryManager.ItemChangeInfo info)
		{
		}

		private void LogItemsUsed(InventoryManager.ItemChangeInfo info)
		{
		}

		private readonly IAdjustInventoryTracking adjustInventoryTracking;
	}
}
