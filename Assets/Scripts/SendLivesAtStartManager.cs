using System;
using Tactile;

public class SendLivesAtStartManager
{
	public SendLivesAtStartManager()
	{
		this.lastTimeSent = this.LastTimeLivesWereSent;
	}

	public bool CanShowSendLivesAtStart()
	{
		int minSecondsBetweenShowSendLivesAtStart = ConfigurationManager.Get<LivesConfig>().MinSecondsBetweenShowSendLivesAtStart;
		if (minSecondsBetweenShowSendLivesAtStart > 0)
		{
			double totalSeconds = (DateTime.UtcNow - this.lastTimeSent).TotalSeconds;
			return totalSeconds > (double)minSecondsBetweenShowSendLivesAtStart;
		}
		return false;
	}

	public void ResetTimer()
	{
		this.lastTimeSent = DateTime.UtcNow;
		this.LastTimeLivesWereSent = this.lastTimeSent;
	}

	private DateTime LastTimeLivesWereSent
	{
		get
		{
			string securedString = TactilePlayerPrefs.GetSecuredString("LastTimeLivesWereSent", string.Empty);
			if (securedString.Length > 0)
			{
				return DateTime.ParseExact(securedString, "yyyy-MM-dd HH:mm:ss", null);
			}
			return DateTime.MinValue;
		}
		set
		{
			TactilePlayerPrefs.SetSecuredString("LastTimeLivesWereSent", value.ToString("yyyy-MM-dd HH:mm:ss"));
		}
	}

	public TimeSpan GetTimeToNextSendLives()
	{
		return this.lastTimeSent + TimeSpan.FromSeconds((double)ConfigurationManager.Get<LivesConfig>().MinSecondsBetweenShowSendLivesAtStart) - DateTime.UtcNow;
	}

	private DateTime lastTimeSent;
}
