using System;
using System.Collections;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.PuzzleGame.HotStreak.Data
{
	public interface IHotStreakProvider
	{
		HotStreakConfig Config { get; }

		string GetTextForNotification(TimeSpan timeSpan, ActivatedFeatureInstanceData instanceData);

		IEnumerator ShowProgressViewAndWait();
	}
}
