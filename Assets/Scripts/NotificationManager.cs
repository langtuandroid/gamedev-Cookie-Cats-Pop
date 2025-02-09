using System;
using Tactile;
using TactileModules.Foundation;

public class NotificationManager
{
	private NotificationManager()
	{
		this.ScheduleNotifications();
		ActivityManager.onResumeEvent += this.ApplicationWillEnterForeground;
	}

	public bool LocalNotificationsBlocked
	{
		get
		{
			return !LocalNotificationManager.Enabled;
		}
		set
		{
			LocalNotificationManager.Enabled = !value;
		}
	}
	

	private ConfigurationManager ConfigurationManager
	{
		get
		{
			return ManagerRepository.Get<ConfigurationManager>();
		}
	}

	public static NotificationManager Instance { get; private set; }

	public static NotificationManager CreateInstance()
	{
		NotificationManager.Instance = new NotificationManager();
	
		return NotificationManager.Instance;
	}

	private void ApplicationWillEnterForeground()
	{
		this.ScheduleNotifications();
	}
	
	private void HandleVipBonusClaimed()
	{
		this.CancelNotfication(NotificationManager.NotificationType.VipReward);
	}
	
	private void ScheduleNotifications()
	{
		this.ScheduleNotificationsForReturn();
	}



	private void ScheduleNotificationsForReturn()
	{
		DateTime after = DateTime.Today.AddDays(1.0);
		this.ScheduleNotification(NotificationManager.NotificationType.ReturnToGame, after);
	}

	
	private NotificationInfo GetNotificationInfo(NotificationManager.NotificationType notification)
	{
		switch (notification)
		{
		case NotificationManager.NotificationType.DailyQuest3HoursLeft:
			return new NotificationManager.NotificationInfo
			{
				notificationID = "dailyQuest3HoursLeftNotification",
				alertBody = L.Get("Your daily quest expires in 3 hours. Don't miss it!"),
				alertAction = L.Get("play now"),
				dayHour = 21
			};
		case NotificationManager.NotificationType.TournamentEnd:
			return new NotificationManager.NotificationInfo
			{
				notificationID = "tournamentEndNotification",
				alertBody = L.Get("Your tournament is over meow. Come and claim your prize!"),
				alertAction = L.Get("play now"),
				dayHour = 17
			};
		case NotificationManager.NotificationType.TournamentJoinable:
			return new NotificationManager.NotificationInfo
			{
				notificationID = "tournamentJoinable",
				alertBody = L.Get("Mr. Waffles' Cookie Eating Constest is open!"),
				alertAction = L.Get("play now"),
				dayHour = 17
			};
		case NotificationManager.NotificationType.ReturnToGame:
			return new NotificationManager.NotificationInfo
			{
				notificationID = "returnToGame",
				alertBody = this.GetRandomReturnMessage(),
				alertAction = L.Get("play now"),
				dayHour = 21
			};
		case NotificationManager.NotificationType.VipReward:
			return new NotificationManager.NotificationInfo
			{
				notificationID = "returnToGame",
				alertBody = L.Get("You VIC gift is ready! Come and claim it!"),
				alertAction = L.Get("play now"),
				dayHour = 21
			};
		default:
			return null;
		}
	}

	public void ScheduleNotification(NotificationManager.NotificationType type, DateTime after)
	{
		NotificationManager.NotificationInfo notificationInfo = this.GetNotificationInfo(type);
		if (notificationInfo == null)
		{
			return;
		}
		DateTime dateTime = after.Date;
		if (after.Hour >= notificationInfo.dayHour)
		{
			dateTime = dateTime.AddDays(1.0);
		}
		DateTime value = dateTime.AddHours((double)notificationInfo.dayHour).ToUniversalTime();
		LocalNotificationManager.Schedule(notificationInfo.notificationID, new LocalNotificationManager.Notification
		{
			WhenEpochSeconds = value.ToUtcEpoch(),
			AlertBody = notificationInfo.alertBody,
			AlertAction = notificationInfo.alertAction,
			BadgeCount = 1
		});
	}

	public string TryAddGreetingWithName(string message)
	{
		string text = this.TryGetGreetingWithName();
		return (!(text != string.Empty)) ? message : (text + message);
	}

	private string TryGetGreetingWithName()
	{
		return string.Empty;
	}

	public void CancelNotfication(NotificationManager.NotificationType type)
	{
		NotificationManager.NotificationInfo notificationInfo = this.GetNotificationInfo(type);
		LocalNotificationManager.Cancel(notificationInfo.notificationID);
	}

	private string GetRandomReturnMessage()
	{
		Lottery<string> lottery = new Lottery<string>();
		lottery.Add(1f, L.Get("The kittens are hungry again. Come feed them some cookies!"));
		lottery.Add(1f, L.Get("The cats are missing you! Come back and bring some cookies!"));
		lottery.Add(1f, L.Get("The cats are already lining up, waiting for your cookies!"));
		return lottery.PickRandomItem(false);
	}

	public class NotificationInfo
	{
		public string notificationID;

		public string alertBody;

		public string alertAction;

		public int dayHour;
	}

	public enum NotificationType
	{
		DailyQuest3HoursLeft,
		TournamentEnd,
		TournamentJoinable,
		ReturnToGame,
		VipReward
	}
}
