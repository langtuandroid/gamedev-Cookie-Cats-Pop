using System;
using System.Collections.Generic;
using System.Diagnostics;

public class GameBallQueue
{


    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action BallsLeftChanged=()=> { };

    public PieceId NextPieceToShoot { get; private set; }

	public PieceId QueuedPiece { get; private set; }

	public PieceId SecondQueuedPiece { get; private set; }
    private int ballsLeft;
    public int BallsLeft
	{
		get
		{
			return this.ballsLeft;
		}
		protected set
		{
            this.ballsLeft = value;
            this.BallsLeftChanged();

        }
    }

	public bool HasTripleQueue { get; private set; }

	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action QueueSwapped = delegate ()
    {
    };



    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action TripleQueueActivated = delegate ()
    {
    };



    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action TripleQueueDeactivated = delegate ()
    {
    };



    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action<GameBallQueue.TripleQueueModification> TripleQueueModified = delegate (GameBallQueue.TripleQueueModification A_0)
    {
    };



    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action<bool> ExtraBallsGiven = delegate (bool A_0)
    {
    };







    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action NonMatchingBallChanged = delegate ()
    {
    };



    public void Initialize(int startWithNumberOfBalls, LevelSession levelSession)
	{
		this.levelSession = levelSession;
		this.BallsLeft = startWithNumberOfBalls;
		levelSession.TurnLogic.PieceCleared += this.HandlePlusPieces;
		this.allMatchFlags.Add("Blue");
		this.allMatchFlags.Add("Green");
		this.allMatchFlags.Add("Red");
		this.allMatchFlags.Add("Yellow");
		this.allMatchFlags.Add("Purple");
		this.allMatchFlags.Add(string.Empty);
		foreach (MatchFlag key in this.allMatchFlags)
		{
			this.spawnChanceModifiers[key] = 1f;
		}
	}

	public void ActivateTripleQueue()
	{
		this.HasTripleQueue = true;
		if (this.BallsLeft > 1)
		{
			this.SecondQueuedPiece = this.ProvideNewBallToQueue();
		}
		this.TripleQueueActivated();
	}

	public void DeactivateTripleQueue()
	{
		Action destroyBall = null;
		destroyBall = delegate()
		{
			this.HasTripleQueue = false;
			this.SecondQueuedPiece = PieceId.Empty;
			this.TripleQueueDeactivated();
			this.levelSession.NextMovePrepared -= destroyBall;
		};
		this.levelSession.NextMovePrepared += destroyBall;
	}

	public void ModifyTripleQueue(GameBallQueue.TripleQueueModification modification)
	{
		this.TripleQueueModified(modification);
	}

	public void SwapQueuedPieces()
	{
		if (!this.SwappingEnabled)
		{
			return;
		}
		if (this.BallsLeft < 2)
		{
			return;
		}
		if (this.HasTripleQueue)
		{
			if (this.BallsLeft > 2)
			{
				PieceId nextPieceToShoot = this.NextPieceToShoot;
				this.NextPieceToShoot = this.QueuedPiece;
				this.QueuedPiece = this.SecondQueuedPiece;
				this.SecondQueuedPiece = nextPieceToShoot;
			}
			else
			{
				PieceId nextPieceToShoot2 = this.NextPieceToShoot;
				this.NextPieceToShoot = this.QueuedPiece;
				this.QueuedPiece = nextPieceToShoot2;
			}
		}
		else
		{
			PieceId queuedPiece = this.QueuedPiece;
			this.QueuedPiece = this.NextPieceToShoot;
			this.NextPieceToShoot = queuedPiece;
		}
		this.QueueSwapped();
	}

	public void StealBalls(int amount)
	{
		this.BallsLeft -= amount;
	}

	public void ConsumeTopBall()
	{
		this.BallsLeft--;
	}

	public void ModifyBallsLeft(int amount, bool waitForResult = true)
	{
		this.BallsLeft += amount;
		this.FillQueue();
		this.ExtraBallsGiven(waitForResult);
	}

	public void PutInQueue()
	{
		if (this.BallsLeft <= 0)
		{
			this.NextPieceToShoot = PieceId.Empty;
			return;
		}
		this.NextPieceToShoot = this.QueuedPiece;
		if (this.BallsLeft == 1)
		{
			this.QueuedPiece = PieceId.Empty;
		}
		else if (this.HasTripleQueue)
		{
			if (this.BallsLeft > 2)
			{
				this.QueuedPiece = this.SecondQueuedPiece;
				this.SecondQueuedPiece = this.ProvideNewBallToQueue();
			}
			else
			{
				this.SecondQueuedPiece = PieceId.Empty;
			}
		}
		else if (this.BallsLeft > 1)
		{
			this.QueuedPiece = this.ProvideNewBallToQueue();
		}
		this.ForceQueueToHaveRelevantColors();
	}

	private void HandlePlusPieces(CPPiece piece, int pointsToGive, HitMark hit)
	{
		if (hit.cause != HitCause.Unknown && piece is PlusPiece)
		{
			this.FillQueue();
		}
	}

