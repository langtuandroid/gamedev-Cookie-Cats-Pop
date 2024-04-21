using System;
using System.Collections;
using Spine;
using TactileModules.PuzzleGame.PiggyBank.UI;

namespace TactileModules.PuzzleGame.PiggyBank.Interfaces
{
	public interface IPiggyBankViewExtension
	{
		float YOffSetFreeOpeningState { get; }

		void Initialize(PiggyBankView view);

		void PauseRefreshingCoins(bool pause);

		IEnumerator PlayOpeningAnimation();

		IEnumerator AnimateCoinAmount(int numCoins, float duration);

		void PlayOpeningAvailableIdleAnimation();

		void PlayLockedJumpingAnimation();

		TrackEntry PlayEmptyBankAnimation();
	}
}
