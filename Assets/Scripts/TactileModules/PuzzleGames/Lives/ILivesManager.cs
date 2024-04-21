using System;

namespace TactileModules.PuzzleGames.Lives
{
	public interface ILivesManager
	{
		event Action<LivesChangedInfo> LivesChanged;

		int LifeRegenerationMaxCount { get; }

		bool HasUnlimitedLives();

		bool IsOutOfLives();

		void UseLifeIfNotUnlimited(string analyticsTag);

		int GetTimeLeftForInfiniteLives();

		int GetRegenerationTimeLeft();
	}
}
