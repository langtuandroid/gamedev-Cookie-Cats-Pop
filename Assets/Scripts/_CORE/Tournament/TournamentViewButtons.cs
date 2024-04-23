using System;
using System.Diagnostics;
using UnityEngine;

public class TournamentViewButtons : ExtensibleView<TournamentViewButtons.IExtension>
{
	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action DidExit;



	public void Initialize(TournamentManager tournamentManager, CloudClientBase cloudClient)
	{
		this.cloudClient = cloudClient;
		this.tournamentManager = tournamentManager;
		this.nextPlayerToBeat.Initialize(cloudClient);
		this.UpdateUI();
		tournamentManager.Cloud.TournamentStateUpdatedEvent += this.TournamentStateUpdatedHandler;
	}

	public void UpdateUI()
	{
		TournamentRank currentRank = this.tournamentManager.GetCurrentRank();
		TournamentSetup.RankSetup rankSetup = SingletonAsset<TournamentSetup>.Instance.GetRankSetup(currentRank);
		if (rankSetup != null && base.Extension != null)
		{
			base.Extension.UpdateRewardButtonIcon(this.rewardsButton, rankSetup);
		}
		this.totalScore.text = this.tournamentManager.GetTotalScoreForTournament();
		TournamentCloudManager.Score score = this.tournamentManager.FindNextPlayerToBeatInfo();
		this.nextPlayerToBeat.UpdateUI(score);
	}

	protected override void ViewWillDisappear()
	{
		this.tournamentManager.Cloud.TournamentStateUpdatedEvent -= this.TournamentStateUpdatedHandler;
	}

	public void TournamentStateUpdatedHandler()
	{
		if (this.tournamentManager.Cloud.TournamentActive)
		{
			this.UpdateUI();
		}
	}

	private void Update()
	{
		if (UIViewManager.Instance.IsEscapeKeyDownAndAvailable(base.gameObject.layer))
		{
			this.Exit();
			return;
		}
		if (this.tournamentManager != null)
		{
			this.countdown.text = this.tournamentManager.GetTimeRemainingForTournamentAsString();
		}
	}

	private void ShowRankRewardsClicked(UIEvent e)
	{
		UIViewManager.Instance.ShowView<TournamentInfoView>(new object[0]).View.Initialize(this.tournamentManager.GetCurrentRank());
	}

	private void BackToMapViewClicked(UIEvent e)
	{
		this.Exit();
	}

	private void Exit()
	{
		this.DidExit();
	}

	private void TournamentLeaderboardButtonClicked(UIEvent e)
	{
		UIViewManager.UIViewStateGeneric<TournamentLeaderboardView> uiviewStateGeneric = UIViewManager.Instance.ShowView<TournamentLeaderboardView>(new object[0]);
		uiviewStateGeneric.View.Initialize(false, this.cloudClient);
	}

	[SerializeField]
	private UILabel totalScore;

	[SerializeField]
	private UILabel countdown;

	[SerializeField]
	private UIInstantiator rewardsButton;

	[SerializeField]
	private TournamentMapAvatarToBeat nextPlayerToBeat;

	private TournamentManager tournamentManager;

	private CloudClientBase cloudClient;

	public interface IExtension
	{
		void UpdateRewardButtonIcon(UIInstantiator button, TournamentSetup.RankSetup rankSetup);
	}
}
