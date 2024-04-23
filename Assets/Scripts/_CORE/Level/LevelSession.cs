using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Tactile;
using TactileModules.Foundation;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGame.ReengagementRewards;
using UnityEngine;

public class LevelSession : ILevelSession
{
	public LevelSession(LevelProxy level)
	{
		this.Level = level;
		this.SessionId = this.CreateSessionID();
		PuzzleLevel puzzleLevel = this.Level.LevelAsset as PuzzleLevel;
		foreach (PuzzleLevel.SpawnInfo spawnInfo in puzzleLevel.spawnInfos)
		{
			PieceId id = spawnInfo.id;
			if (id.MatchFlag != string.Empty)
			{
				this.cachedSpawnColorList.Add(id.MatchFlag);
			}
		}
	}

	public TurnLogic TurnLogic { get; private set; }

	public GameCannon Cannon { get; private set; }

	public GameBallQueue BallQueue { get; private set; }

	public GamePowers Powers { get; private set; }

	public TutorialLogic Tutorial { get; private set; }

	public LevelSessionStats Stats { get; private set; }

	public EndlessChallengeSessionStats EndlessChallengeStats { get; private set; }

	public bool IsTurnResolving { get; private set; }

	public Transform NonMovingEffectRoot { get; private set; }

	public GamePowers.Power PurchasedPower { get; set; }

	public BossLevelController BossLevelController { get; set; }

	public int MovesUsed { get; private set; }

	public int GoodMoves { get; private set; }

	public LevelSessionState SessionState { get; private set; }

	public LevelProxy Level { get; private set; }

	public int Points { get; private set; }

	public int TotalGoalPieces { get; private set; }

	public PieceId SpecialPieceToShoot { get; private set; }

	public List<SelectedBooster> PregameBoosters { get; private set; }

	public Queue<PieceId> FirstShots { get; private set; }

	public bool ShieldActive { get; private set; }

	public bool IsDeathTriggered { get; private set; }

	public LevelSession.SessionID SessionId { get; private set; }

	public int AdjustedOverrideChargeAmount
	{
		get
		{
			LevelAsset levelAsset = this.Level.LevelAsset as LevelAsset;
			return (this.Level.LevelDifficulty != LevelDifficulty.Hard) ? levelAsset.overrideChargeAmount : levelAsset.hardLevelParameters.overrideCharge;
		}
	}

	public int AdjustedMoves
	{
		get
		{
			LevelAsset levelAsset = this.Level.LevelAsset as LevelAsset;
			return (this.Level.LevelDifficulty != LevelDifficulty.Hard) ? levelAsset.moves : levelAsset.hardLevelParameters.moves;
		}
	}

	public int[] AdjustedStarThresholds
	{
		get
		{
			LevelAsset levelAsset = this.Level.LevelAsset as LevelAsset;
			return (this.Level.LevelDifficulty != LevelDifficulty.Hard) ? levelAsset.starThresholds : levelAsset.hardLevelParameters.starThresholds;
		}
	}

	public List<MatchFlag> AdjustedEnabledPowerupColors
	{
		get
		{
			LevelAsset levelAsset = this.Level.LevelAsset as LevelAsset;
			return (this.Level.LevelDifficulty != LevelDifficulty.Hard) ? levelAsset.enabledPowerupColors : levelAsset.hardLevelParameters.enabledPowerupColors;
		}
	}

	public int GoalPiecesCollected
	{
		get
		{
			return this.TotalGoalPieces - this.GetGoalPiecesRemaining();
		}
	}

	public bool FirstTimeCompleted
	{
		get
		{
			return !this.wasCompletedBefore && this.SessionState == LevelSessionState.Completed;
		}
	}

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action SpecialShotLoaded;

	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<LevelSession> StateChanged = delegate (LevelSession A_0)
    {
    };



    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action ShotFired = delegate ()
    {
    };



    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action Cheat = delegate ()
    {
    };



    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action ShieldActivated = delegate ()
    {
    };



    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action ShieldDeactivated = delegate ()
    {
    };



    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action<LevelSession.ShieldModification> ShieldModified = delegate (LevelSession.ShieldModification A_0)
    {
    };



    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action<LevelSession> Ended = delegate (LevelSession A_0)
    {
    };



    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action NextMovePrepared = delegate ()
    {
    };



    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action<int> PointsChanged = delegate (int A_0)
    {
    };



    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action InGameBoosterUsed = delegate ()
    {
    };



