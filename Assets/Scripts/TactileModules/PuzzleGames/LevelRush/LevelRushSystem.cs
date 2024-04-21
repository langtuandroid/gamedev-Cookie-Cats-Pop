using System;

namespace TactileModules.PuzzleGames.LevelRush
{
	public class LevelRushSystem : ILevelRushSystem
	{
		public LevelRushSystem(ILevelRushFeatureHandler levelRushFeatureHandler)
		{
			this.LevelRushFeatureHandler = levelRushFeatureHandler;
		}

		public ILevelRushFeatureHandler LevelRushFeatureHandler { get; private set; }
	}
}
