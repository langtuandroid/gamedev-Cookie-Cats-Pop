using System;
using UnityEngine;

public class HardLevelsView : UIView
{
	protected override void ViewLoad(object[] parameters)
	{
		HardLevelsView.EventState eventState = (parameters.Length <= 0) ? HardLevelsView.EventState.None : ((HardLevelsView.EventState)parameters[0]);
		this.expirationDate = ((parameters.Length <= 1) ? DateTime.MinValue : ((DateTime)parameters[1]));
		this.startPivot.SetActive(eventState == HardLevelsView.EventState.Started);
		this.completedPivot.SetActive(eventState == HardLevelsView.EventState.Completed);
		this.expiredPivot.SetActive(eventState == HardLevelsView.EventState.Expired);
		this.reminderPivot.SetActive(eventState == HardLevelsView.EventState.Reminder);
		this.timerContainer.SetActive(eventState == HardLevelsView.EventState.Started || eventState == HardLevelsView.EventState.Reminder);
		if (eventState == HardLevelsView.EventState.None)
		{
		}
	}

	private void Update()
	{
		if (Time.realtimeSinceStartup - this.lastUpdated > 1f)
		{
			int num = Mathf.Max(0, (int)(this.expirationDate - DateTime.UtcNow).TotalSeconds);
			if (num > 0)
			{
				TimeSpan timeSpan = new TimeSpan(0, 0, num);
				if ((int)timeSpan.TotalDays >= 1)
				{
					this.timerLabel.text = string.Format("{0:D1} {1}", (int)timeSpan.TotalDays + 1, L.Get("Days"));
				}
				else
				{
					this.timerLabel.text = string.Format("{0:D2}:{1:D2}:{2:D2}", (int)timeSpan.TotalHours, timeSpan.Minutes, timeSpan.Seconds);
				}
			}
			this.lastUpdated = Time.realtimeSinceStartup;
		}
	}

	private void DismissClicked(UIEvent e)
	{
		base.Close(0);
	}

	public GameObject startPivot;

	public GameObject completedPivot;

	public GameObject expiredPivot;

	public GameObject reminderPivot;

	public UILabel timerLabel;

	public GameObject timerContainer;

	private DateTime expirationDate;

	private float lastUpdated;

	public enum EventState
	{
		None,
		Started,
		Reminder,
		Completed,
		Expired
	}
}
