using System;
using System.Collections.Generic;
using Tactile;
using TactileModules.PuzzleGame.PiggyBank;
using TactileModules.PuzzleGame.PiggyBank.Interfaces;

public class PiggyBankIAPProvider : IIAPProvider
{
	public PiggyBankIAPProvider(ShopManager shopManager, InAppPurchaseManager inAppPurchaseManager, IDataProvider<PiggyBankConfig> config)
	{
		this.shopManager = shopManager;
		this.inAppPurchaseManager = inAppPurchaseManager;
		this.config = config;
	}

	public string GetFormattedOpenPrice()
	{
		ShopItem shopItem = this.shopManager.GetShopItem("PiggyBankOpen");
		return shopItem.FormattedPricePreferIAP(this.inAppPurchaseManager);
	}

	public string GetFormattedOfferPrice()
	{
		ShopItem shopItem = this.shopManager.GetShopItem("PiggyBankOffer");
		return shopItem.FormattedPricePreferIAP(this.inAppPurchaseManager);
	}

	public InAppProduct GetBuyOpenInAppProduct()
	{
		ShopItem shopItem = this.shopManager.GetShopItem("PiggyBankOpen");
		return this.inAppPurchaseManager.GetProductForIdentifier(shopItem.FullIAPIdentifier);
	}

	public InAppProduct GetOfferInAppProduct()
	{
		ShopItem shopItem = this.shopManager.GetShopItem("PiggyBankOffer");
		return this.inAppPurchaseManager.GetProductForIdentifier(shopItem.FullIAPIdentifier);
	}

	public List<ItemAmount> GetOfferItems()
	{
		return this.config.Get().OfferItems;
	}

	private readonly ShopManager shopManager;

	private readonly InAppPurchaseManager inAppPurchaseManager;

	private readonly IDataProvider<PiggyBankConfig> config;
}
