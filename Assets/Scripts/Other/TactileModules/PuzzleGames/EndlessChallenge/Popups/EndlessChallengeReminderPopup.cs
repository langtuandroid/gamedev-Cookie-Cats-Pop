using System;
using System.Collections;
using Fibers;

namespace TactileModules.PuzzleGames.EndlessChallenge.Popups
{
	public class EndlessChallengeReminderPopup : MapPopupManager.IMapPopup
	{
		public EndlessChallengeReminderPopup(EndlessChallengeHandler endlessChallengeHandler)
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
			return this.handler.IsEndlessChallengeActive && !this.handler.ShouldDeactivateFeature();
		}

		private IEnumerator ShowPopup()
		{
			if (!this.ShouldShow())
			{
				yield break;
			}
			EnumeratorResult<UIViewManager.UIViewState> result = new EnumeratorResult<UIViewManager.UIViewState>();
			yield return this.handler.TryShowLeaderboard(result);
			yield break;
		}

		private readonly EndlessChallengeHandler handler;
	}
}
