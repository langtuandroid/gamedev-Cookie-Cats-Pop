using System;

namespace TactileModules.PuzzleGame.ThemeHunt.Views
{
	public class ThemeHuntIntroView : UIView
	{
		protected override void ViewLoad(object[] parameters)
		{
		}

		private void CloseClicked(UIEvent e)
		{
			base.Close(0);
		}
	}
}
