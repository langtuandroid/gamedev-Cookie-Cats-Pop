using System;
using System.Collections.Generic;
using Tactile;

public class BoosterManagerBase<T> where T : BoosterManagerBase<T>, new()
{
	public static T Instance { get; private set; }

	private IPlayerState PlayerState
	{
		get
		{
			return PuzzleGame.PlayerState;
		}
	}

	public static T CreateInstance()
	{
		BoosterManagerBase<T>.Instance = Activator.CreateInstance<T>();
		return BoosterManagerBase<T>.Instance;
	}

	public int GetNumberOfBoosters(InventoryItem id)
	{
		if (id == null)
		{
			return 0;
		}
		return InventoryManager.Instance.GetAmount(id);
	}

	public bool IsInventoryItemABooster(InventoryItem item)
	{
		return InventoryManager.Instance.GetMetaData<BoosterMetaData>(item);
	}

	public BoosterMetaData GetRandomUnlockedBoosterType()
	{
		Lottery<BoosterMetaData> lottery = new Lottery<BoosterMetaData>();
		int farthestUnlockedLevelIndex = this.PlayerState.FarthestUnlockedLevelIndex;
		foreach (BoosterMetaData boosterMetaData in InventoryManager.Instance.GetAllMetaDataOf<BoosterMetaData>())
		{
			if (farthestUnlockedLevelIndex >= boosterMetaData.UnlockAtLevelIndex)
			{
				lottery.Add(1f, boosterMetaData);
			}
		}
		return lottery.PickRandomItem(false);
	}

	public bool IsBoosterUnlocked(InventoryItem boosterId)
	{
		return this.IsBoosterUnlocked(boosterId, this.PlayerState.FarthestUnlockedLevelIndex);
	}

	public bool IsBoosterUnlocked(InventoryItem boosterId, int atMainLevelIndex)
	{
		return atMainLevelIndex >= InventoryManager.Instance.GetMetaData<BoosterMetaData>(boosterId).UnlockAtLevelIndex;
	}

	public bool IsBoosterUnlockedAtLevelIndex(InventoryItem boosterId, int atMainLevelIndex)
	{
		return atMainLevelIndex == InventoryManager.Instance.GetMetaData<BoosterMetaData>(boosterId).UnlockAtLevelIndex;
	}

	public BoosterMetaData GetBoosterUnlockingAt(int levelIndex)
	{
		foreach (BoosterMetaData boosterMetaData in InventoryManager.Instance.GetAllMetaDataOf<BoosterMetaData>())
		{
			if (boosterMetaData.UnlockAtLevelIndex == levelIndex)
			{
				return boosterMetaData;
			}
		}
		return null;
	}

	public IEnumerable<InventoryItem> GetUnlockedBoosters()
	{
		int farthest = this.PlayerState.FarthestUnlockedLevelIndex;
		foreach (BoosterMetaData info in InventoryManager.Instance.GetAllMetaDataOf<BoosterMetaData>())
		{
			if (info.UnlockAtLevelIndex > 0 && farthest >= info.UnlockAtLevelIndex)
			{
				yield return info.Id;
			}
		}
		yield break;
	}

	public List<InventoryItem> GetUnlockedBoostersFromWantedList(List<InventoryItem> wantedList, int numberOfBoostersToGive)
	{
		List<InventoryItem> list = new List<InventoryItem>();
		int farthestUnlockedLevelIndex = this.PlayerState.FarthestUnlockedLevelIndex;
		List<InventoryItem> list2 = new List<InventoryItem>();
		foreach (InventoryItem item in wantedList)
		{
			bool flag = farthestUnlockedLevelIndex >= InventoryManager.Instance.GetMetaData<BoosterMetaData>(item).UnlockAtLevelIndex;
			if (flag)
			{
				list2.Add(item);
				list.Add(item);
				numberOfBoostersToGive--;
			}
		}
		while (numberOfBoostersToGive > 0)
		{
			InventoryItem item2 = wantedList[0];
			if (list2.Count > 0)
			{
				item2 = list2.GetRandom<InventoryItem>();
			}
			list.Add(item2);
			numberOfBoostersToGive--;
		}
		return list;
	}

	public void MarkBoosterToGetAttention(InventoryItem id, bool mark)
	{
		Dictionary<string, int> needAttention = this.GetState().needAttention;
		if (mark)
		{
			needAttention[id] = 1;
		}
		else
		{
			needAttention.Remove(id);
		}
		this.Save();
	}

	public bool IsBoosterNeedingAttention(InventoryItem id)
	{
		Dictionary<string, int> needAttention = this.GetState().needAttention;
		return needAttention.ContainsKey(id);
	}

	private BoosterManagerPersistableState GetState()
	{
		return UserSettingsManager.Instance.GetSettings<BoosterManagerPersistableState>();
	}

	private void Save()
	{
		UserSettingsManager.Instance.SaveLocalSettings();
	}
}