	private void FillQueue()
	{
		int num = this.BallsLeft;
		if (this.NextPieceToShoot.IsEmpty && num > 1)
		{
			this.NextPieceToShoot = this.ProvideNewBallToQueue();
			num--;
		}
		if (this.QueuedPiece.IsEmpty && num > 1)
		{
			this.QueuedPiece = this.ProvideNewBallToQueue();
			num--;
		}
		if (this.SecondQueuedPiece.IsEmpty && num > 1)
		{
			this.SecondQueuedPiece = this.ProvideNewBallToQueue();
			num--;
		}
	}

	private PieceId GetRandomIdMatchingExistingPieces()
	{
		List<MatchFlag> colorsAmongPieces = this.levelSession.TurnLogic.Board.GetColorsAmongPieces(true);
		Lottery<PieceId> lottery = new Lottery<PieceId>();
		PuzzleLevel puzzleLevel = this.levelSession.Level.LevelAsset as PuzzleLevel;
		foreach (PuzzleLevel.SpawnInfo spawnInfo in puzzleLevel.spawnInfos)
		{
			PieceId id = spawnInfo.id;
			if (colorsAmongPieces.Contains(id.MatchFlag))
			{
				float weightedChance = spawnInfo.chance * this.spawnChanceModifiers[id.MatchFlag];
				lottery.Add(weightedChance, spawnInfo.id);
			}
		}
		return lottery.PickRandomItem(false);
	}

	private void ForceQueueToHaveRelevantColors()
	{
		List<MatchFlag> colorsAmongPieces = this.levelSession.TurnLogic.Board.GetColorsAmongPieces(false);
		PieceId pieceId = this.ForceRelevantColor(this.NextPieceToShoot, colorsAmongPieces);
		if (pieceId != this.NextPieceToShoot)
		{
			this.NonMatchingBallChanged();
		}
		this.NextPieceToShoot = pieceId;
		this.QueuedPiece = this.ForceRelevantColor(this.QueuedPiece, colorsAmongPieces);
		this.SecondQueuedPiece = this.ForceRelevantColor(this.SecondQueuedPiece, colorsAmongPieces);
	}

	private PieceId ForceRelevantColor(PieceId id, List<MatchFlag> colorsOnBoard)
	{
		if (id.IsEmpty || id.MatchFlag == string.Empty)
		{
			return id;
		}
		if (colorsOnBoard.Contains(id.MatchFlag))
		{
			return id;
		}
		if (colorsOnBoard.Count == 0)
		{
			return id;
		}
		return new PieceId(id.TypeName, colorsOnBoard.GetRandom<MatchFlag>());
	}

	private PieceId ProvideNewBallToQueue()
	{
		LevelAsset levelAsset = this.levelSession.Level.LevelAsset as LevelAsset;
		Queue<PieceId> firstShots = this.levelSession.FirstShots;
		PieceId pieceId = PieceId.Empty;
		if (firstShots.Count > 0)
		{
			pieceId = firstShots.Dequeue();
			for (int i = 0; i < LevelAsset.RANDOM_GROUPS.Length; i++)
			{
				if (pieceId.MatchFlag == LevelAsset.RANDOM_GROUPS[i])
				{
					pieceId = new PieceId(pieceId.TypeName, this.levelSession.TurnLogic.Board.ColorGroups[i]);
					break;
				}
			}
			if (pieceId.MatchFlag == "?")
			{
				pieceId = this.levelSession.GetRandomSpawnPieceId();
			}
		}
		else if (levelAsset.spawnExistingColorsOnly)
		{
			PieceId randomIdMatchingExistingPieces = this.GetRandomIdMatchingExistingPieces();
			if (randomIdMatchingExistingPieces != PieceId.Empty)
			{
				pieceId = randomIdMatchingExistingPieces;
			}
		}
		if (pieceId == PieceId.Empty)
		{
			pieceId = this.levelSession.GetRandomSpawnPieceId();
		}
		foreach (MatchFlag matchFlag in this.allMatchFlags)
		{
			if (this.spawnChanceModifiers[matchFlag] < 10000f)
			{
				Dictionary<MatchFlag, float> dictionary;
				MatchFlag key;
				(dictionary = this.spawnChanceModifiers)[key = matchFlag] = dictionary[key] * 1.618034f;
			}
		}
		this.spawnChanceModifiers[pieceId.MatchFlag] = 1f;
		string text = "Chances:";
		foreach (KeyValuePair<MatchFlag, float> keyValuePair in this.spawnChanceModifiers)
		{
			string text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				keyValuePair.Key,
				":",
				keyValuePair.Value,
				" | "
			});
		}
		return pieceId;
	}

	private const float INCREASING_CHANCE_FOR_LRU_COLORS = 1.618034f;

	private const float MAX_CHANCE_FOR_LRU_COLORS = 10000f;

	public NestedEnabler SwappingEnabled;

	private Dictionary<MatchFlag, float> spawnChanceModifiers = new Dictionary<MatchFlag, float>();

	private LevelSession levelSession;

	

	private List<MatchFlag> allMatchFlags = new List<MatchFlag>();

	public enum TripleQueueModification
	{
		Blink
	}
}
