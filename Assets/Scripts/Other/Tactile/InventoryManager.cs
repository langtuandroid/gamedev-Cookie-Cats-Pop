using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Tactile
{
	public class InventoryManager : ManagerWithMetaData<InventoryItem, InventoryItemMetaData>, IInventoryManager
	{
		private InventoryManager()
		{
		}

		public static InventoryManager Instance { get; private set; }

		public int Lives
		{
			get
			{
				return 4;
				//return this.GetAmount("Life"); //TODO unkoment
			}
		}

		public int Coins
		{
			get
			{
				return 1000000; //TODO unkoment
				//return this.GetAmount("Coin");
			}
		}

		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<InventoryManager.ItemChangeInfo> InventoryChanged = delegate (InventoryManager.ItemChangeInfo A_0)
        {
        };



        ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public event Action<InventoryItem> InventoryReserveChanged = delegate (InventoryItem A_0)
        {
        };



        public static InventoryManager CreateInstance()
		{
			InventoryManager.Instance = new InventoryManager();
			return InventoryManager.Instance;
		}

		protected override string MetaDataAssetFolder
		{
			get
			{
				return "Assets/[Inventory]/Resources/InventoryMetaData";
			}
		}

		public int GetAmount(InventoryItem item)
		{
			InventoryManager.PersistableState state = this.GetState();
			int num = 0;
			state.items.TryGetValue(item, out num);
			return num - this.reservedAmounts.GetValueOrDefault(item);
		}

		public void SetAmount(InventoryItem item, int newAmount, string analyticsTag)
		{
			InventoryManager.PersistableState state = this.GetState();
			int num = 0;
			state.items.TryGetValue(item, out num);
			this.ChangeAmount(item, newAmount - num, analyticsTag);
		}

		public void Add(InventoryItem item, int amount, string analyticsTag)
		{
			this.ChangeAmount(item, amount, analyticsTag);
		}

		public void Add(InventoryItem item, int amount, string analyticsTag, string purchaseSessionId, string transactionId)
		{
			this.ChangeAmount(new InventoryManager.ItemChangeInfo(item, amount, analyticsTag, purchaseSessionId, transactionId));
		}

		public void Reserve(InventoryItem item, int amount)
		{
			this.reservedAmounts.IncreaseSafely(item, amount);
			this.InventoryReserveChanged(item);
		}

		public void Unreserve(InventoryItem item, int amount)
		{
			this.reservedAmounts.IncreaseSafely(item, -amount);
			this.InventoryReserveChanged(item);
		}

		public void ClearReservations()
		{
			foreach (KeyValuePair<InventoryItem, int> keyValuePair in this.reservedAmounts)
			{
				this.InventoryReserveChanged(keyValuePair.Key);
			}
			this.reservedAmounts.Clear();
		}

		public void Consume(InventoryItem item, int amount, string analyticsTag)
		{
			this.ChangeAmount(item, -amount, analyticsTag);
		}

		public void AddCoins(int amount, string analyticsTag)
		{
			this.ChangeAmount("Coin", amount, analyticsTag);
		}

		public void ConsumeCoins(int amount, string analyticsTag)
		{
			this.ChangeAmount("Coin", -amount, analyticsTag);
		}

		private void ChangeAmount(InventoryItem item, int amount, string analyticsTag)
		{
			this.ChangeAmount(new InventoryManager.ItemChangeInfo(item, amount, analyticsTag, null, null));
		}

		private void ChangeAmount(InventoryManager.ItemChangeInfo info)
		{
			if (info.ChangeByAmount == 0)
			{
				return;
			}
			InventoryManager.PersistableState state = this.GetState();
			int num = 0;
			state.items.TryGetValue(info.Item, out num);
			if (num + info.ChangeByAmount < 0)
			{
				info = new InventoryManager.ItemChangeInfo(info.Item, -num, info.ProductOrCause, info.PurchaseSessionId, info.TransactionId);
			}
			state.items[info.Item] = num + info.ChangeByAmount;
			if (info.ChangeByAmount < 0 && this.reservedAmounts.GetValueOrDefault(info.Item) > 0)
			{
				Dictionary<InventoryItem, int> dictionary;
				InventoryItem item;
				(dictionary = this.reservedAmounts)[item = info.Item] = dictionary[item] + info.ChangeByAmount;
			}
			this.InventoryChanged(info);
			this.Save();
		}

		private InventoryManager.PersistableState GetState()
		{
			return UserSettingsManager.Get<InventoryManager.PersistableState>();
		}

		public void Save()
		{
			UserSettingsManager.Instance.SaveLocalSettings();
		}

		private Dictionary<InventoryItem, int> reservedAmounts = new Dictionary<InventoryItem, int>();

		[SettingsProvider("in", false, new Type[]
		{

		})]
		public class PersistableState : IPersistableState<InventoryManager.PersistableState>, IPersistableState
		{
			public PersistableState()
			{
				this.items = new Dictionary<string, int>();
			}

			[JsonSerializable("it", typeof(int))]
			public Dictionary<string, int> items { get; set; }

			public void MergeFromOther(InventoryManager.PersistableState newest, InventoryManager.PersistableState last)
			{
				foreach (KeyValuePair<string, int> keyValuePair in newest.items)
				{
					int num = newest.items[keyValuePair.Key];
					int num2 = 0;
					this.items.TryGetValue(keyValuePair.Key, out num2);
					int num3 = num2;
					if (this.itemsNotUsingDifMerge.ContainsKey(keyValuePair.Key))
					{
						num3 = Mathf.Max(num3, num);
					}
					else if (last != null)
					{
						int num4;
						last.items.TryGetValue(keyValuePair.Key, out num4);
						int num5 = num - num4;
						num3 += num5;
					}
					else
					{
						num3 = Mathf.Max(num3, num);
					}
					num3 = Mathf.Max(num3, 0);
					int changeByAmount = num3 - num2;
					this.items[keyValuePair.Key] = num3;
					InventoryManager.Instance.InventoryChanged(new InventoryManager.ItemChangeInfo(keyValuePair.Key, changeByAmount, null, null, null));
				}
			}

			private readonly Dictionary<string, bool> itemsNotUsingDifMerge = new Dictionary<string, bool>
			{
				{
					"Life",
					true
				}
			};
		}

		public class ItemChangeInfo
		{
			public ItemChangeInfo(InventoryItem item, int changeByAmount, string productOrCause = null, string purchaseSessionId = null, string transactionId = null)
			{
				this.Item = item;
				this.ChangeByAmount = changeByAmount;
				this.ProductOrCause = productOrCause;
				this.PurchaseSessionId = purchaseSessionId;
				this.TransactionId = transactionId;
			}

			public InventoryItem Item { get; private set; }

			public int ChangeByAmount { get; private set; }

			public string ProductOrCause { get; private set; }

			public string PurchaseSessionId { get; private set; }

			public string TransactionId { get; private set; }
		}
	}
}
