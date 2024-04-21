using System;
using Tactile;

namespace TactileModules.Inventory
{
	[TactileAnalytics.EventAttribute("itemsUsed", true)]
	public class ItemsUsedEvent : ItemsChangedEvent
	{
		public ItemsUsedEvent(InventoryManager.ItemChangeInfo info) : base(info)
		{
			this.AmountUsed = Math.Abs(info.ChangeByAmount);
		}

		private TactileAnalytics.RequiredParam<int> AmountUsed { get; set; }
	}
}
