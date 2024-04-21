using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.SagaCore;

public class TournamentMapFlow : MapFlow
{
	public TournamentMapFlow(CloudClientBase cloudClient, MapIdentifier mapIdentifier, MapFacade mapFacade, IPlayFlowFactory playFlowFactory, IFullScreenManager fullScreenManager, IFlowStack flowStack, TournamentManager tournamentManager) : base(mapIdentifier, mapFacade, fullScreenManager, flowStack)
	{
		this.cloudClient = cloudClient;
		this.playFlowFactory = playFlowFactory;
		this.tournamentManager = tournamentManager;
		this.uiProvider = new TournamentMapFlow.FlowProvider(cloudClient);
	}

	protected override IEnumerator PostPlayFlowSequence(int nextDotIndexToOpen)
	{
		bool bail = false;
		yield return this.tournamentManager.StartFlow(this.uiProvider, delegate(bool didSucceed)
		{
			bail = !didSucceed;
		});
		if (bail)
		{
			base.EndThisFlow();
		}
		else
		{
			this.Refresh();
			if (nextDotIndexToOpen > 0)
			{
				base.StartFlowForDot(nextDotIndexToOpen);
			}
		}
		yield break;
	}

	private void Refresh()
	{
		base.MapContentController.Refresh();
	}

	private void TournamentEndedHandler()
	{
		if (this.flowFiber.IsTerminated)
		{
			this.flowFiber.Start(this.EndSequence());
		}
	}

	private IEnumerator EndSequence()
	{
		yield return TournamentManager.Instance.EndFlow(this.uiProvider);
		yield return base.PostPlayFlowSequence();
		yield break;
	}

	protected override IEnumerator AfterScreenAcquired()
	{
		UIViewManager.UIViewStateGeneric<TournamentViewButtons> uiviewStateGeneric = UIViewManager.Instance.ShowView<TournamentViewButtons>(new object[0]);
		uiviewStateGeneric.View.Initialize(this.tournamentManager, this.cloudClient);
		uiviewStateGeneric.View.DidExit += base.EndThisFlow;
		this.tournamentManager.Cloud.TournamentEndedEvent += this.TournamentEndedHandler;
		base.MapContentController.JumpToDot(0);
		yield break;
	}

	protected override void AfterScreenLost()
	{
		this.tournamentManager.Cloud.TournamentEndedEvent -= this.TournamentEndedHandler;
	}

	protected override int GetFarthestUnlockedDotIndex()
	{
		return this.tournamentManager.GetHighestUnlockedLevel();
	}

	protected override IFlow CreateFlowForDot(int dotIndex)
	{
		if (this.tournamentManager.HasUnlimitedLives() || this.tournamentManager.Lives > 0)
		{
			return new TournamentPlayFlow(dotIndex, this.playFlowFactory, this.tournamentManager);
		}
		UIViewManager.UIViewStateGeneric<NoMoreLivesView> uiviewStateGeneric = UIViewManager.Instance.ShowView<NoMoreLivesView>(new object[0]);
		return null;
	}

	public override SagaAvatarInfo CreateMeAvatarInfo()
	{
		return new SagaAvatarInfo
		{
			dotIndex = this.GetFarthestUnlockedDotIndex()
		};
	}

	public override Dictionary<CloudUser, SagaAvatarInfo> CreateFriendsAvatarInfos()
	{
		return null;
	}

	private readonly CloudClientBase cloudClient;

	private readonly IPlayFlowFactory playFlowFactory;

	private readonly TournamentManager tournamentManager;

	private readonly TournamentManager.IFlowProvider uiProvider;

	private readonly Fiber flowFiber = new Fiber();

	private class FlowProvider : TournamentManager.IFlowProvider
	{
		public FlowProvider(CloudClientBase cloudClientBase)
		{
			this.cloudClientBase = cloudClientBase;
		}

		public IEnumerator ShowTournamentEnded()
		{
			UIViewManager.UIViewStateGeneric<TournamentEndedView> endedViewState = UIViewManager.Instance.ShowView<TournamentEndedView>(new object[0]);
			yield return endedViewState.WaitForClose();
			UICamera.DisableInput();
			yield return new Fiber.OnExit(delegate()
			{
				UICamera.EnableInput();
			});
			UIViewManager.UIViewStateGeneric<TournamentLeaderboardView> vs = UIViewManager.Instance.ShowView<TournamentLeaderboardView>(new object[0]);
			vs.View.Initialize(true, this.cloudClientBase);
			vs.View.AnimateFocusOfPlayerOnViewDidAppear = true;
			yield return vs.WaitForClose();
			UICamera.EnableInput();
			yield return new Fiber.OnExit(delegate()
			{
			});
			this.tournamentEndRank = TournamentManager.Instance.GetCurrentRank();
			yield break;
		}

		public IEnumerator JoinNewTournament()
		{
			UIViewManager.UIViewStateGeneric<TournamentSelectView> selectVs = UIViewManager.Instance.ShowView<TournamentSelectView>(new object[0]);
			selectVs.View.RankChanged = delegate(TournamentRank r)
			{
			};
			yield return selectVs.WaitForClose();
			if (TournamentManager.Instance.Cloud.TournamentJoined)
			{
				UIViewManager.UIViewStateGeneric<TournamentTutorialView> vs = UIViewManager.Instance.ShowView<TournamentTutorialView>(new object[0]);
				vs.View.StartShowingSteps(SingletonAsset<TournamentSetup>.Instance.tutorialSteps);
				yield return vs.WaitForClose();
			}
			yield break;
		}

		public IEnumerator ShowReward(TournamentPrizeConfig prizeGiven)
		{
			TournamentRankConfig rankPrizes = TournamentManager.Instance.GetRewardsForTournamentRank(this.tournamentEndRank);
			int prizeTier = rankPrizes.Prizes.IndexOf(prizeGiven);
			UICamera.DisableInput();
			yield return new Fiber.OnExit(delegate()
			{
				UICamera.EnableInput();
			});
			UIViewManager.UIViewStateGeneric<TournamentInfoView> vs = UIViewManager.Instance.ShowView<TournamentInfoView>(new object[0]);
			vs.View.Initialize(this.tournamentEndRank);
			yield return vs.View.AnimateAvatar(prizeTier);
			vs.View.Close(0);
			UICamera.EnableInput();
			yield return new Fiber.OnExit(delegate()
			{
			});
			TournamentSetup.TutorialStep message = SingletonAsset<TournamentSetup>.Instance.GetDebriefingMessage(prizeTier);
			if (message != null)
			{
				UIViewManager.UIViewStateGeneric<TournamentTutorialView> debriefingView = UIViewManager.Instance.ShowView<TournamentTutorialView>(new object[0]);
				debriefingView.View.StartShowingSteps(new List<TournamentSetup.TutorialStep>
				{
					message
				});
				yield return debriefingView.WaitForClose();
			}
			yield break;
		}

		private readonly CloudClientBase cloudClientBase;

		private TournamentRank tournamentEndRank;
	}
}
