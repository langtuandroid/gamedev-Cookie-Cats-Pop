using System;
using System.Collections;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.TactileCloud.TargetingParameters;

namespace TactileModules.PuzzleGames.EndlessChallenge
{
	public class EndlessChallengeTargetingParameters : ITargetingParametersProvider
	{
		public EndlessChallengeTargetingParameters(MainProgressionManager mainProgression)
		{
			this.mainProgression = mainProgression;
		}

		private int LevelsLeftInMainProgression
		{
			get
			{
				int maxAvailableLevelHumanNumber = this.mainProgression.GetMaxAvailableLevelHumanNumber();
				int farthestUnlockedLevelHumanNumber = this.mainProgression.GetFarthestUnlockedLevelHumanNumber();
				return maxAvailableLevelHumanNumber - farthestUnlockedLevelHumanNumber;
			}
		}

		public Hashtable GetAdditionalTargetingParameters()
		{
			return new Hashtable
			{
				{
					"levelsLeftInMainProgression",
					this.LevelsLeftInMainProgression
				}
			};
		}

		private readonly MainProgressionManager mainProgression;
	}
}
