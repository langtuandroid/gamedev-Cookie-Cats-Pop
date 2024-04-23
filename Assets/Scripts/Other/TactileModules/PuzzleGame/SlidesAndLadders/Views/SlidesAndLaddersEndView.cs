using System;
using JetBrains.Annotations;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Views
{
	public class SlidesAndLaddersEndView : UIView
	{
		[UsedImplicitly]
		private void Dismiss(UIEvent e)
		{
			base.Close(0);
		}
	}
}
