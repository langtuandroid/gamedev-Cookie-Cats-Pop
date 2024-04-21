using System;
using System.Collections;
using TactileModules.ReengagementRewards.Views;

namespace TactileModules.PuzzleGame.ReengagementRewards.Popups
{
	public class ReengagementRewardPopup : MapPopupManager.IMapPopup
	{
		public ReengagementRewardPopup(ReengagementRewardManager manager, UIViewManager uiViewManager, IReengagementDataProvider dataProvider)
		{
			this.manager = manager;
			this.dataProvider = dataProvider;
			this.uiViewManager = uiViewManager;
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
			PersistableState persistableState = this.dataProvider.PersistableState;
			if (persistableState.IsPopupShown)
			{
				return false;
			}
			DateTime lastDateVisitedMap = persistableState.LastDateVisitedMap;
			this.manager.UpdateState();
			return this.AreDaysAwaySurpassed(lastDateVisitedMap);
		}

		private bool AreDaysAwaySurpassed(DateTime storedDate)
		{
			DateTime utcNow = DateTime.UtcNow;
			if (storedDate == DateHelper.DefaultTime)
			{
				return false;
			}
			double totalDays = (utcNow - storedDate).TotalDays;
			return totalDays >= (double)this.dataProvider.Config.DaysAwayRequired;
		}

		private IEnumerator ShowPopup()
		{
			this.dataProvider.ClaimReward(this.dataProvider.Config.Items);
			LevelProxy currentGateLevelProxy = this.dataProvider.GetCurrentGate();
			if (currentGateLevelProxy.IsValid)
			{
				this.dataProvider.UpdateGate(currentGateLevelProxy.Index);
				yield return this.HandlePlayerOnGate();
			}
			else
			{
				yield return this.HandlePlayerOnNormalLevel();
			}
			yield break;
		}

		protected virtual IEnumerator HandlePlayerOnGate()
		{
			UIViewManager.UIViewState viewState = this.uiViewManager.ShowView<ReengagementRewardsGateView>(new object[0]);
			yield return viewState.WaitForClose();
			yield return this.manager.HandlePlayerOnGate();
			yield break;
		}

		protected virtual IEnumerator HandlePlayerOnNormalLevel()
		{
			UIViewManager.UIViewState viewState = this.uiViewManager.ShowView<ReengagementRewardsView>(new object[0]);
			yield return viewState.WaitForClose();
			this.manager.HandlePlayerOnNormalLevel();
			yield break;
		}

		protected readonly ReengagementRewardManager manager;

		protected readonly IReengagementDataProvider dataProvider;

		protected readonly UIViewManager uiViewManager;
	}
}
