using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Fibers;
using UnityEngine;

public class TurnLogic
{
	public TurnLogic(LevelSession session)
	{
		this.session = session;
		this.StreakMultiplier = 1;
		this.logicModules.Add(new MorphModule());
		this.logicModules.Add(new BonusDropModule());
		this.logicModules.Add(new BonusCollectModule());
		this.logicModules.Add(new BoosterPieceSpawnModule());
		this.logicModules.Add(new BoosterPieceCollectModule());
		this.logicModules.Add(new SquidModule());
		this.logicModules.Add(new SeagullModule());
	}

	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<TurnLogic.Shot[], ResolveState> ShotFired = delegate (TurnLogic.Shot[] A_0, ResolveState A_1)
    {
    };



    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action TurnCompleted = delegate ()
    {
    };



    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action<TurnLogic> ResolveCompleted = delegate (TurnLogic A_0)
    {
    };



    //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event TurnLogic.PanningToBottomCompletedDelegate PanningToBottomCompleted;

	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<CPPiece, int, HitMark> PieceCleared = delegate (CPPiece A_0, int A_1, HitMark A_2)
    {
    };



    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action<CPPiece> PieceDiscarded = delegate (CPPiece A_0)
    {
    };




    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action HitsMarked = delegate ()
    {
    };



    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action<CPPiece, Action<bool>> ShotMovementStarted = delegate (CPPiece A_0, Action<bool> A_1)
    {
    };



    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action<CPPiece> ShotMovementEnded = delegate (CPPiece A_0)
    {
    };




    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action<SquidPiece> ForceSquidRetreat = delegate (SquidPiece A_0)
    {
    };



    public GameBoard Board { get; set; }

	public int StreakMultiplier { get; private set; }

	public bool FloatersAreAnimating
	{
		get
		{
			return !this.floatersLogic.IsDone();
		}
	}

	private bool PowersWasResolvedThisTurn { get; set; }

	public bool IsFibersTerminated
	{
		get
		{
			return this.resolveFiber.IsTerminated && this.floatersLogic.IsDone();
		}
	}

	public void Initialize()
	{
		foreach (LogicModule logicModule in this.logicModules)
		{
			logicModule.Begin(this.session);
		}
	}

	public void Destroy()
	{
		foreach (LogicModule logicModule in this.logicModules)
		{
			logicModule.End(this.session);
		}
		this.resolveFiber.Terminate();
		this.floatersLogic.Terminate();
	}

	public void Shoot(Ray2D[] aims, Action finished)
	{
		this.resolveFiber.Start(this.ResolveShoot(aims, finished));
	}

	public void BossPoppedSquid(SquidPiece squid)
	{
		this.ForceSquidRetreat(squid);
	}

	private IEnumerator AnimateShot(TurnLogic.Shot shot, PieceId pieceToShoot, ResolveState resolveState)
	{
		Tile hitTile = shot.hitTile;
		List<Vector2> collisionPoints = shot.collisionPoints;
		Vector2 lastCollision = collisionPoints[collisionPoints.Count - 1];
		CPPiece newPiece = this.Board.SpawnPiece(pieceToShoot) as CPPiece;
		newPiece.GetElement().Size = this.Board.TileSize * Vector2.one;
		newPiece.ZSorter().layer = ZLayer.BoardPiece;
		newPiece.Initialize(this.session);
		Tile destinationTile = this.Board.GetClosest(lastCollision, this.Board.TileSize, (Tile t) => t.Piece == null && !resolveState.newPiecePerTile.ContainsKey(t));
		bool shouldFlyOffTop = this.ShouldFlyOffTop(destinationTile);
		if (shouldFlyOffTop)
		{
			destinationTile = Tile.Invalid;
		}
		Vector2 from = shot.origin;
		bool isDestroyedByIntercept = false;
		if (this.ShotMovementStarted != null)
		{
			this.ShotMovementStarted(newPiece, delegate(bool b)
			{
				this.PieceDiscarded(newPiece);
				this.Board.DespawnPiece(newPiece);
				isDestroyedByIntercept = true;
			});
		}
		float unitsPerSec = SingletonAsset<LevelVisuals>.Instance.shotSpeed;
		for (int i = 0; i < collisionPoints.Count; i++)
		{
			if (isDestroyedByIntercept)
			{
				break;
			}
			Vector2 collidePos = collisionPoints[i];
			Vector2 dif = collidePos - from;
			IEnumerator movementAnim = (this.CustomPieceMovement != null) ? this.CustomPieceMovement(i, newPiece, from, collidePos) : null;
			if (movementAnim == null)
			{
				float duration = dif.magnitude / unitsPerSec;
				yield return FiberAnimation.MoveLocalTransform(newPiece.transform, from, collidePos, null, duration);
			}
			else
			{
				yield return movementAnim;
			}
			if (shouldFlyOffTop && i == collisionPoints.Count - 1)
			{
				Vector2 flyOffset = dif * (200f / dif.y);
				Vector2 flyOffPos = collidePos + flyOffset;
				dif = flyOffPos - collidePos;
				float duration2 = dif.magnitude / unitsPerSec;
				movementAnim = FiberAnimation.MoveLocalTransform(newPiece.transform, collidePos, flyOffPos, null, duration2);
				yield return movementAnim;
			}
			from = collidePos;
		}
		if (this.ShotMovementEnded != null)
		{
			this.ShotMovementEnded(newPiece);
		}
		if (!isDestroyedByIntercept)
		{
			if (hitTile != Tile.Invalid && destinationTile != Tile.Invalid && !resolveState.newPiecePerTile.ContainsKey(destinationTile))
			{
				newPiece.TileIndex = destinationTile.Index;
				resolveState.newPiecePerTile.Add(destinationTile, newPiece);
				newPiece.AlignToTile();
				this.OptimizedAddForcesToBubbleWobble(destinationTile);
				newPiece.ActivateSpring(from, 30f);
			}
			else
			{
				this.PieceDiscarded(newPiece);
				this.Board.DespawnPiece(newPiece);
			}
		}
		yield break;
	}

