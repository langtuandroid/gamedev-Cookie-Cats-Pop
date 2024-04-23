using System;
using System.Collections;
using System.Collections.Generic;

namespace TactileModules.PuzzleGame.PiggyBank.Interfaces
{
	public interface IPiggyBankOfferViewExtension
	{
		void Initialize(List<ItemAmount> offerItems);

		IEnumerator AnimateItemsToInventory(List<ItemAmount> items);
	}
}