    public void SetPreGameBoosters(List<SelectedBooster> startBoosters)
	{
		this.PregameBoosters = new List<SelectedBooster>(startBoosters);
		ReengagementRewardManager reengagementRewardManager = ManagerRepository.Get<ReengagementRewardManager>();
		if (reengagementRewardManager.IsActiveOnLevel(this.Level.Index))
		{
			this.PregameBoosters.Add(new SelectedBooster
			{
				id = "BoosterExtraMoves",
				isFree = true
			});
		}
		this.Powers = new GamePowers();
		this.Powers.Initialize(this);
		this.SpecialPieceToShoot = PieceId.Empty;
	}

	public void Initialize(GameBoard board, GameCannon cannon, Transform nonMovingEffectRoot)
	{
		this.TurnLogic = new TurnLogic(this);
		this.Tutorial = new TutorialLogic();
		this.Tutorial.Initialize(this);
		this.BallQueue = new GameBallQueue();
		this.BallQueue.Initialize(this.AdjustedMoves, this);
		this.NonMovingEffectRoot = nonMovingEffectRoot;
		this.TurnLogic.Board = board;
		this.TurnLogic.PieceCleared += this.HandlePieceCleared;
		this.TurnLogic.TurnCompleted += this.HandleTurnCompleted;
		this.TurnLogic.PanningToBottomCompleted += this.PanningToBottomCompleted;
		board.BuildLevel();
		this.TotalGoalPieces = this.GetGoalPiecesRemaining();
		this.wasCompletedBefore = this.Level.IsCompleted;
		this.FirstShots = new Queue<PieceId>();
		LevelAsset levelAsset = this.Level.LevelAsset as LevelAsset;
		foreach (PieceId item in levelAsset.firstPieces)
		{
			this.FirstShots.Enqueue(item);
		}
		this.LoadNextShot();
		this.LoadNextShot();
		this.Cannon = cannon;
		this.Cannon.Initialize(board, (AimingState aim) => aim.Main.IsValidForShot);
		this.Cannon.Shoot += delegate(AimingState aim)
		{
			if (!this.IsAimValid(aim))
			{
				return;
			}
			if (this.IsTurnResolving)
			{
				return;
			}
			this.IsTurnResolving = true;
			this.Cannon.InputEnabled += false;
			this.BallQueue.SwappingEnabled += false;
			this.TurnLogic.Shoot(aim.ValidAimRays, delegate
			{
				this.Cannon.InputEnabled += true;
				this.BallQueue.SwappingEnabled += true;
				this.IsTurnResolving = false;
			});
		};
		this.Cannon.Swapped += delegate()
		{
			this.BallQueue.SwapQueuedPieces();
		};
		GamePowers powers = this.Powers;
		powers.PowerActivated = (Action<GamePowers.Power>)Delegate.Combine(powers.PowerActivated, new Action<GamePowers.Power>(delegate(GamePowers.Power p)
		{
			this.LoadSpecialShot(this.Powers.GetPowerPiece(), false);
			if (this.Powers.UseThreewayShot)
			{
				cannon.AddAim(-16.18034f);
				cannon.AddAim(16.18034f);
			}
		}));
		this.Cannon.SuperAimActivated += delegate()
		{
			this.superAimPieceHighlighter = new SuperAimPieceHighlighter(this);
		};
		this.Cannon.SuperAimDeactivated += delegate()
		{
			if (this.superAimPieceHighlighter != null)
			{
				this.superAimPieceHighlighter.StopSuperAim();
			}
		};
		InventoryManager.Instance.InventoryChanged += this.HandleInventoryChanged;
		this.Stats = new LevelSessionStats(this);
		this.EndlessChallengeStats = new EndlessChallengeSessionStats(this);
		this.TurnLogic.Initialize();
		this.BossLevelController = new BossLevelController(this);
	}

	private void HandleInventoryChanged(InventoryManager.ItemChangeInfo info)
	{
		if (this.Cannon == null)
		{
			return;
		}
		if (info.ChangeByAmount < 0 && BoosterManagerBase<BoosterManager>.Instance.IsInventoryItemABooster(info.Item))
		{
			List<InventoryItem> list = new List<InventoryItem>
			{
				"BoosterRainbow",
				"BoosterFinalPower"
			};
			if (list.Contains(info.Item))
			{
				this.InGameBoosterUsed();
			}
		}
	}

