using System;

namespace TactileModules.PuzzleCore.LevelPlaying
{
	public interface IPlayFlowContextCheats
	{
		void CheatCompleteLevel(LevelProxy levelProxy, object accomplishmentData);
	}
}
