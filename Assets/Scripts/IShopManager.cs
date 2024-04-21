using System;

public interface IShopManager
{
	void TrySpendCoins(ShopItem shopItem, object context, Action<bool> onComplete);
}
