using System;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Views
{
	public interface ISlidesAndLaddersMapView
	{
		void PlaySlideSound(float triggerTimer);

		void PlayLadderSound(float triggerTimer);

		UIViewManager.UIViewState ShowNoMoreLivesView();

		void FadeAndSwitchToMainMapView();
	}
}
