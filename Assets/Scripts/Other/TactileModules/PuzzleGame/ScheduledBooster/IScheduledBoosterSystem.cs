using System;
using TactileModules.PuzzleGame.ScheduledBooster.Model;

namespace TactileModules.PuzzleGame.ScheduledBooster
{
	public interface IScheduledBoosterSystem
	{
		ScheduledBoosterHandler Handler { get; }

		ScheduledBoosters ScheduledBoosters { get; }
	}
}