	private LevelSession.SessionID CreateSessionID()
	{
		this.SessionId = new LevelSession.SessionID
		{
			id = Guid.NewGuid().ToString()
		};
		return this.SessionId;
	}

	public void Destroy()
	{
		this.TurnLogic.Destroy();
		this.Tutorial.Destroy();
		if (this.Level.LevelAsset is BossLevel)
		{
			this.BossLevelController.End();
		}
	}

	private bool IsAimValid(AimingState aim)
	{
		if (this.Tutorial != null && this.Tutorial.CurrentStep != null)
		{
			TutorialStep tutorialStep = (TutorialStep)this.Tutorial.CurrentStep;
			if (tutorialStep.dismissType == TutorialStep.DismissType.SwapQueue || tutorialStep.dismissType == TutorialStep.DismissType.UseBooster || tutorialStep.dismissType == TutorialStep.DismissType.UsePower || tutorialStep.dismissType == TutorialStep.DismissType.WaitForFreePowerClaimed)
			{
				return false;
			}
			if (tutorialStep.activeTiles.Count > 0)
			{
				Tile a;
				Trajectory.CalculateTrajectoryHits(this.TurnLogic.Board, aim.Main.aimOriginInBoardSpace, aim.Main.direction, out a);
				if (a != Tile.Invalid)
				{
					List<int> activeTiles = tutorialStep.activeTiles;
					bool flag = activeTiles.Contains(a.Index);
					foreach (Tile tile in a.GetNeighbours())
					{
						if (activeTiles.Contains(tile.Index) && tile.Piece == null)
						{
							flag |= true;
							break;
						}
					}
					if (!flag)
					{
						return false;
					}
				}
			}
		}
		return aim.Main.IsValidForShot;
	}

	public List<InventoryItem> GetAvailableIngameBoostersForThisLevel()
	{
		List<InventoryItem> list = new List<InventoryItem>();
		int atMainLevelIndex = (!(this.Level.LevelCollection is MainLevelDatabase)) ? MainProgressionManager.Instance.GetFarthestUnlockedLevelIndex() : this.Level.Index;
		List<InventoryItem> list2 = new List<InventoryItem>
		{
			"BoosterRainbow",
			"BoosterFinalPower"
		};
		foreach (InventoryItem inventoryItem in list2)
		{
			if (BoosterManagerBase<BoosterManager>.Instance.IsBoosterUnlocked(inventoryItem, atMainLevelIndex) || this.BoosterNeededInTutorial(inventoryItem))
			{
				list.Add(inventoryItem);
			}
		}
		return list;
	}

	public void CheatComplete(bool success, int pointsToAdd = 300)
	{
		if (success)
		{
			this.AddPoints(pointsToAdd);
			this.SetState(LevelSessionState.ReadyForAftermath);
		}
		else
		{
			this.BallQueue.StealBalls(this.BallQueue.BallsLeft - 1);
			this.Cheat();
		}
	}

	private bool BoosterNeededInTutorial(InventoryItem boosterId)
	{
		foreach (ITutorialStep tutorialStep in ((ILevelSession)this).GetTutorialSteps())
		{
			TutorialStep tutorialStep2 = (TutorialStep)tutorialStep;
			if (tutorialStep2.useBoosterType == boosterId)
			{
				return true;
			}
		}
		return false;
	}

	private IEnumerator PanningToBottomCompleted()
	{
		if (this.Level.LevelAsset is EndlessLevel)
		{
			EndlessChallengeGameBoard endlessGameBoard = this.TurnLogic.Board as EndlessChallengeGameBoard;
			if (endlessGameBoard.ShouldSpawnEndlessBlocks())
			{
				while (this.TurnLogic.FloatersAreAnimating)
				{
					yield return null;
				}
				endlessGameBoard.SpawnEndlessBlocks();
			}
			yield return endlessGameBoard.CollectAllCheckPoints(this);
		}
		yield break;
	}

