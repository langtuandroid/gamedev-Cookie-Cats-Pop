using System;
using UnityEngine;

[Serializable]
public struct ShopItemIdentifier
{
	public ShopItemIdentifier(string s)
	{
		this.value = s;
	}

	public static implicit operator ShopItemIdentifier(string val)
	{
		return new ShopItemIdentifier(val)
		{
			value = val
		};
	}

	public static implicit operator string(ShopItemIdentifier t)
	{
		return t.value;
	}

	public static bool operator ==(ShopItemIdentifier a, ShopItemIdentifier b)
	{
		return object.ReferenceEquals(a, b) || ((object)a != (object)null && (object)b != (object)null && a.value == b.value);
	}

	public override bool Equals(object obj)
	{
		return obj is ShopItemIdentifier && this.value == ((ShopItemIdentifier)obj).value;
	}

	public static bool operator !=(ShopItemIdentifier a, ShopItemIdentifier b)
	{
		return !(a == b);
	}

	public override int GetHashCode()
	{
		return this.value.GetHashCode();
	}

	public override string ToString()
	{
		return this.value;
	}

	public const string Continue = "ShopItemContinue";

	public const string ContinueSpecial = "ShopItemContinueSpecial";

	public const string ContinueAfterDeath = "ContinueAfterDeath";

	public const string ContinueAfterDeathWithBooster = "ContinueAfterDeathWithBooster";

	public const string CoinPack1 = "ShopItemCoinPack1";

	public const string CoinPack2 = "ShopItemCoinPack2";

	public const string CoinPack3 = "ShopItemCoinPack3";

	public const string CoinPack4 = "ShopItemCoinPack4";

	public const string CoinPack5 = "ShopItemCoinPack5";

	public const string ExtraLives = "ShopItemExtraLives";

	public const string UnlockGate = "ShopItemUnlockGate";

	public const string SkipWaitDailyQuest = "ShopItemSkipWaitDailyQuest";

	public const string SkipPlayDailyQuest = "ShopItemSkipPlayDailyQuest";

	public const string Freebie = "ShopItemFreebie";

	public const string WelcomePack = "ShopItemWelcomePack";

	public const string SpecialOffer_1_1 = "ShopItemSpecialOffer_1_1";

	public const string RecommendedPack = "ShopItemRecommendedPack";

	public const string AdFreePeriodEndPack = "ShopItemAdFreePeriodEndPack";

	public const string BoosterRainbowBubble = "ShopItemRainbow";

	public const string BoosterSuperQueue = "ShopItemSuperQueue";

	public const string BoosterFinalPower = "ShopItemFinalPower";

	public const string BoosterShield = "ShopItemShield";

	public const string BoosterSuperAim = "ShopItemSuperAim";

	public const string BoosterExtraMoves = "ShopItemExtraMoves";

	public const string BoosterPreGameShield = "ShopItemPreGameShield";

	public const string BoosterPreGameSuperAim = "ShopItemPreGameSuperAim";

	public const string BoosterPreGameSuperQueue = "ShopItemPreGameSuperQueue";

	public const string BoosterPreGameShield_Unlimited = "ShopItemPreGameShield_Unlimited";

	public const string BoosterPreGameSuperAim_Unlimited = "ShopItemPreGameSuperAim_Unlimited";

	public const string BoosterPreGameSuperQueue_Unlimited = "ShopItemPreGameSuperQueue_Unlimited";

	public const string FillBlue = "ShopItemFillBlue";

	public const string FillRed = "ShopItemFillRed";

	public const string FillGreen = "ShopItemFillGreen";

	public const string FillYellow = "ShopItemFillYellow";

	public const string Vip = "ShopItemVip";

	public const string PiggyBankOpen = "PiggyBankOpen";

	public const string PiggyBankOffer = "PiggyBankOffer";

	[SerializeField]
	private string value;
}
