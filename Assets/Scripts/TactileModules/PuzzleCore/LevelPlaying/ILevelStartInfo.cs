using System;
using System.Collections.Generic;

namespace TactileModules.PuzzleCore.LevelPlaying
{
	public interface ILevelStartInfo
	{
		int PregameBoostersUsed { get; }

		List<SelectedBooster> SelectedPregameBoosters { get; }

		bool DidStart { get; set; }
	}
}
