using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Fibers;
using JetBrains.Annotations;
using Tactile;

public class ShopManager : ManagerWithMetaData<ShopItemIdentifier, ShopItemMetaData>, IShopManager
{
	private ShopManager(ShopManager.IShopManagerInterface managerInterface,  IUserSettings userSettings)
	{
		this.managerInterface = managerInterface;
		this.userSettings = userSettings;
	}

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<ShopItem> ShopItemBought;

	public static ShopManager Instance { get; private set; }

	public static ShopManager CreateInstance(ShopManager.IShopManagerInterface managerInterface,  IUserSettings userSettings)
	{
		ShopManager.Instance = new ShopManager(managerInterface, userSettings);
		return ShopManager.Instance;
	}

	protected override string MetaDataAssetFolder
	{
		get
		{
			return "Assets/[ShopItems]/Resources/ShopItemMetaData";
		}
	}

	public ShopItem GetShopItem(ShopItemIdentifier itemType)
	{
		foreach (ShopItem shopItem in ShopManager.Config.ShopItems)
		{
			if (shopItem.Type == itemType)
			{
				return shopItem;
			}
		}
		return null;
	}

	public ShopItem GetShopItemFromFullIapIdentifier(string iapIdentifier)
	{
		foreach (ShopItem shopItem in ShopManager.Config.ShopItems)
		{
			
		}
		return null;
	}

	public static ShopConfig Config
	{
		get
		{
			return ConfigurationManager.Get<ShopConfig>();
		}
	}

	public ShopItemMetaData GetMetaDataForShopItem(ShopItem shopItem)
	{
		return base.GetMetaData<ShopItemMetaData>(shopItem.Type);
	}

	public IEnumerable<ShopItem> ShopItemsWithIAPForItems(InventoryItem itemId)
	{
		foreach (ShopItem item in ShopManager.Config.ShopItems)
		{
			if (!string.IsNullOrEmpty(item.PartialIAPIdentifier) && item.Amount(itemId) > 0)
			{
				yield return item;
			}
		}
		yield break;
	}

	public IEnumerable<ShopItem> ShopItemsWithCustomTag(string tag)
	{
		foreach (ShopItem item in ShopManager.Config.ShopItems)
		{
			if (item.CustomTag.Contains(tag))
			{
				yield return item;
			}
		}
		yield break;
	}


	

	public void TrySpendCoins(ShopItemIdentifier shopItemIdentifier, object context, Action<bool> onComplete)
	{
		ShopItem shopItem = this.GetShopItem(shopItemIdentifier);
		this.TrySpendCoins(shopItem, context, onComplete);
	}

	public void TrySpendCoins(ShopItem shopItem, object context, Action<bool> onComplete)
	{
		if (this.isSpendingCoins)
		{
			return;
		}
		FiberCtrl.Pool.Run(this.TrySpendCoinsCr(shopItem, context, onComplete), false);
	}

	private IEnumerator TrySpendCoinsCr(ShopItem shopItem, object context, Action<bool> onComplete)
	{
		this.isSpendingCoins = true;
		yield return new Fiber.OnExit(delegate()
		{
			this.isSpendingCoins = false;
		});
		bool canAfford = InventoryManager.Instance.Coins >= shopItem.CurrencyPrice;
		if (!canAfford)
		{
			yield return this.managerInterface.TrySpendingCoins(canAfford, shopItem, context);
			canAfford = (InventoryManager.Instance.Coins >= shopItem.CurrencyPrice);
		}
		if (canAfford)
		{
			InventoryManager.Instance.ConsumeCoins(shopItem.CurrencyPrice, shopItem.Type);
			UserSettingsManager.Instance.SaveLocalSettings();
			yield return this.managerInterface.TrySpendingCoins(canAfford, shopItem, context);
			this.managerInterface.LogCoinsSpentToAnalytics(shopItem, context);
		}
		onComplete(canAfford);
		yield break;
	}

	private ShopManager.IShopManagerInterface managerInterface;

	private readonly IUserSettings userSettings;

	private bool isSpendingCoins;

	public interface IShopManagerInterface
	{
		IEnumerator TrySpendingCoins(bool canAfford, ShopItem shopItem, object context);

		void LogCoinsSpentToAnalytics(ShopItem shopItem, object context);
	}
}
