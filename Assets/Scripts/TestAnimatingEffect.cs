using System;
using System.Collections;
using System.Collections.Generic;
using TactileModules.Foundation;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGame.MainLevels;

public class TestAnimatingEffect : Boot
{
	private LevelDatabaseCollection LevelDatabaseCollection
	{
		get
		{
			return ManagerRepository.Get<LevelDatabaseCollection>();
		}
	}

	protected override void BootCompleted()
	{
		FiberCtrl.Pool.Run(this.Flow(), false);
	}

	private IEnumerator Flow()
	{
		LevelProxy level = this.LevelDatabaseCollection.GetLevelDatabase<MainLevelDatabase>("Main").GetLevel(2);
		LevelSession levelSession = new LevelSession(level);
		levelSession.SetPreGameBoosters(new List<SelectedBooster>());
		UIViewManager.Instance.ShowView<LevelResultView>(new object[]
		{
			levelSession
		});
		yield break;
	}
}
