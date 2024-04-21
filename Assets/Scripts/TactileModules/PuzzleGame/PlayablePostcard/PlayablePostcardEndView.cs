using System;
using JetBrains.Annotations;

namespace TactileModules.PuzzleGame.PlayablePostcard
{
	public class PlayablePostcardEndView : UIView
	{
		[UsedImplicitly]
		private void Dismiss(UIEvent e)
		{
			base.Close(0);
		}
	}
}
