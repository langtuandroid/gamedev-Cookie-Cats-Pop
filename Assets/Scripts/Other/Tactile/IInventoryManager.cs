using System;

namespace Tactile
{
	public interface IInventoryManager
	{
		event Action<InventoryManager.ItemChangeInfo> InventoryChanged;

		event Action<InventoryItem> InventoryReserveChanged;

		void Add(InventoryItem item, int amount, string analyticsTag);

		void Add(InventoryItem item, int amount, string analyticsTag, string purchaseSessionId, string transactionId);

		int GetAmount(InventoryItem item);

		void Consume(InventoryItem item, int amount, string analyticsTag);
	}
}
