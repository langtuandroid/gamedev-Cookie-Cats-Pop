using System;
using System.Collections.Generic;

public class DailyQuestChallengeState
{
	public int DaysLeftInChallenge { get; set; }

	public bool HasPendingReward { get; set; }

	public bool HasQuestCooldown { get; set; }

	public int CurrentQuestIndex { get; set; }

	public List<DailyQuestManager.QuestState> QuestStates { get; set; }

	public Dictionary<string, int> FriendProgress { get; set; }
}
