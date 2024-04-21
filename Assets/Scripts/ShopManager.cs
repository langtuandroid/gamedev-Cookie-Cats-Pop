using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Fibers;
using JetBrains.Annotations;
using Tactile;

public class ShopManager : ManagerWithMetaData<ShopItemIdentifier, ShopItemMetaData>, IShopManager
{
	private ShopManager(ShopManager.IShopManagerInterface managerInterface, [NotNull] InAppPurchaseManagerBase inAppPurchaseManagerBase, IUserSettings userSettings)
	{
		if (inAppPurchaseManagerBase == null)
		{
			throw new ArgumentNullException("inAppPurchaseManagerBase");
		}
		this.managerInterface = managerInterface;
		this.userSettings = userSettings;
		this.inAppPurchaseManagerBase = inAppPurchaseManagerBase;
		this.inAppPurchaseManagerBase.PurchaseSuccessfulEvent += this.HandleIapSuccess;
	}

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<ShopItem> ShopItemBought;

	public static ShopManager Instance { get; private set; }

	public static ShopManager CreateInstance(ShopManager.IShopManagerInterface managerInterface, InAppPurchaseManagerBase inAppPurchaseManagerBase, IUserSettings userSettings)
	{
		ShopManager.Instance = new ShopManager(managerInterface, inAppPurchaseManagerBase, userSettings);
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
			if (shopItem.FullIAPIdentifier == iapIdentifier)
			{
				return shopItem;
			}
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

	private void HandleIapSuccess(InAppPurchaseManagerBase.PurchaseSuccessfulEventData purchaseEventData)
	{
		ShopItem shopItemFromFullIapIdentifier = ShopManager.Instance.GetShopItemFromFullIapIdentifier(purchaseEventData.ProductId);
		this.HandleShopItemBought(shopItemFromFullIapIdentifier, purchaseEventData);
	}

	public void HandleShopItemBought(ShopItem shopItem, InAppPurchaseManagerBase.PurchaseSuccessfulEventData inAppPurchaseEventDataOrNull)
	{
		if (shopItem != null)
		{
			string transactionId = null;
			string purchaseSessionId = null;
			if (inAppPurchaseEventDataOrNull != null)
			{
				transactionId = inAppPurchaseEventDataOrNull.TransactionId;
				purchaseSessionId = inAppPurchaseEventDataOrNull.PurchaseSessionId;
			}
			foreach (ItemAmount itemAmount in shopItem.Rewards)
			{
				InventoryManager.Instance.Add(itemAmount.ItemId, itemAmount.Amount, shopItem.Type, purchaseSessionId, transactionId);
			}
			if (this.ShopItemBought != null)
			{
				this.ShopItemBought(shopItem);
				this.userSettings.SaveLocalSettings();
				this.userSettings.SyncUserSettings();
			}
		}
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
			this.HandleShopItemBought(shopItem, null);
		}
		onComplete(canAfford);
		yield break;
	}

	private ShopManager.IShopManagerInterface managerInterface;

	private readonly InAppPurchaseManagerBase inAppPurchaseManagerBase;

	private readonly IUserSettings userSettings;

	private bool isSpendingCoins;

	public interface IShopManagerInterface
	{
		IEnumerator TrySpendingCoins(bool canAfford, ShopItem shopItem, object context);

		void LogCoinsSpentToAnalytics(ShopItem shopItem, object context);
	}
}
