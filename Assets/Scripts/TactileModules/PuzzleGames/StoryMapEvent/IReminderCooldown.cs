using System;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	public interface IReminderCooldown
	{
		bool IsTimeToShow();

		void Reset();
	}
}
