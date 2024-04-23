using System;
using System.Collections;

namespace TactileModules.PuzzleGames.EndlessChallenge.Popups
{
	public class EndlessChallengeStartPopup : MapPopupManager.IMapPopup
	{
		public EndlessChallengeStartPopup(EndlessChallengeHandler endlessChallengeHandler)
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
			return !this.handler.IsEndlessChallengeActive && this.handler.ShouldActivateEndlessChallenge();
		}

		private IEnumerator ShowPopup()
		{
			if (!this.ShouldShow())
			{
				yield break;
			}
			this.handler.ActivateEndlessChallenge();
			UIViewManager.UIViewStateGeneric<EndlessChallengeStartPopupView> vs = UIViewManager.Instance.ShowView<EndlessChallengeStartPopupView>(new object[0]);
			yield return vs.WaitForClose();
			yield break;
		}

		private readonly EndlessChallengeHandler handler;
	}
}
