using System;
using System.Collections.Generic;

namespace Shared.PiggyBank.Module.Interfaces
{
	public interface IItemsProvider
	{
		void GiveContentToPlayer(int content);

		void GiveOfferItemsToPlayer(List<ItemAmount> items);
	}
}