	private void HandleTurnCompleted()
	{
		int num = this.GetGoalPiecesRemaining();
		if (this.Level.LevelAsset is EndlessLevel || this.Level.LevelAsset is BossLevel)
		{
			num = int.MaxValue;
		}
		bool flag = false;
		bool flag2 = false;
		if (this.Level.LevelAsset is BossLevel)
		{
			BossLevelGameBoard bossLevelGameBoard = this.TurnLogic.Board as BossLevelGameBoard;
			flag = bossLevelGameBoard.IsChangingStages;
			flag2 = (bossLevelGameBoard.BossStagesLeft == 0);
		}
		if (num > 0 && this.IsDeathTriggered)
		{
			this.SetState(LevelSessionState.DeathTriggered);
		}
		else if (num > 0 && this.BallQueue.BallsLeft <= 0 && !flag)
		{
			this.SetState(LevelSessionState.NoMoreMoves);
		}
		else if (num == 0)
		{
			this.SetState(LevelSessionState.ReadyForAftermath);
		}
		else if (!flag2)
		{
			this.PrepareForNextShot();
			this.LoadNextShot();
			this.NextMovePrepared();
		}
	}

	private void HandlePieceCleared(CPPiece piece, int pointsToGive, HitMark hit)
	{
		if (pointsToGive > 0)
		{
			int points = pointsToGive * this.TurnLogic.StreakMultiplier;
			this.AddPoints(points);
		}
		if (hit.cause == HitCause.Unknown || hit.cause == HitCause.Cluster)
		{
			this.Powers.CollectPieceForCharging(piece);
		}
		if (hit.cause != HitCause.Unknown)
		{
			if (!this.ShieldActive && piece is DeathPiece)
			{
				this.IsDeathTriggered = true;
			}
			else if (!this.ShieldActive && piece is MinusPiece)
			{
				this.BallQueue.StealBalls(2);
			}
			else if (piece is PlusPiece)
			{
				this.Stats.MovesAddedByGamePiece += 2;
				this.BallQueue.ModifyBallsLeft(2, true);
			}
			else if (piece is FillPowerPiece)
			{
				this.Powers.FillInstantly(piece as FillPowerPiece);
			}
		}
	}

	public void UseShot()
	{
		this.MovesUsed++;
		this.BallQueue.ConsumeTopBall();
		this.Cannon.ClearAims();
		this.ShotFired();
	}

	public void IncrementGoodMoves()
	{
		this.GoodMoves++;
	}

	public void PrepareForNextShot()
	{
		this.Powers.Reset();
		this.Powers.ResetOverrideCombination();
	}

	public void LoadSpecialShot(PieceId wantedId, bool clearPowers = true)
	{
		this.Cannon.ClearAims();
		if (clearPowers)
		{
			this.Powers.Reset();
		}
		this.SpecialPieceToShoot = wantedId;
		if (this.SpecialShotLoaded != null)
		{
			this.SpecialShotLoaded();
		}
	}

	public void ResetDeathTriggeredAfterPurchaseAndResume()
	{
		this.IsDeathTriggered = false;
	}

	public int GetGoalPiecesRemaining()
	{
		int result;
		if (this.Level.LevelAsset is EndlessLevel)
		{
			result = int.MaxValue;
		}
		else if (this.Level.LevelAsset is BossLevel)
		{
			BossLevelGameBoard bossLevelGameBoard = this.TurnLogic.Board as BossLevelGameBoard;
			result = bossLevelGameBoard.BossStagesLeft;
		}
		else
		{
			result = this.TurnLogic.Board.CountAllPiecesWithClass<GoalPiece>();
		}
		return result;
	}

	private void LoadNextShot()
	{
		this.BallQueue.PutInQueue();
		this.SpecialPieceToShoot = PieceId.Empty;
	}

	public void SetState(LevelSessionState newState)
	{
		this.SessionState = newState;
		if (this.SessionState == LevelSessionState.Abandoned || this.SessionState == LevelSessionState.Completed || this.SessionState == LevelSessionState.NoMoreMoves || this.SessionState == LevelSessionState.Failed || this.SessionState == LevelSessionState.DeathTriggered)
		{
			this.Stats.MarkEndValues();
		}
		this.StateChanged(this);
		switch (this.SessionState)
		{
		case LevelSessionState.Playing:
			this.Cannon.InputEnabled += true;
			this.BallQueue.SwappingEnabled += true;
			this.HandleTurnCompleted();
			break;
		case LevelSessionState.NoMoreMoves:
			this.Cannon.InputEnabled += false;
			this.BallQueue.SwappingEnabled += false;
			break;
		case LevelSessionState.ReadyForAftermath:
			this.Stats.MarkEndValuesPreAftermath();
			this.Cannon.InputEnabled += false;
			this.BallQueue.SwappingEnabled += false;
			break;
		case LevelSessionState.Failed:
			this.DoEnd(this);
			break;
		case LevelSessionState.Abandoned:
			this.DoEnd(this);
			break;
		case LevelSessionState.Completed:
			this.Points = Mathf.Max(this.Level.StarThresholds[0], this.Points);
			this.DoEnd(this);
			break;
		case LevelSessionState.DeathTriggered:
			this.Cannon.InputEnabled += false;
			this.BallQueue.SwappingEnabled += false;
			break;
		}
	}