	private bool ShouldFlyOffTop(Tile destinationTile)
	{
		bool result = false;
		if (this.session.Level.LevelAsset is BossLevel)
		{
			int num = 0;
			foreach (Tile tile in destinationTile.GetNeighbours())
			{
				if (tile.Piece != null)
				{
					num++;
				}
			}
			result = (destinationTile.Coord.y == 0 && num == 0);
		}
		return result;
	}

	private IEnumerator ResolveShoot(Ray2D[] aims, Action finished)
	{
		yield return new Fiber.OnExit(delegate()
		{
			finished();
		});
		this.pointsThisTurn.totalPoints = 0;
		this.pointsThisTurn.areaOfGivenPoints = UIBounds.Empty;
		PieceId pieceToShoot = this.session.GetNextPieceToShoot();
		this.session.UseShot();
		ResolveState resolveState = new ResolveState(this.Board);
		resolveState.PieceRemoved += this.HandlePieceRemovedByResolver;
		TurnLogic.Shot[] shots = new TurnLogic.Shot[aims.Length];
		for (int i = 0; i < aims.Length; i++)
		{
			shots[i] = new TurnLogic.Shot();
			shots[i].Calculate(this.Board, aims[i].origin, aims[i].direction);
		}
		this.ShotFired(shots, resolveState);
		IEnumerator[] shotsAnimations = new IEnumerator[shots.Length];
		for (int j = 0; j < shots.Length; j++)
		{
			shotsAnimations[j] = this.AnimateShot(shots[j], pieceToShoot, resolveState);
		}
		yield return FiberHelper.RunParallel(shotsAnimations);
		foreach (KeyValuePair<Tile, Piece> keyValuePair in resolveState.newPiecePerTile)
		{
			resolveState.MarkHit(keyValuePair.Key, HitCause.PlacedByShot, 0f, null);
			foreach (Tile at in keyValuePair.Key.GetNeighbours())
			{
				resolveState.MarkHit(at, HitCause.DirectHit, 0f, keyValuePair.Key.Piece as CPPiece);
			}
		}
		List<Clusters.ClusterHit> clusters = this.HitClusters(resolveState.newPiecePerTile.Keys, resolveState);
		yield return resolveState.ApplyAllHits(delegate
		{
			this.HitsMarked();
		});
		this.RemoveBubblesAfterClusters(resolveState);
		yield return resolveState.ApplyAllHits(null);
		if (this.session.Level.LevelAsset is BossLevel)
		{
			this.RemoveBottomBubbles(resolveState);
			yield return resolveState.ApplyAllHits(null);
		}
		if (clusters.Count > 0 || this.session.Powers.AnyActive)
		{
			this.StreakMultiplier++;
			this.session.IncrementGoodMoves();
		}
		else
		{
			this.StreakMultiplier = 1;
		}
		this.session.Powers.Reset();
		this.floatersLogic.Run(this.DoFloaters(resolveState), true);
		this.ResolveCompleted(this);
		if (this.session.Level.LevelAsset is BossLevel)
		{
			while (this.session.BossLevelController.BossCharacterController.IsBossHitByPower)
			{
				yield return null;
			}
		}
		else
		{
			yield return this.Board.AnimatePanToBottom();
		}
		if (this.PanningToBottomCompleted != null)
		{
			yield return this.PanningToBottomCompleted();
		}
		this.CompleteTurn();
		yield break;
	}

	private IEnumerator DoFloaters(ResolveState resolveState)
	{
		Floaters floaters = new Floaters(this.session);
		yield return floaters.Resolve(resolveState);
		yield break;
	}

	public ResolveState CreateResolveStateForAftermath()
	{
		this.StreakMultiplier = 1;
		ResolveState resolveState = new ResolveState(this.Board);
		resolveState.PieceRemoved += this.HandlePieceRemovedByResolver;
		return resolveState;
	}

