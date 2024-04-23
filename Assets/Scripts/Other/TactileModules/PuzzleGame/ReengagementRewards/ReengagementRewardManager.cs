using System;
using System.Collections;
using System.Diagnostics;
using TactileModules.PuzzleGame.MainLevels;

namespace TactileModules.PuzzleGame.ReengagementRewards
{
	public class ReengagementRewardManager
	{
		public ReengagementRewardManager(MainProgressionManager mainLevelDatabase, IReengagementDataProvider dataProvider)
		{
			this.mainLevelDatabase = mainLevelDatabase;
			this.dataProvider = dataProvider;
			dataProvider.OnLevelComplete += this.OnLevelComplete;
		}

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnReengagementActivated;

		private PersistableState State
		{
			get
			{
				return this.dataProvider.PersistableState;
			}
		}

		public bool IsActiveOnLevel(int levelIndex)
		{
			return levelIndex == this.State.LevelIndex;
		}

		private void OnLevelComplete(int completedLevelIndex)
		{
			if (completedLevelIndex == this.State.LevelIndex)
			{
				this.dataProvider.Reset();
				this.dataProvider.Save();
			}
		}

		public void UpdateState()
		{
			this.State.LastDateVisitedMap = DateTime.UtcNow;
			this.dataProvider.Save();
		}

		public IEnumerator HandlePlayerOnGate()
		{
			yield return this.dataProvider.UnlockCurrentGate();
			this.dataProvider.Reset();
			this.dataProvider.Save();
			yield break;
		}

		public void HandlePlayerOnNormalLevel()
		{
			this.State.LevelIndex = this.mainLevelDatabase.GetFarthestUnlockedLevelIndex();
			this.State.IsPopupShown = true;
			if (this.OnReengagementActivated != null)
			{
				this.OnReengagementActivated();
			}
			this.dataProvider.Save();
		}

		private readonly MainProgressionManager mainLevelDatabase;

		private readonly IReengagementDataProvider dataProvider;
	}
}
