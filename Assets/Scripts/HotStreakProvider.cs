using System;
using System.Collections;
using Tactile;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.PuzzleGame.HotStreak.Data;

public class HotStreakProvider : IHotStreakProvider
{
	public string GetTextForNotification(TimeSpan timeSpan, ActivatedFeatureInstanceData instanceData)
	{
		return string.Format(L.Get("Only {0} hours left till Hot Streaks fizzles out!"), timeSpan.Hours);
	}

	public IEnumerator ShowProgressViewAndWait()
	{
		UIViewManager.UIViewStateGeneric<CCPHotStreakProgressView> vs = UIViewManager.Instance.ShowView<CCPHotStreakProgressView>(new object[0]);
		yield return vs.WaitForClose();
		yield break;
	}

	public HotStreakConfig Config
	{
		get
		{
			return ConfigurationManager.Get<HotStreakConfig>();
		}
	}
}
