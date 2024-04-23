using System;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.PuzzleGames.LevelDash.Providers
{
	public interface ILevelDashDataProvider
	{
		int FarthestCompletedLevelHumanNumber { get; }

		int MaxAvailableLevelHumanNumber { get; }

		event Action LevelCompleted;

		void ClaimReward(LevelDashConfig.Reward reward);

		string GetTextForNotification(TimeSpan timeSpan, ActivatedFeatureInstanceData instanceData);
	}
}
