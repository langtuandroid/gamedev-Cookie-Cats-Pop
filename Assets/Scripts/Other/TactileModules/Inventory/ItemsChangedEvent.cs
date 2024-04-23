using System;
using Tactile;

namespace TactileModules.Inventory
{
	public class ItemsChangedEvent : BasicItemsChangedEvent
	{
		public ItemsChangedEvent(InventoryManager.ItemChangeInfo info) : base(info)
		{
			this.ItemType = info.Item.ToString();
		}

		private TactileAnalytics.RequiredParam<string> ItemType { get; set; }
	}
}
