using System;
using System.Collections.Generic;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Data
{
	public interface ISlidesAndLaddersInventory
	{
		void AddToInventory(List<ItemAmount> items, string analyticsId = "");
	}
}
