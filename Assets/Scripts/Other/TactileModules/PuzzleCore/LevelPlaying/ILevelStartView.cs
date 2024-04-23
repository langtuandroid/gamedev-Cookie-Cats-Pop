using System;
using System.Collections.Generic;

namespace TactileModules.PuzzleCore.LevelPlaying
{
	public interface ILevelStartView : IUIView
	{
		event Action<List<SelectedBooster>> PlayButtonClicked;

		event Action DismissButtonClicked;

		event Action<int> Developer_CheatCompleteClicked;

		void Initialize(LevelProxy levelProxy, bool isRetrying, IPlayFlowContext context);
	}
}
