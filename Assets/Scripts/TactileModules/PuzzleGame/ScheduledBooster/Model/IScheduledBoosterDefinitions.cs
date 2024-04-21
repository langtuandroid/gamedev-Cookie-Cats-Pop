using System;
using TactileModules.PuzzleGame.ScheduledBooster.Data;

namespace TactileModules.PuzzleGame.ScheduledBooster.Model
{
	public interface IScheduledBoosterDefinitions
	{
		ScheduledBoosterDefinition GetDefinition(string scheduledBoosterType);
	}
}
