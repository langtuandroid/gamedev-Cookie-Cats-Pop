using System;

public abstract class LevelFlowBase : Flow
{
	public LevelFlowBase(LevelProxy levelProxy)
	{
		this.levelToPlay = levelProxy;
	}

	protected LevelProxy levelToPlay;

	public const string LEVEL_TYPE_NORMAL = "normal";

	public const string LEVEL_TYPE_DAILY_QUEST = "daily-quest";

	public const string LEVEL_TYPE_GATE = "gate";

	public const string LEVEL_TYPE_TOURNAMENT = "tournament";

	public const string LEVEL_TYPE_SOCIAL_CHALLENGE = "social-challenge";

	public const string LEVEL_TYPE_TREASURE_HUNT = "treasure-hunt";

	public const string LEVEL_TYPE_ONE_LIFE_CHALLENGE = "one-life-challenge";

	public const string LEVEL_TYPE_SLIDES_AND_LADDERS = "slides-and-ladders";

	public const string LEVEL_TYPE_ENDLESS_CHALLENGE = "endless-challenge";

	public class LevelStartResult
	{
		public bool wantToPlay;
	}
}
