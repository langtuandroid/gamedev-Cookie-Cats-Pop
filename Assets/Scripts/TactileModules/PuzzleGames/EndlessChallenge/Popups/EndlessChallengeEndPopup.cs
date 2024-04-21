using System;
using System.Collections;

namespace TactileModules.PuzzleGames.EndlessChallenge.Popups
{
	public class EndlessChallengeEndPopup : MapPopupManager.IMapPopup
	{
		public EndlessChallengeEndPopup(EndlessChallengeHandler endlessChallengeHandler)
		{
			this.handler = endlessChallengeHandler;
			MapPopupManager.Instance.RegisterPopupObject(this);
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
			return this.handler.Config.IsActive && this.handler.ShouldDeactivateFeature();
		}

		private IEnumerator ShowPopup()
		{
			if (!this.ShouldShow())
			{
				yield break;
			}
			UIViewManager.UIViewStateGeneric<EndlessChallengeEndPopupView> vs = UIViewManager.Instance.ShowView<EndlessChallengeEndPopupView>(new object[0]);
			yield return vs.WaitForClose();
			this.handler.DeactivateEndlessChallenge();
			yield break;
		}

		private readonly EndlessChallengeHandler handler;
	}
}
