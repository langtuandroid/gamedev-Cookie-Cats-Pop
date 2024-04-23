using System;

namespace TactileModules.PuzzleGames.LevelDash.Views
{
	public class LevelDashEndedView : UIView
	{
		private void OnOKButtonClicked(UIEvent e)
		{
			base.Close(1);
		}

		private void OnCloseButtonClicked(UIEvent e)
		{
			base.Close(0);
		}
	}
}
