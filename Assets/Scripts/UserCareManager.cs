using System;
using System.Collections.Generic;
using Tactile;
using UnityEngine;

public class UserCareManager
{
	public UserCareManager()
	{
		this.itemsToCareAbount = new HashSet<InventoryItem>();
		this.DoCrashAndItemCheck();
	}

	public bool CrashedInLastSession { get; private set; }

	private static IEnumerable<InventoryItem> AllItems
	{
		get
		{
			foreach (InventoryItem item in UserCareManager.userCareItems)
			{
				yield return item;
			}
			foreach (InventoryItem item2 in UserCareManager.boosterItems)
			{
				yield return item2;
			}
			yield break;
		}
	}

	public static UserCareManager Instance { get; private set; }

	public static UserCareManager CreateInstance()
	{
		UserCareManager.Instance = new UserCareManager();
		return UserCareManager.Instance;
	}

	public void DoCrashAndItemCheck()
	{
		if (this.GameCrashedLastSession() && this.TryRestoreUserItems())
		{
			this.CrashedInLastSession = true;
		}
		this.ClearSavedItems();
	}

	public void StartCare(InventoryItem item)
	{
		this.itemsToCareAbount.Add(item);
		if (this.itemsToCareAbount.Count == 1)
		{
			InventoryManager.Instance.InventoryChanged += this.InventoryChanged;
		}
	}

	public void StopCare(InventoryItem item)
	{
		this.itemsToCareAbount.Remove(item);
		if (this.itemsToCareAbount.Count == 0)
		{
			InventoryManager.Instance.InventoryChanged -= this.InventoryChanged;
		}
	}

	public void StartBoosterCare()
	{
		foreach (InventoryItem item in UserCareManager.boosterItems)
		{
			this.StartCare(item);
		}
	}

	public void StopBoosterCare()
	{
		foreach (InventoryItem item in UserCareManager.boosterItems)
		{
			this.StopCare(item);
		}
	}

	private void InventoryChanged(InventoryManager.ItemChangeInfo info)
	{
		if (this.itemsToCareAbount.Contains(info.Item) && info.ChangeByAmount < 0)
		{
			this.ItemUsed(info.Item, Mathf.Abs(info.ChangeByAmount));
		}
	}

	private void ItemUsed(InventoryItem item, int amount)
	{
		int @int = TactilePlayerPrefs.GetInt("UserCareManagerItemUsed" + item);
		TactilePlayerPrefs.SetInt("UserCareManagerItemUsed" + item, @int + amount);
		PlayerPrefs.Save();
	}

	private bool TryRestoreUserItems()
	{
		bool flag = false;
		foreach (InventoryItem item in UserCareManager.AllItems)
		{
			flag |= this.TryRestoreItem(item);
		}
		return flag;
	}

	private bool TryRestoreItem(InventoryItem item)
	{
		int @int = TactilePlayerPrefs.GetInt("UserCareManagerItemUsed" + item, 0);
		if (@int > 0)
		{
			InventoryManager.Instance.Add(item, @int, "UserCareRestore");
		}
		return @int > 0;
	}

	public void ClearSavedItems()
	{
		this.itemsToCareAbount.Clear();
		InventoryManager.Instance.InventoryChanged -= this.InventoryChanged;
		foreach (InventoryItem t in UserCareManager.AllItems)
		{
			TactilePlayerPrefs.SetInt("UserCareManagerItemUsed" + t, 0);
		}
	}

	private bool GameCrashedLastSession()
	{
		return false;
	}

	public void ResetCrashCheck()
	{
		this.CrashedInLastSession = false;
	}

	private const string USERCARE_ITEM = "UserCareManagerItemUsed";

	private static InventoryItem[] boosterItems = new InventoryItem[]
	{
		"BoosterRainbow",
		"BoosterSuperQueue",
		"BoosterFinalPower",
		"BoosterShield",
		"BoosterSuperAim",
		"BoosterExtraMoves"
	};

	private static InventoryItem[] userCareItems = new InventoryItem[]
	{
		"Coin"
	};

	private HashSet<InventoryItem> itemsToCareAbount;
}
