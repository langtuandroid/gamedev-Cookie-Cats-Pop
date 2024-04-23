using System;

public class TournamentUI : DialogViewProvider, TournamentManager.ITournamentUIProvider
{
	void TournamentManager.ITournamentUIProvider.ScheduleNotificationWhenTournamentIsJoinable(DateTime utcEndTime)
	{
		NotificationManager.Instance.ScheduleNotification(NotificationManager.NotificationType.TournamentJoinable, utcEndTime);
	}

	public static string GetRankAsString(TournamentRank rank)
	{
		return SingletonAsset<TournamentSetup>.Instance.GetRankSetup(rank).displayName;
	}
}
