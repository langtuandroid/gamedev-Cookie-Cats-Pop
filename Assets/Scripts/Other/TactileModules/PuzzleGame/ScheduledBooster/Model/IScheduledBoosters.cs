using System;

namespace TactileModules.PuzzleGame.ScheduledBooster.Model
{
	public interface IScheduledBoosters
	{
		void AddBooster(IScheduledBooster booster);

		void RemoveBooster(string boosterType);

		IScheduledBooster GetBooster(string boosterType);
	}
}
