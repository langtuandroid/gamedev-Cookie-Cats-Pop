using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TactileModules.PuzzleCore.LevelPlaying
{
	public abstract class LevelStartView : UIView, ILevelStartView, IUIView
	{
		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<List<SelectedBooster>> PlayButtonClicked;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action DismissButtonClicked;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<int> Developer_CheatCompleteClicked;



		public abstract void Initialize(LevelProxy levelProxy, bool isRetrying, IPlayFlowContext context);

		protected void InvokePlayButtonClicked(List<SelectedBooster> selectedBoosters)
		{
			this.PlayButtonClicked(selectedBoosters);
		}

		protected void InvokeDismissButtonClicked()
		{
			this.DismissButtonClicked();
		}

		protected void Developer_InvokeCheatCompleteClicked(int numStars)
		{
			this.Developer_CheatCompleteClicked(numStars);
		}
	}
}