	private void DoEnd(LevelSession session)
	{
		this.Ended(session);
		InventoryManager.Instance.InventoryChanged -= this.HandleInventoryChanged;
		int index = session.Level.Index;
		if (index != LevelSession.PersistedLastFailedLevelId)
		{
			LevelSession.PersistedLastFailedLevelId = index;
			LevelSession.PersistedFailsForSameLevel = 0;
		}
		if (this.Stats != null)
		{
			this.Stats.End();
		}
		LevelSession.PersistedFailsForSameLevel++;
	}

	public PieceId GetRandomSpawnPieceId()
	{
		Lottery<PieceId> lottery = new Lottery<PieceId>();
		PuzzleLevel puzzleLevel = this.Level.LevelAsset as PuzzleLevel;
		foreach (PuzzleLevel.SpawnInfo spawnInfo in puzzleLevel.spawnInfos)
		{
			lottery.Add(spawnInfo.chance, spawnInfo.id);
		}
		return lottery.PickRandomItem(false);
	}

	public List<MatchFlag> GetSpawnColors(Func<MatchFlag, bool> predicate = null)
	{
		List<MatchFlag> list = new List<MatchFlag>(this.cachedSpawnColorList);
		if (predicate != null)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (!predicate(list[i]))
				{
					list.RemoveAt(i);
					i--;
				}
			}
		}
		return list;
	}

	public void AddPoints(int points)
	{
		this.Points += points;
		this.PointsChanged(points);
	}

	public PieceId GetNextPieceToShoot()
	{
		return (!(this.SpecialPieceToShoot == PieceId.Empty)) ? this.SpecialPieceToShoot : this.BallQueue.NextPieceToShoot;
	}

	IEnumerable<ITutorialStep> ILevelSession.GetTutorialSteps()
	{
		return ((LevelAsset)this.Level.LevelAsset).TutorialSteps;
	}

	public void ActivateShield()
	{
		this.ShieldActive = true;
		this.ShieldActivated();
	}

	public void DeactivateShield()
	{
		this.ShieldActive = false;
		this.ShieldDeactivated();
	}

	public void ModifyShield(LevelSession.ShieldModification modification)
	{
		this.ShieldModified(modification);
	}

	public int GetContinuousFailsForThisLevel()
	{
		if (this.Level.Index != LevelSession.PersistedLastFailedLevelId)
		{
			return 0;
		}
		return LevelSession.PersistedFailsForSameLevel;
	}

	private static int PersistedFailsForSameLevel
	{
		get
		{
			return TactilePlayerPrefs.GetInt("gameSessionFailsForSameLevel", 0);
		}
		set
		{
			TactilePlayerPrefs.SetInt("gameSessionFailsForSameLevel", value);
		}
	}

	private static int PersistedLastFailedLevelId
	{
		get
		{
			return TactilePlayerPrefs.GetInt("gameSessionLastFailedLevelId", -1);
		}
		set
		{
			TactilePlayerPrefs.SetInt("gameSessionLastFailedLevelId", value);
		}
	}

	private const string PREFS_GAMESESSION_FAILS_FOR_SAME_LEVEL = "gameSessionFailsForSameLevel";

	private const string PREFS_GAMESESSION_LAST_FAILED_LEVEL_ID = "gameSessionLastFailedLevelId";

	private SuperAimPieceHighlighter superAimPieceHighlighter;

	private bool wasCompletedBefore;

	private readonly HashSet<MatchFlag> cachedSpawnColorList = new HashSet<MatchFlag>();

	public enum ShieldModification
	{
		Blink
	}

	public class SessionID
	{
		public string id;
	}
}
