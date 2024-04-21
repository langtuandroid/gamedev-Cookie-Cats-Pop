using System;
using TactileModules.FacebookExtras;
using TactileModules.Foundation;
using UnityEngine;

public class GetMoreLivesView : UIView
{
	private FacebookLoginManager FacebookLoginManager
	{
		get
		{
			return ManagerRepository.Get<FacebookLoginManager>();
		}
	}

	protected override void ViewLoad(object[] parameters)
	{
		this.playingTournament = (parameters.Length > 0 && (bool)parameters[0]);
	}

	private void UpdateUI()
	{
		this.showSendLivesTimer = false;
		bool isLoggedInAndUserRegistered = this.FacebookLoginManager.IsLoggedInAndUserRegistered;
		this.sendLivesPivot.SetActive(isLoggedInAndUserRegistered);
		if (!isLoggedInAndUserRegistered)
		{
			this.askFriendsButton.localPosition = this.askFriendsNotLoggedInLocation.localPosition;
		}
		else
		{
			if (this.playingTournament)
			{
				this.sendLivesPivot.SetActive(false);
				this.askFriendsButton.localPosition = this.askFriendsNotLoggedInLocation.localPosition;
			}
			this.askFriendsButton.localPosition = this.askFriendsLoggedInLocation.localPosition;
			this.showSendLivesTimer = !ManagerRepository.Get<SendLivesAtStartManager>().CanShowSendLivesAtStart();
			this.sendLivesButton.SetActive(!this.showSendLivesTimer);
			this.sendLivesTimer.SetActive(this.showSendLivesTimer);
			if (this.showSendLivesTimer)
			{
				this.UpdateTimerLabel();
			}
		}
	}

	protected override void ViewWillAppear()
	{
		this.livesOverlay = base.ObtainOverlay<LivesOverlay>();
	}

	protected override void ViewDidDisappear()
	{
		base.ReleaseOverlay<LivesOverlay>();
	}

	protected override void ViewGotFocus()
	{
		this.livesOverlay.OnLifeButtonClicked = delegate()
		{
			base.Close(0);
		};
		this.UpdateUI();
	}

	private void ButtonCloseClicked(UIEvent e)
	{
		base.Close(0);
	}

	private void ButtonAskFriendsClicked(UIEvent e)
	{
		if (this.FacebookLoginManager.IsLoggedInAndUserRegistered)
		{
			UIViewManager.Instance.ShowView<FacebookSelectFriendsAndRequestView>(new object[]
			{
				(!this.playingTournament) ? FacebookSelectFriendsAndRequestView.RequestType.Life : FacebookSelectFriendsAndRequestView.RequestType.TournamentLife,
				true
			});
		}
		else
		{
			UIViewManager.Instance.ShowView<FacebookLoginInfoView>(new object[]
			{
				LoginContext.Lives
			});
		}
	}

	private void SendLivesButtonClicked(UIEvent e)
	{
		UIViewManager.Instance.ShowView<FacebookSelectFriendsAndRequestView>(new object[]
		{
			FacebookSelectFriendsAndRequestView.RequestType.GiftLives,
			false
		});
		this.UpdateUI();
	}

	private TimeSpan UpdateTimerLabel()
	{
		TimeSpan timeToNextSendLives = ManagerRepository.Get<SendLivesAtStartManager>().GetTimeToNextSendLives();
		this.timerLabel.text = string.Format("{0:D2}:{1:D2}:{2:D2}", timeToNextSendLives.Hours, timeToNextSendLives.Minutes, timeToNextSendLives.Seconds);
		return timeToNextSendLives;
	}

	private void Update()
	{
		if (!this.showSendLivesTimer)
		{
			return;
		}
		if (this.timer >= 1f)
		{
			this.timer = 0f;
			TimeSpan t = this.UpdateTimerLabel();
			if (t < TimeSpan.Zero)
			{
				this.UpdateUI();
			}
		}
		else
		{
			this.timer += Time.deltaTime;
		}
	}

	public GameObject sendLivesPivot;

	public GameObject sendLivesButton;

	public GameObject sendLivesTimer;

	public UILabel timerLabel;

	public Transform askFriendsButton;

	public Transform askFriendsLoggedInLocation;

	public Transform askFriendsNotLoggedInLocation;

	private LivesOverlay livesOverlay;

	private bool showSendLivesTimer;

	private float timer;

	private bool playingTournament;
}
