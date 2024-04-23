using System;
using System.Collections.Generic;

namespace TactileModules.PuzzleGame.PiggyBank.Interfaces
{
	public interface IPiggyBankOfferViewText
	{
		string GetDescriptionLabelText(List<ItemAmount> offerItems, int capacityIncrease);
	}
}
