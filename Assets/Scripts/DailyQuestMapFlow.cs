using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.SagaCore;

public class DailyQuestMapFlow : IFlow, IFiberRunnable
{
	public DailyQuestMapFlow(DailyQuestManager dailyQuestManager, IPlayFlowFactory playLevelSystem, MapFacade mapFacade, DailyQuestFactory dailyQuestFactory)
	{
		this.playLevelSystem = playLevelSystem;
		this.mapFacade = mapFacade;
		this.dailyQuestFactory = dailyQuestFactory;
		this.dailyQuestManager = dailyQuestManager;
	}

	public IEnumerator Run()
	{
		for (;;)
		{
			yield return this.CreateMapViewAndCheckin();
			while (this.nextFlowAction == DailyQuestMapFlow.FlowAction.None)
			{
				yield return null;
			}
			DailyQuestMapFlow.FlowAction flowAction = this.nextFlowAction;
			if (flowAction != DailyQuestMapFlow.FlowAction.Play)
			{
				if (flowAction == DailyQuestMapFlow.FlowAction.Close)
				{
					break;
				}
			}
			else
			{
				this.DestroyMapViewAndCheckOut();
				yield return this.RunQuestLevel();
			}
			this.nextFlowAction = DailyQuestMapFlow.FlowAction.None;
		}
		this.DestroyMapViewAndCheckOut();
		yield break;
		yield break;
	}

	public void OnExit()
	{
	}

	private IEnumerator CreateMapViewAndCheckin()
	{
		UIViewManager.UIViewStateGeneric<DailyQuestSagaView> vs = UIViewManager.Instance.ShowView<DailyQuestSagaView>(new object[0]);
		this.view = vs.View;
		this.view.Initialize(this.dailyQuestManager.Config.Quests, new Func<string>(this.CoolDownTimerLabelCallback), this.mapFacade, this.dailyQuestManager, this.dailyQuestFactory);
		this.view.PlayClicked += this.PlayQuestLevel;
		this.view.ClickedClose += this.CloseClicked;
		this.view.ClickedClaim += this.ClaimClicked;
		this.view.CooldownSkipped += this.CooldownSkipped;
		this.view.QuestSkipped += this.QuestSkipped;
		yield return this.dailyQuestManager.Checkin(new DailyQuestMapFlow.ManagerUpdateDelegate(this.view));
		yield break;
	}

	private void DestroyMapViewAndCheckOut()
	{
		this.dailyQuestManager.Checkout();
		if (this.view != null)
		{
			this.view.PlayClicked -= this.PlayQuestLevel;
			this.view.ClickedClose -= this.CloseClicked;
			this.view.ClickedClaim -= this.ClaimClicked;
			this.view.CooldownSkipped -= this.CooldownSkipped;
			this.view.QuestSkipped -= this.QuestSkipped;
			this.view.Close(0);
			this.view = null;
		}
	}

	private void CloseClicked()
	{
		this.nextFlowAction = DailyQuestMapFlow.FlowAction.Close;
	}

	private void PlayQuestLevel()
	{
		this.nextFlowAction = DailyQuestMapFlow.FlowAction.Play;
	}

	private void ClaimClicked()
	{
		DailyQuestManager.Instance.ManualConsumePendingReward();
	}

	private void QuestSkipped()
	{
		this.dailyQuestManager.SkipCurrentQuest();
	}

	private void CooldownSkipped()
	{
		this.dailyQuestManager.SkipCooldown();
	}

	private string CoolDownTimerLabelCallback()
	{
		return this.dailyQuestManager.GetSecondsLeftInCooldownStr();
	}

	private IEnumerator RunQuestLevel()
	{
		LevelProxy levelToPlay = this.dailyQuestManager.CurrentQuestLevel;
		DailyQuestPlayFlow playFlow = new DailyQuestPlayFlow(this.dailyQuestManager, this.playLevelSystem, levelToPlay);
		yield return playFlow;
		yield break;
	}

	private readonly IPlayFlowFactory playLevelSystem;

	private readonly MapFacade mapFacade;

	private readonly DailyQuestFactory dailyQuestFactory;

	private readonly DailyQuestManager dailyQuestManager;

	private DailyQuestSagaView view;

	private DailyQuestMapFlow.FlowAction nextFlowAction;

	private enum FlowAction
	{
		None,
		Play,
		Close
	}

	private class ManagerUpdateDelegate : DailyQuestManager.IUpdateDelegate
	{
		public ManagerUpdateDelegate(DailyQuestSagaView view)
		{
			this.view = view;
		}

		IEnumerator DailyQuestManager.IUpdateDelegate.OnConsumeReward(int questIndex, DailyQuestInfo info, bool isLastReward)
		{
			while (UIViewManager.Instance.AnyViewsAnimating)
			{
				yield return null;
			}
			yield return this.view.AnimateConsumingReward(questIndex, info);
			yield break;
		}

		IEnumerator DailyQuestManager.IUpdateDelegate.OnShowHeadStartView()
		{
			UIViewManager.UIViewState viewState = UIViewManager.Instance.ShowView<DailyQuestWelcomeView>(new object[0]);
			yield return viewState.WaitForClose();
			yield break;
		}

		IEnumerator DailyQuestManager.IUpdateDelegate.OnChallengeExpired()
		{
			yield break;
		}

		IEnumerator DailyQuestManager.IUpdateDelegate.OnMissedQuests(int missed, int furthestAvailableQuestIndex)
		{
			yield return FiberHelper.Wait(0.5f, (FiberHelper.WaitFlag)0);
			UIViewManager.UIViewStateGeneric<DailyQuestInfoDaysView> vs = UIViewManager.Instance.ShowView<DailyQuestInfoDaysView>(new object[]
			{
				missed
			});
			yield return vs.WaitForClose();
			yield return this.view.AnimateCrossingOutMissedQuests(missed, furthestAvailableQuestIndex);
			yield break;
		}

		IEnumerator DailyQuestManager.IUpdateDelegate.OnFriendProgression(List<DailyQuestManager.FriendProgressionState> states)
		{
			yield return this.view.AnimateFriendProgression(states);
			yield break;
		}

		void DailyQuestManager.IUpdateDelegate.OnStateUpdated(DailyQuestChallengeState state)
		{
			this.view.UpdateFromState(state);
		}

		private readonly DailyQuestSagaView view;
	}
}
