using System;
using System.Collections.Generic;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Views
{
	public interface IReward
	{
		void Initialize(List<ItemAmount> rewards);
	}
}
