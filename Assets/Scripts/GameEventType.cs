using System;

public struct GameEventType
{
	public static implicit operator GameEventType(int val)
	{
		return new GameEventType
		{
			value = val
		};
	}

	public static implicit operator int(GameEventType t)
	{
		return t.value;
	}

	private const int TUTORIAL_BASE = 2000;

	public const int TutorialStarted = 2000;

	public const int SwappedQueue = 1;

	public const int PieceCleared = 2;

	public const int BoosterSelected = 3;

	public const int BoosterUsed = 4;

	public const int MainLevelCompletedFirstTime = 5;

	public const int DailyQuestLevelCompleted = 6;

	public const int BoardReadyForNextMove = 7;

	public const int ShotFired = 8;

	public const int GotPoints = 9;

	public const int SentGateKey = 10;

	public const int SentLife = 11;

	public const int SentTournamentLife = 12;

	public const int ActivatePower = 13;

	public const int ActivateRedPower = 14;

	public const int ActivateGreenPower = 15;

	public const int ActivateBluePower = 16;

	public const int ActivateYellowPower = 17;

	public const int FailedLevelInRow = 18;

	public const int MovesLeftAfterLevel30 = 19;

	public const int TournamentCompleted = 20;

	public const int TotalStarsEarned = 21;

	public const int GotFirstPlaceOnHighscore = 22;

	public const int DeveloperCheated = 23;

	public const int LevelStarted = 24;

	public const int LevelEnded = 25;

	public const int PowerComboUsed = 26;

	public const int WatchedAdForReward = 31;

	public const int FreeBeeUsed = 41;

	public const int FreePowerClaimed = 51;

	public const int BossNewStage = 62;

	private int value;

	private const int TOURNAMENT_BASE = 1000;

	public const int TournamentEnded = 1000;
}
