using System;

namespace TactileModules.PuzzleGame.PlayablePostcard.Views
{
	public interface IPostcardMapButtonView
	{
		void ObtainOverlay(UIView view);

		void ReleaseOverlay();

		void SetupOnLifeButtonClicked();

		void ReleaseOnLifeButtonClicked();
	}
}
