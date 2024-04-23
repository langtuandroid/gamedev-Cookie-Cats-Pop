using System;
using Tactile;

namespace TactileModules.Inventory
{
	public class BasicItemsChangedEvent : BasicEvent
	{
		public BasicItemsChangedEvent(InventoryManager.ItemChangeInfo info)
		{
			this.TotalAmount = InventoryManager.Instance.GetAmount(info.Item);
			this.ShopItemProduct = info.ProductOrCause;
		}

		private TactileAnalytics.RequiredParam<int> TotalAmount { get; set; }

		private TactileAnalytics.RequiredParam<string> ShopItemProduct { get; set; }
	}
}
