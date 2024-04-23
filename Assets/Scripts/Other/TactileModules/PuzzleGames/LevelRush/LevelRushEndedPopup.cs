using System;
using System.Collections;

namespace TactileModules.PuzzleGames.LevelRush
{
	public class LevelRushEndedPopup : MapPopupManager.IMapPopup
	{
		public LevelRushEndedPopup(ILevelRushActivation levelRushActivation)
		{
			this.levelRushActivation = levelRushActivation;
		}

		private bool ShouldShowPopup()
		{
			return this.levelRushActivation.ShouldDeactivateLevelRush() && this.levelRushActivation.HasPlayerAnyProgress();
		}

		private bool ShouldRunSilentAction()
		{
			return this.levelRushActivation.ShouldDeactivateLevelRush() && !this.levelRushActivation.HasPlayerAnyProgress();
		}

		private void EndAndDespawn()
		{
			this.levelRushActivation.DeactivateLevelRush();
		}

		private IEnumerator ShowPopup()
		{
			if (!this.levelRushActivation.HasActiveFeature())
			{
				yield break;
			}
			this.EndAndDespawn();
			yield return this.ShowLevelRushEndedView();
			yield break;
		}

		public void TryShowPopup(int unlockedLevelIndex, MapPopupManager.PopupFlow popupFlow)
		{
			if (!this.levelRushActivation.FeatureEnabled())
			{
				return;
			}
			if (this.ShouldShowPopup())
			{
				popupFlow.AddPopup(this.ShowPopup());
			}
			else if (this.ShouldRunSilentAction())
			{
				popupFlow.AddSilentAction(new Action(this.EndAndDespawn));
			}
		}

		public IEnumerator ShowLevelRushEndedView()
		{
			UIViewManager.UIViewStateGeneric<LevelRushEndedView> vs = UIViewManager.Instance.ShowView<LevelRushEndedView>(new object[0]);
			yield return vs.WaitForClose();
			yield break;
		}

		private readonly ILevelRushActivation levelRushActivation;
	}
}
