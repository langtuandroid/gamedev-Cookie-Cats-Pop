using System;
using System.Collections;
using TactileModules.PuzzleGame.ThemeHunt.Views;

namespace TactileModules.PuzzleGame.ThemeHunt.Popup
{
	public class ThemeHuntEndPopup : MapPopupManager.IMapPopup
	{
		public ThemeHuntEndPopup(ThemeHuntManagerBase manager)
		{
			this.manager = manager;
		}

		private bool ShouldShowPopup
		{
			get
			{
				return this.manager.ShouldEndHunt();
			}
		}

		public void TryShowPopup(int unlockedLevelIndex, MapPopupManager.PopupFlow popupFlow)
		{
			if (this.ShouldShowPopup)
			{
				popupFlow.AddPopup(this.TryEndThemeHunt());
			}
		}

		public IEnumerator TryEndThemeHunt()
		{
			if (this.manager.ShouldEndHunt())
			{
				this.manager.EndHunt();
				yield return this.ShowThemeHuntEndView();
			}
			yield break;
		}

		private IEnumerator ShowThemeHuntEndView()
		{
			UIViewManager.UIViewStateGeneric<ThemeHuntEndView> vs = UIViewManager.Instance.ShowView<ThemeHuntEndView>(new object[0]);
			yield return vs.WaitForClose();
			yield break;
		}

		private readonly ThemeHuntManagerBase manager;
	}
}
