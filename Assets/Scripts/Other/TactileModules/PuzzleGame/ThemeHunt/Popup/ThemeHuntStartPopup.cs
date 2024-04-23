using System;
using System.Collections;
using TactileModules.PuzzleGame.ThemeHunt.Views;

namespace TactileModules.PuzzleGame.ThemeHunt.Popup
{
	public class ThemeHuntStartPopup : MapPopupManager.IMapPopup
	{
		public ThemeHuntStartPopup(ThemeHuntManagerBase manager)
		{
			this.manager = manager;
		}

		private bool ShouldShowPopup
		{
			get
			{
				return this.manager.ShouldStartHunt();
			}
		}

		public void TryShowPopup(int unlockedLevelIndex, MapPopupManager.PopupFlow popupFlow)
		{
			if (this.ShouldShowPopup)
			{
				popupFlow.AddPopup(this.TryStartThemeHunt());
			}
		}

		public IEnumerator TryStartThemeHunt()
		{
			if (this.manager.ShouldStartHunt())
			{
				this.manager.StartHunt();
				yield return this.ShowThemeHuntIntroView();
			}
			yield break;
		}

		private IEnumerator ShowThemeHuntIntroView()
		{
			UIViewManager.UIViewStateGeneric<ThemeHuntIntroView> vs = UIViewManager.Instance.ShowView<ThemeHuntIntroView>(new object[0]);
			yield return vs.WaitForClose();
			yield break;
		}

		private readonly ThemeHuntManagerBase manager;
	}
}
