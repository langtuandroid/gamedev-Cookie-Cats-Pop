using System;
using System.Collections;
using System.Collections.Generic;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Views
{
	public interface IRewardAnimation
	{
		IEnumerator Animate();

		void Initialize(List<ItemAmount> rewards);

		void PlaySound();
	}
}
