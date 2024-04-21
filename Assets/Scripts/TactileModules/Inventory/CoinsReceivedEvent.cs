using System;
using Tactile;

namespace TactileModules.Inventory
{
	[TactileAnalytics.EventAttribute("coinsReceived", true)]
	public class CoinsReceivedEvent : BasicItemsChangedEvent
	{
		public CoinsReceivedEvent(InventoryManager.ItemChangeInfo info) : base(info)
		{
			this.TransactionId = info.TransactionId;
			this.PurchaseSessionId = info.PurchaseSessionId;
			this.AmountReceived = info.ChangeByAmount;
		}

		private TactileAnalytics.RequiredParam<int> AmountReceived { get; set; }

		private TactileAnalytics.OptionalParam<string> TransactionId { get; set; }

		private TactileAnalytics.OptionalParam<string> PurchaseSessionId { get; set; }
	}
}
