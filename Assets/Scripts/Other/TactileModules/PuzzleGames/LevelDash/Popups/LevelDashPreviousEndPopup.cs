using System;
using System.Collections;

namespace TactileModules.PuzzleGames.LevelDash.Popups
{
	public class LevelDashPreviousEndPopup : MapPopupManager.IMapPopup
	{
		public LevelDashPreviousEndPopup(LevelDashManager manager, LevelDashViewController levelDashViewController)
		{
			this.manager = manager;
			this.levelDashViewController = levelDashViewController;
		}

		public void TryShowPopup(int unlockedLevelIndex, MapPopupManager.PopupFlow popupFlow)
		{
			if (this.ShouldShow())
			{
				popupFlow.AddPopup(this.ShowPopup());
			}
		}

		private bool ShouldShow()
		{
			return this.manager.ShouldShowPreviousEndView();
		}

		private IEnumerator ShowPopup()
		{
			yield return this.levelDashViewController.ShowResultsForPreviousLevelDash();
			yield break;
		}

		private LevelDashManager manager;

		private LevelDashViewController levelDashViewController;
	}
}
