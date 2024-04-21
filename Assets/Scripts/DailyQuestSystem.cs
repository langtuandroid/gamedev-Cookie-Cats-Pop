using System;

public class DailyQuestSystem : IDailyQuestSystem
{
	public DailyQuestSystem(DailyQuestManager manager, DailyQuestFactory factory)
	{
		this.Manager = manager;
		this.Factory = factory;
	}

	public DailyQuestManager Manager { get; set; }

	public DailyQuestFactory Factory { get; set; }
}
