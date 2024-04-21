using System;

namespace TactileModules.PuzzleGames.StarTournament
{
	public class CurrentActiveLevel
	{
		public int StarsLevelStart { get; private set; }

		public int StarsToShow { get; private set; }

		public int LevelId { get; private set; }

		public void Start(int levelId, int stars)
		{
			this.LevelId = levelId;
			this.StarsLevelStart = stars;
			this.StarsToShow = -1;
		}

		public void End(int starsToShow)
		{
			this.StarsToShow = starsToShow;
		}

		public void Reset()
		{
			this.StarsLevelStart = -1;
			this.StarsToShow = -1;
		}
	}
}