	private void OptimizedAddForcesToBubbleWobble(Tile tile)
	{
		Vector3 position = tile.Piece.gameObject.transform.position;
		float piecesSpringArea = SingletonAsset<LevelVisuals>.Instance.piecesSpringArea;
		List<int> list = new List<int>();
		list.Add(tile.Index);
		for (int i = 0; i < list.Count; i++)
		{
			foreach (Tile tile2 in this.Board.GetTile(list[i]).GetOccupiedNeighbours())
			{
				if (!list.Contains(tile2.Index))
				{
					CPPiece cppiece = tile2.Pieces[0] as CPPiece;
					float num = (position - cppiece.transform.position).magnitude;
					if (num >= 1f && num < piecesSpringArea)
					{
						list.Add(tile2.Index);
						foreach (Piece piece in tile2.Pieces)
						{
							CPPiece cppiece2 = (CPPiece)piece;
							num = (piecesSpringArea - num) / piecesSpringArea;
							cppiece2.ActivateSpring(position, num * 30f);
						}
					}
				}
			}
		}
	}

	private void CompleteTurn()
	{
		this.PowersWasResolvedThisTurn = false;
		foreach (LogicModule logicModule in this.logicModules)
		{
			logicModule.TurnCompleted(this.session);
		}
		this.TurnCompleted();
	}

	private void HandlePieceRemovedByResolver(CPPiece piece, int points, HitMark hit)
	{
		if (hit.cause == HitCause.Power)
		{
			this.PowersWasResolvedThisTurn = true;
		}
		if (points > 0)
		{
			this.pointsThisTurn.totalPoints += piece.PointsForRemoving;
			this.pointsThisTurn.areaOfGivenPoints.Encapsulate(piece.transform.position);
		}
		this.PieceCleared(piece, points, hit);
	}

	private void RemoveBubblesAfterClusters(ResolveState resolveState)
	{
		foreach (KeyValuePair<Tile, Piece> keyValuePair in resolveState.newPiecePerTile)
		{
			if (keyValuePair.Key.Piece != null)
			{
				CPPiece cppiece = keyValuePair.Key.Piece as CPPiece;
				cppiece.RemoveAfterClustersIfAble(resolveState);
			}
		}
	}

	private void RemoveBottomBubbles(ResolveState resolveState)
	{
		foreach (Tile tile in resolveState.newPiecePerTile.Keys)
		{
			if (tile.Piece != null)
			{
				CPPiece cppiece = tile.Piece as CPPiece;
				cppiece.RemoveIfAtBottom(resolveState);
			}
		}
	}

	private bool IsPieceTypeInEnumerable<T>(IEnumerable<Tile> tiles) where T : CPPiece
	{
		foreach (Tile tile in tiles)
		{
			if (tile.Piece is T)
			{
				return true;
			}
		}
		return false;
	}

	private List<Clusters.ClusterHit> HitClusters(IEnumerable<Tile> originTiles, ResolveState resolveState)
	{
		List<Clusters.ClusterHit> list = Clusters.FindAllClusters(originTiles);
		if (list.Count < 3 && !this.IsPieceTypeInEnumerable<RainbowPiece>(originTiles))
		{
			return new List<Clusters.ClusterHit>();
		}
		foreach (Clusters.ClusterHit clusterHit in list)
		{
			Tile tile = this.Board.GetTile(clusterHit.tileIndex);
			resolveState.MarkHit(tile, HitCause.Cluster, (float)(clusterHit.step + 1) * SingletonAsset<LevelVisuals>.Instance.clusterPopInterval, null);
		}
		return new List<Clusters.ClusterHit>(list);
	}

	public readonly TurnLogic.PointsSummary pointsThisTurn = new TurnLogic.PointsSummary();

	private readonly Fiber resolveFiber = new Fiber();

	private readonly List<LogicModule> logicModules = new List<LogicModule>();

	private readonly FiberRunner floatersLogic = new FiberRunner(FiberBucket.Update);

	private readonly LevelSession session;

	public TurnLogic.CustomPieceMovementDelegate CustomPieceMovement;

	public delegate IEnumerator CustomPieceMovementDelegate(int movementIndex, CPPiece piece, Vector3 from, Vector3 to);

	public delegate IEnumerator PanningToBottomCompletedDelegate();

	public class PointsSummary
	{
		public int totalPoints;

		public UIBounds areaOfGivenPoints;
	}

	public class Shot
	{
		public void Calculate(GameBoard board, Vector2 origin, Vector2 dir)
		{
			this.origin = origin;
			this.collisionPoints = Trajectory.CalculateTrajectoryHits(board, origin, dir, out this.hitTile);
		}

		public List<Vector2> collisionPoints = new List<Vector2>();

		public Tile hitTile;

		public Vector2 origin;
	}
}
