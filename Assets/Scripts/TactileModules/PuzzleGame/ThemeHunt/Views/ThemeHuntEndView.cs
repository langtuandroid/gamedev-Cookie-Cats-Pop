using System;

namespace TactileModules.PuzzleGame.ThemeHunt.Views
{
	public class ThemeHuntEndView : UIView
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
