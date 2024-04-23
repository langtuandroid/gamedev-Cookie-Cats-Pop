using System;
using System.Collections;
using System.Collections.Generic;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Views
{
	public interface ISlidesRewardAnimation
	{
		IEnumerator Animate(List<ItemAmount> rewards, MapStreamer mapStreamer, int preSlideIndex, int currentIndex, int chestIndex);

		void PlaySound();
	}
}
