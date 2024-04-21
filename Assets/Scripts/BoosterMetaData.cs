using System;
using UnityEngine;

public class BoosterMetaData : InventoryItemMetaData
{
	public int UnlockAtLevelIndex
	{
		get
		{
			return this.unlockAtLevelIndex;
		}
	}

	public UnityEngine.Object UnlockPrefab
	{
		get
		{
			return this.unlockPrefab;
		}
	}

	public bool PreGame
	{
		get
		{
			return this.preGame;
		}
	}

	public string InGameShopItemIdentifier
	{
		get
		{
			return this.inGameShopItemIdentifier;
		}
	}

	public string PreGameShopItemIdentifier
	{
		get
		{
			return this.preGameShopItemIdentifier;
		}
	}

	[SerializeField]
	protected int unlockAtLevelIndex;

	[SerializeField]
	protected UnityEngine.Object unlockPrefab;

	[SerializeField]
	protected bool preGame;

	[SerializeField]
	protected ShopItemIdentifier preGameShopItemIdentifier;

	[SerializeField]
	protected ShopItemIdentifier inGameShopItemIdentifier;
}
