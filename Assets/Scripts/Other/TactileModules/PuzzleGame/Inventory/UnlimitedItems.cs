using System;
using Tactile;
using TactileModules.Timing.Interfaces;

namespace TactileModules.PuzzleGame.Inventory
{
	public class UnlimitedItems : IUnlimitedItems
	{
		public UnlimitedItems(IInventoryManager inventoryManager, ITimingManager timingManager, IUnlimitedItemsProvider unlimitedItemsProvider, IUnlimitedItemsAnalytics unlimitedItemsAnalytics)
		{
			this.inventoryManager = inventoryManager;
			this.timingManager = timingManager;
			this.unlimitedItemsProvider = unlimitedItemsProvider;
			this.unlimitedItemsAnalytics = unlimitedItemsAnalytics;
			inventoryManager.InventoryChanged += this.InventoryChangedHandler;
		}

		private void InventoryChangedHandler(InventoryManager.ItemChangeInfo itemChangeInfo)
		{
			if (this.unlimitedItemsProvider.IsUnlimitedType(itemChangeInfo.Item))
			{
				InventoryItem item = itemChangeInfo.Item;
				if (this.inventoryManager.GetAmount(item) == 0)
				{
					return;
				}
				this.EnableUnlimitedItem(itemChangeInfo);
				this.inventoryManager.Consume(item, itemChangeInfo.ChangeByAmount, itemChangeInfo.ProductOrCause);
			}
		}

		private void EnableUnlimitedItem(InventoryManager.ItemChangeInfo itemChangeInfo)
		{
			InventoryItem correspondingNonUnlimitedItem = this.unlimitedItemsProvider.GetCorrespondingNonUnlimitedItem(itemChangeInfo.Item);
			string timeStampKey = this.GetTimeStampKey(correspondingNonUnlimitedItem);
			int num = itemChangeInfo.ChangeByAmount;
			int num2 = 0;
			if (this.timingManager.TimeStampExist(timeStampKey))
			{
				num2 = Math.Max(0, this.timingManager.GetTimeLeftInSeconds(timeStampKey));
				this.timingManager.RemoveTimeStampIfItExist(timeStampKey);
				num += num2;
			}
			this.timingManager.CreateTimeStamp(timeStampKey, num);
			this.unlimitedItemsAnalytics.LogUnlimitedItemActivated(itemChangeInfo, num2);
		}

		private string GetTimeStampKey(InventoryItem inventoryItem)
		{
			return inventoryItem + "_TimeStamp";
		}

		public bool HasUnlimitedTime(InventoryItem inventoryItem)
		{
			int timeLeftInSeconds = this.GetTimeLeftInSeconds(inventoryItem);
			return timeLeftInSeconds > 0;
		}

		public bool IsUnlimitedType(InventoryItem inventoryItem)
		{
			return this.unlimitedItemsProvider.IsUnlimitedType(inventoryItem);
		}

		public InventoryItem GetCorrespondingNonUnlimitedItem(InventoryItem unlimitedItem)
		{
			return this.unlimitedItemsProvider.GetCorrespondingNonUnlimitedItem(unlimitedItem);
		}

		public int GetTimeLeftInSeconds(InventoryItem inventoryItem)
		{
			string timeStampKey = this.GetTimeStampKey(inventoryItem);
			return this.timingManager.GetTimeLeftInSeconds(timeStampKey);
		}

		private readonly IInventoryManager inventoryManager;

		private readonly ITimingManager timingManager;

		private readonly IUnlimitedItemsProvider unlimitedItemsProvider;

		private readonly IUnlimitedItemsAnalytics unlimitedItemsAnalytics;
	}
}
