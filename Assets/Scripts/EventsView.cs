using System;
using Tactile;
using TactileModules.FacebookExtras;
using TactileModules.Foundation;
using TactileModules.PuzzleGames.GameCore;
using UnityEngine;

public class EventsView : UIView
{
	private static TournamentCloudManager TournamentCloudManager
	{
		get
		{
			return ManagerRepository.Get<TournamentCloudManager>();
		}
	}

	private ConfigurationManager ConfigurationManager
	{
		get
		{
			return ManagerRepository.Get<ConfigurationManager>();
		}
	}

	private CloudClient CloudClient
	{
		get
		{
			return ManagerRepository.Get<CloudClient>();
		}
	}

	private FacebookLoginManager FacebookLoginManager
	{
		get
		{
			return ManagerRepository.Get<FacebookLoginManager>();
		}
	}

	protected override void ViewWillAppear()
	{
		EventsView.TournamentCloudManager.TournamentEndedEvent += this.UpdateBadgeStatus;
		this.UpdateBadgeStatus();
	}

	protected override void ViewWillDisappear()
	{
		EventsView.TournamentCloudManager.TournamentEndedEvent -= this.UpdateBadgeStatus;
	}

	private void Update()
	{
		this.tournamentTimer.text = this.GetTournamentStatusInfo();
		this.dailyQuestTimer.text = this.GetDailyQuestStatusInfo();
	}

	private void UpdateBadgeStatus()
	{
		this.tournamentNotificationBadge.SetActive(EventsView.TournamentCloudManager.TournamentEnded && EventsView.TournamentCloudManager.TournamentJoined);
		this.dailyQuestNotificationBadge.SetActive(DailyQuestManager.Instance.WantAttention);
	}

	private void TournamentButtonClicked(UIEvent e)
	{
		if (UICamera.InputDisabled)
		{
			return;
		}
		if (TournamentManager.Instance.IsTournamentRankUnlocked(TournamentRank.Bronze))
		{
			ITournamentSystem tournamentSystem = ManagerRepository.Get<ITournamentSystem>();
			tournamentSystem.ControllerFactory.CreateAndPushMapFlow();
		}
		else
		{
			UIViewManager.Instance.ShowView<TournamentLockedView>(new object[0]);
		}
	}

	private void DailyQuestButtonClicked(UIEvent e)
	{
		IDailyQuestSystem dailyQuestSystem = ManagerRepository.Get<IDailyQuestSystem>();
		if (dailyQuestSystem.Manager.IsChallengeAvailable)
		{
			FlowStack flowStack = ManagerRepository.Get<FlowStack>();
			DailyQuestMapFlow c = dailyQuestSystem.Factory.CreateMapFlow();
			flowStack.Push(c);
		}
		else
		{
			UIViewManager.Instance.ShowView<DailyQuestLockedView>(new object[0]);
		}
	}

	private void Dismiss(UIEvent e)
	{
		base.Close(0);
	}

	private string GetTournamentStatusInfo()
	{
		bool flag = TournamentManager.Instance.IsTournamentRankUnlocked(TournamentRank.Bronze);
		this.tournamentPlayButton.gameObject.SetActive(flag);
		this.tournamentLockedButton.gameObject.SetActive(!flag);
		if (!flag)
		{
			int num = TournamentManager.Instance.LevelNrRequiredForTournament(TournamentRank.Bronze);
			return string.Format(L.Get("Unlocks at level {0}"), num);
		}
		if (EventsView.TournamentCloudManager.TournamentJoined && !EventsView.TournamentCloudManager.TournamentEnded)
		{
			return string.Format(L.Get("Ends in: {0}"), TournamentManager.Instance.GetTimeRemainingForTournamentAsString());
		}
		if (EventsView.TournamentCloudManager.IsTournamentResultReady)
		{
			return L.Get("Claim Reward!");
		}
		return string.Empty;
	}

	private string GetDailyQuestStatusInfo()
	{
		bool isChallengeAvailable = DailyQuestManager.Instance.IsChallengeAvailable;
		this.dailyQuestPlayButton.gameObject.SetActive(isChallengeAvailable);
		this.dailyQuestLockedButton.gameObject.SetActive(!isChallengeAvailable);
		if (!isChallengeAvailable)
		{
			int levelRequiredForDailyQuests = this.ConfigurationManager.GetConfig<DailyQuestConfig>().LevelRequiredForDailyQuests;
			return string.Format(L.Get("Unlocks at level {0}"), levelRequiredForDailyQuests);
		}
		if (DailyQuestManager.Instance.HasQuestCooldown)
		{
			return L.Get("Cooldown: ") + DailyQuestManager.Instance.GetSecondsLeftInCooldownStr();
		}
		return L.Get("Ends in: ") + DailyQuestManager.Instance.GetSecondsLeftTillQuestExpiredStr();
	}

	[Header("Tournament Settings")]
	public UILabel tournamentTimer;

	public UIElement tournamentPlayButton;

	public UIElement tournamentLockedButton;

	[Header("Daily Quest Settings")]
	public UILabel dailyQuestTimer;

	public UIElement dailyQuestPlayButton;

	public UIElement dailyQuestLockedButton;

	[Header("Badges")]
	[SerializeField]
	private GameObject tournamentNotificationBadge;

	[SerializeField]
	private GameObject dailyQuestNotificationBadge;
}
