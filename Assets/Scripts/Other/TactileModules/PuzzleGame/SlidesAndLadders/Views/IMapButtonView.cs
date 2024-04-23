using System;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Views
{
	public interface IMapButtonView
	{
		void ObtainOverlay(UIView view);

		void ReleaseOverlay();

		void SetupOnLifeButtonClicked();

		void ReleaseOnLifeButtonClicked();

		void FadeAndSwitchToMainMapView();
	}
}
