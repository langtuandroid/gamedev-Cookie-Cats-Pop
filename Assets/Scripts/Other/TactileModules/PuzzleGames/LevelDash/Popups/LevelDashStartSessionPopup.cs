using System;
using System.Collections;

namespace TactileModules.PuzzleGames.LevelDash.Popups
{
	public class LevelDashStartSessionPopup : MapPopupManager.IMapPopup
	{
		public LevelDashStartSessionPopup(LevelDashManager manager, LevelDashViewController levelDashViewController)
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
			return this.manager.IsActive() && this.manager.HasPresentedStartView();
		}

		private IEnumerator ShowPopup()
		{
			yield return this.levelDashViewController.ShowStartPopup(true);
			yield break;
		}

		private LevelDashManager manager;

		private LevelDashViewController levelDashViewController;
	}
}
