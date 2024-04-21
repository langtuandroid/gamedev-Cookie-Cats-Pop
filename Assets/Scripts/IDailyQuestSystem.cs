using System;

public interface IDailyQuestSystem
{
	DailyQuestManager Manager { get; }

	DailyQuestFactory Factory { get; }
}
