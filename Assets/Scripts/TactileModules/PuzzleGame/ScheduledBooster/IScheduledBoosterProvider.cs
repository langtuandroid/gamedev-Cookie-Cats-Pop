using System;

namespace TactileModules.PuzzleGame.ScheduledBooster
{
	public interface IScheduledBoosterProvider
	{
		bool IsTutorialLevel(ILevelProxy levelProxy);
	}
}
