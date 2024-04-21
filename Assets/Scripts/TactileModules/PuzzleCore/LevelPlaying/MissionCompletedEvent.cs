using System;

namespace TactileModules.PuzzleCore.LevelPlaying
{
	[TactileAnalytics.EventAttribute("levelCompleted", true)]
	public class MissionCompletedEvent : MissionEndEvent
	{
		public MissionCompletedEvent(ILevelAttempt levelAttempt, IMainProgressionForAnalytics mainProgression) : base(levelAttempt)
		{
			this.FirstComplete = levelAttempt.WasCompletedForTheFirstTime;
			this.LevelStars = levelAttempt.Stats.Stars;
			if (levelAttempt.WasCompletedForTheFirstTime)
			{
				LevelProxy farthestUnlockedLevelProxy = mainProgression.GetFarthestUnlockedLevelProxy();
				this.ReachedLevelNumber = farthestUnlockedLevelProxy.RootDatabase.GetHumanNumber(farthestUnlockedLevelProxy);
				this.ReachedLevelFilename = farthestUnlockedLevelProxy.LevelAssetName;
			}
		}

		private TactileAnalytics.RequiredParam<int> LevelStars { get; set; }

		private TactileAnalytics.RequiredParam<bool> FirstComplete { get; set; }

		private TactileAnalytics.OptionalParam<int> ReachedLevelNumber { get; set; }

		private TactileAnalytics.OptionalParam<string> ReachedLevelFilename { get; set; }
	}
}
