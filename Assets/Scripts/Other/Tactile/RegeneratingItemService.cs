using System;
using UnityEngine;

namespace Tactile
{
	public class RegeneratingItemService
	{
		public RegeneratingItemService(TimeStampManager timeStampManager, InventoryItem itemToRegenerate, int maxItems, int itemRegenerationTime)
		{
			this.timeStampId = "RegeneratingItemService_" + itemToRegenerate;
			timeStampManager.TimeDone += this.OnTimerDone;
			this.timeStampManager = timeStampManager;
			this.itemToRegenerate = itemToRegenerate;
			this.maxItems = maxItems;
			this.itemRegenerationTime = itemRegenerationTime;
			InventoryManager.Instance.InventoryChanged += this.HandleInventoryChanged;
			this.StartOrStopTimestampsAsNeeded();
		}

		public int maxItems { get; private set; }

		private void OnTimerDone(string timerId)
		{
			InventoryManager.Instance.InventoryChanged -= this.HandleInventoryChanged;
			if (timerId == this.timeStampId)
			{
				InventoryManager.Instance.Add(this.itemToRegenerate, 1, null);
				InventoryManager.Instance.Save();
				if (this.Amount < this.maxItems)
				{
					int timePassedInSeconds = this.timeStampManager.GetTimePassedInSeconds(this.timeStampId);
					int timeDuration = this.timeStampManager.GetTimeDuration(this.timeStampId);
					int num = timePassedInSeconds - timeDuration;
					int num2 = num / this.itemRegenerationTime;
					num2 = Mathf.Clamp(num2, 0, this.maxItems - this.Amount);
					InventoryManager.Instance.Add(this.itemToRegenerate, num2, null);
					InventoryManager.Instance.Save();
					if (this.Amount < this.maxItems)
					{
						num -= num2 * this.itemRegenerationTime;
						this.timeStampManager.CreateTimeStamp(this.timeStampId, this.itemRegenerationTime - num);
					}
				}
			}
			InventoryManager.Instance.InventoryChanged += this.HandleInventoryChanged;
		}

		private int Amount
		{
			get
			{
				return InventoryManager.Instance.GetAmount(this.itemToRegenerate);
			}
		}

		public bool LivesAreRegenerating()
		{
			return this.Amount < this.maxItems;
		}

		public int GetSecondsLeftForRegeneration()
		{
			return this.timeStampManager.GetTimeLeftInSeconds(this.timeStampId);
		}

		public void ChangeMaxItems(int newMaxItems)
		{
			this.maxItems = newMaxItems;
			this.StartOrStopTimestampsAsNeeded();
		}

		private void HandleInventoryChanged(InventoryManager.ItemChangeInfo info)
		{
			if (info.Item == this.itemToRegenerate)
			{
				this.StartOrStopTimestampsAsNeeded();
			}
		}

		public void StartOrStopTimestampsAsNeeded()
		{
			if (this.Amount >= this.maxItems)
			{
				this.timeStampManager.RemoveTimeStampIfItExist(this.timeStampId);
			}
			else if (this.Amount < this.maxItems && !this.timeStampManager.TimeStampExist(this.timeStampId))
			{
				this.timeStampManager.CreateTimeStamp(this.timeStampId, this.itemRegenerationTime);
			}
		}

		private const string TIMESTAMP_PREFIX = "RegeneratingItemService_";

		private readonly TimeStampManager timeStampManager;

		private string timeStampId;

		private InventoryItem itemToRegenerate;

		private int itemRegenerationTime;
	}
}
