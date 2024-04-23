using System;

namespace TactileModules.PuzzleGames.Lives
{
	public class LivesChangedInfo
	{
		public LivesChangedInfo(int currentLives, int maxLives, int lifeRegenerationTime)
		{
			this.currentLives = currentLives;
			this.maxLives = maxLives;
			this.lifeRegenerationTime = lifeRegenerationTime;
		}

		public int currentLives;

		public int maxLives;

		public int lifeRegenerationTime;
	}
}
