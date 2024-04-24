using System;
using TactileModules.Foundation;
using UnityEngine;

public class GetMoreLivesView : UIView
{
	
	protected override void ViewLoad(object[] parameters)
	{
		
	}

	private void UpdateUI()
	{
		this.showSendLivesTimer = false;
		bool isLoggedInAndUserRegistered = false;
		this.sendLivesPivot.SetActive(isLoggedInAndUserRegistered);
		
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

	private void ButtonCloseClicked(UIEvent e) //TODO Fix
	{
		base.Close(0);
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

	public UILabel timerLabel;
	
	private LivesOverlay livesOverlay;

	private bool showSendLivesTimer;

	private float timer;
}
