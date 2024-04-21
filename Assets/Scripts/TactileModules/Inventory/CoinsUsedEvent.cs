using System;
using Tactile;

namespace TactileModules.Inventory
{
	[TactileAnalytics.EventAttribute("coinsUsed", true)]
	public class CoinsUsedEvent : BasicItemsChangedEvent
	{
		public CoinsUsedEvent(InventoryManager.ItemChangeInfo info) : base(info)
		{
			this.AmountUsed = Math.Abs(info.ChangeByAmount);
		}

		private TactileAnalytics.RequiredParam<int> AmountUsed { get; set; }
	}
}
