using System;
using System.Collections.Generic;
using Tactile;

[ConfigProvider("DailyQuestConfig")]
public class DailyQuestConfig
{
	[JsonSerializable("InUse", null)]
	public bool InUse { get; set; }

	[JsonSerializable("LevelRequiredForDailyQuests", null)]
	public int LevelRequiredForDailyQuests { get; set; }

	[JsonSerializable("DailyQuestSkipPrice", null)]
	public int DailyQuestSkipPrice { get; set; }

	[JsonSerializable("DailyQuests", typeof(DailyQuestInfo))]
	public List<DailyQuestInfo> Quests { get; set; }

	[JsonSerializable("DaysToEndNotification", null)]
	public int DaysToEndNotification { get; set; }

	[JsonSerializable("HeadStartAmountInDays", null)]
	public int HeadStartAmountInDays { get; set; }
}
