using System;
using TactileModules.PuzzleGame.MainLevels;

public class AchievementsLevelSessionListener
{
	public void Initialize(LevelSession levelSession)
	{
		this.Powers(levelSession);
		levelSession.BallQueue.QueueSwapped += delegate()
		{
			GameEventManager.Instance.Emit(1);
		};
		levelSession.TurnLogic.PieceCleared += delegate(CPPiece piece, int points, HitMark hitMark)
		{
			GameEventManager.Instance.Emit(2, piece, 1);
		};
		levelSession.StateChanged += this.LevelSession_StateChanged;
		levelSession.TurnLogic.TurnCompleted += delegate()
		{
			GameEventManager.Instance.Emit(7);
		};
		levelSession.ShotFired += delegate()
		{
			GameEventManager.Instance.Emit(8);
		};
		levelSession.PointsChanged += delegate(int delta)
		{
			GameEventManager.Instance.Emit(9, null, levelSession.Points);
		};
		if (levelSession.Level.HumanNumber > 30)
		{
			levelSession.StateChanged += this.ReadyForAftermathAfterLevelThirty;
		}
	}

	private void Powers(LevelSession levelSession)
	{
		GamePowers powers = levelSession.Powers;
		powers.PowerActivated = (Action<GamePowers.Power>)Delegate.Combine(powers.PowerActivated, new Action<GamePowers.Power>(delegate(GamePowers.Power power)
		{
			GameEventManager.Instance.Emit(13, power.Color, 1);
			if (power.Color == "Red")
			{
				GameEventManager.Instance.Emit(14, power.Color, 1);
			}
			else if (power.Color == "Green")
			{
				GameEventManager.Instance.Emit(15, power.Color, 1);
			}
			else if (power.Color == "Blue")
			{
				GameEventManager.Instance.Emit(16, power.Color, 1);
			}
			else if (power.Color == "Yellow")
			{
				GameEventManager.Instance.Emit(17, power.Color, 1);
			}
		}));
	}

	private void ReadyForAftermathAfterLevelThirty(LevelSession levelSession)
	{
		if (levelSession.SessionState == LevelSessionState.ReadyForAftermath)
		{
			GameEventManager.Instance.Emit(19, null, levelSession.BallQueue.BallsLeft);
		}
	}

	private void LevelSession_StateChanged(LevelSession levelSession)
	{
		if (levelSession.SessionState == LevelSessionState.Completed)
		{
			if (levelSession.Level.LevelCollection is MainLevelDatabase)
			{
				if (levelSession.FirstTimeCompleted)
				{
					GameEventManager.Instance.Emit(5, levelSession.Level, levelSession.Level.HumanNumber);
				}
				int totalEarnedStars = MainProgressionManager.Instance.GetTotalEarnedStars();
				GameEventManager.Instance.Emit(21, null, totalEarnedStars);
			}
			this.ResetLevelFailCounter();
		}
		else if (levelSession.SessionState == LevelSessionState.Failed)
		{
			this.TryEmitLevelFailed(levelSession);
		}
	}

	private void ResetLevelFailCounter()
	{
		AchievementsLevelSessionListener.PersistedLastFailedLevelId = -1;
		AchievementsLevelSessionListener.PersistedFailsForSameLevel = 0;
	}

	private void TryEmitLevelFailed(LevelSession levelSession)
	{
		if (levelSession.Level.GetHashCode() != AchievementsLevelSessionListener.PersistedLastFailedLevelId)
		{
			AchievementsLevelSessionListener.PersistedLastFailedLevelId = levelSession.Level.GetHashCode();
			AchievementsLevelSessionListener.PersistedFailsForSameLevel = 0;
		}
		AchievementsLevelSessionListener.PersistedFailsForSameLevel++;
		GameEventManager.Instance.Emit(18, levelSession.Level, AchievementsLevelSessionListener.PersistedFailsForSameLevel);
	}

	public static int PersistedFailsForSameLevel
	{
		get
		{
			return TactilePlayerPrefs.GetInt("AchievementsFailsForSameLevel", 0);
		}
		private set
		{
			TactilePlayerPrefs.SetInt("AchievementsFailsForSameLevel", value);
		}
	}

	private static int PersistedLastFailedLevelId
	{
		get
		{
			return TactilePlayerPrefs.GetInt("AchievementsFailedLevelId", -1);
		}
		set
		{
			TactilePlayerPrefs.SetInt("AchievementsFailedLevelId", value);
		}
	}

	private const string PREFS_GAMESESSION_FAILS_FOR_SAME_LEVEL = "AchievementsFailsForSameLevel";

	private const string PREFS_GAMESESSION_LAST_FAILED_LEVEL_ID = "AchievementsFailedLevelId";
}
