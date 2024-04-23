using System;
using Tactile;

namespace TactileModules.Inventory
{
	[TactileAnalytics.EventAttribute("unlimitedItemActivated", true)]
	public class UnlimitedItemActivatedEvent : BasicEvent
	{
		public UnlimitedItemActivatedEvent(InventoryManager.ItemChangeInfo info, int previouslyAvailableUnlimitedSeconds)
		{
			this.ItemType = info.Item.ToString();
			this.TimeUnlimited = info.ChangeByAmount;
			this.CurrentlyActive = (previouslyAvailableUnlimitedSeconds > 0);
			this.TotalTimeUnlimited = info.ChangeByAmount + previouslyAvailableUnlimitedSeconds;
			this.ShopItemProduct = info.ProductOrCause;
		}

		private TactileAnalytics.RequiredParam<string> ItemType { get; set; }

		private TactileAnalytics.RequiredParam<int> TimeUnlimited { get; set; }

		private TactileAnalytics.RequiredParam<bool> CurrentlyActive { get; set; }

		private TactileAnalytics.RequiredParam<int> TotalTimeUnlimited { get; set; }

		private TactileAnalytics.RequiredParam<string> ShopItemProduct { get; set; }
	}
}
