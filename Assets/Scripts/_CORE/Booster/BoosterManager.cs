using System;
using System.Collections;
using System.Collections.Generic;
using Tactile;
using TactileModules.Foundation;

public class BoosterManager : BoosterManagerBase<BoosterManager>, MapPopupManager.IMapPopup
{
	public BoosterManager()
	{
		MapPopupManager.Instance.RegisterPopupObject(this);
	}

	private ConfigurationManager ConfigurationManager
	{
		get
		{
			return ManagerRepository.Get<ConfigurationManager>();
		}
	}

	void MapPopupManager.IMapPopup.TryShowPopup(int unlockedLevelIndex, MapPopupManager.PopupFlow popupFlow)
	{
		BoosterMetaData boosterUnlockingAt = base.GetBoosterUnlockingAt(unlockedLevelIndex);
		if (boosterUnlockingAt != null)
		{
			popupFlow.AddPopup(this.ShowPopup(boosterUnlockingAt.Id));
		}
	}

	private IEnumerator ShowPopup(InventoryItem boosterId)
	{
		CPBoosterMetaData boosterMetaData = InventoryManager.Instance.GetMetaData<CPBoosterMetaData>(boosterId);
		int unlockAmount = this.ConfigurationManager.GetConfig<BoosterConfig>().GetUnlockAmount(boosterMetaData.Id);
		if (unlockAmount <= 0)
		{
			yield break;
		}
		base.MarkBoosterToGetAttention(boosterMetaData.Id, true);
		bool isComplete = false;
		UIViewManager.Instance.ShowView<BoosterUnlockedView>(new object[]
		{
			new BoosterUnlockedView.BoosterUnlockedViewParameters
			{
				itemUnlocked = boosterMetaData.Id,
				rewards = new List<ItemAmount>
				{
					new ItemAmount
					{
						ItemId = boosterMetaData.Id,
						Amount = unlockAmount
					}
				},
				Completed = delegate()
				{
					isComplete = true;
				}
			}
		});
		InventoryManager.Instance.Add(boosterMetaData.Id, unlockAmount, "booster unlocked");
		while (!isComplete)
		{
			yield return null;
		}
		yield break;
	}

	public bool ShouldUseSuperAimMonocular
	{
		get
		{
			return this.ConfigurationManager.GetConfig<BoosterConfig>().UseSuperAimMonocular;
		}
	}

	public List<InventoryItem> AutoSelectedBoosters { get; set; }
}
