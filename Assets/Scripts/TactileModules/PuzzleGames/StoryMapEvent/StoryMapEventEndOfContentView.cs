using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	public class StoryMapEventEndOfContentView : UIView
	{
		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action ClickedContinue;



		[UsedImplicitly]
		private void Continue(UIEvent e)
		{
			this.ClickedContinue();
		}
	}
}
