using System;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.SagaCore;

public static class DailyQuestSystemBuilder
{
	public static IDailyQuestSystem Build(DailyQuestManager manager, IPlayFlowFactory playFlowFactory, MapFacade mapFacade)
	{
		DailyQuestFactory factory = new DailyQuestFactory(manager, playFlowFactory, mapFacade);
		return new DailyQuestSystem(manager, factory);
	}
}
