using System;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.PuzzleGame.ScheduledBooster.Data;

namespace TactileModules.PuzzleGame.ScheduledBooster.Model
{
	public interface IScheduledBooster
	{
		bool IsActive { get; }

		ScheduledBoosterDefinition Definition { get; }

		string Type { get; }

		int Price { get; }

		ActivatedFeatureInstanceData FeatureInstanceData { get; }

		int GetSecondsLeft();

		string GetTimeRemainingAsFormattedString();

		ScheduledBoosterInstanceCustomData GetInstanceCustomData();

		ScheduledBoosterMetaData GetMetaData();

		bool IsFree();

		void Activate();

		void Deactivate();
	}
}
