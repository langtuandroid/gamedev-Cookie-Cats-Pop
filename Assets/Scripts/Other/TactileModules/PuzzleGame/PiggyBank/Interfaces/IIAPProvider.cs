using System;
using System.Collections.Generic;

namespace TactileModules.PuzzleGame.PiggyBank.Interfaces
{
	public interface IIAPProvider
	{
		string GetFormattedOpenPrice();

		string GetFormattedOfferPrice();

		InAppProduct GetBuyOpenInAppProduct();

		InAppProduct GetOfferInAppProduct();

		List<ItemAmount> GetOfferItems();
	}
}
