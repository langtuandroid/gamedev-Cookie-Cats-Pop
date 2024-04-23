using System;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.SagaCore;
using UnityEngine;

public class DailyQuestFactory
{
	public DailyQuestFactory(DailyQuestManager dailyQuestManager, IPlayFlowFactory playFlowFactory, MapFacade mapFacade)
	{
		this.playFlowFactory = playFlowFactory;
		this.mapFacade = mapFacade;
		this.dailyQuestManager = dailyQuestManager;
	}

	public DailyQuestMapFlow CreateMapFlow()
	{
		return new DailyQuestMapFlow(this.dailyQuestManager, this.playFlowFactory, this.mapFacade, this);
	}

	public MapAvatar GetMeAvatarPrefab()
	{
		return Resources.Load<MapAvatar>("DailyQuest/DailyQuestPlayerAvatar");
	}

	public MapAvatar GetFriendAvatarPrefab()
	{
		return Resources.Load<MapAvatar>("DailyQuest/DailyQuestFriendAvatar");
	}

	private readonly DailyQuestManager dailyQuestManager;

	private readonly MapFacade mapFacade;

	private readonly IPlayFlowFactory playFlowFactory;
}
