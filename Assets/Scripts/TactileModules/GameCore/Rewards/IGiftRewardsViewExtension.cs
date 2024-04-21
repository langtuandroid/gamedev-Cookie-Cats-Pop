using System;
using System.Collections;

namespace TactileModules.GameCore.Rewards
{
	public interface IGiftRewardsViewExtension
	{
		IEnumerator InitializeVisualGift(int giftType);

		IEnumerator AnimateOpenGift();

		IEnumerator AnimateRewardsIn();

		IEnumerator AnimateRewardsOut();

		void HideButtons();
	}
}
