using System;
using JetBrains.Annotations;

namespace TactileModules.PuzzleGames.StarTournament.Views
{
	public class StarTournamentEndedView : UIView
	{
		[UsedImplicitly]
		private void OnOKButtonClicked(UIEvent e)
		{
			base.Close(1);
		}

		[UsedImplicitly]
		private void OnCloseButtonClicked(UIEvent e)
		{
			base.Close(0);
		}
	}
}
