using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class ResolveState
{
	public ResolveState(GameBoard board)
	{
		this.Board = board;
		this.Reset();
	}

	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<CPPiece, int, HitMark> PieceRemoved = delegate (CPPiece A_0, int A_1, HitMark A_2)
    {
    };



    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action<List<HitMark>> AllHitsApplied = delegate (List<HitMark> A_0)
    {
    };



    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action<List<TileHitMark>> AllMarkedByPowerTilesApplied = delegate (List<TileHitMark> A_0)
    {
    };



    public Board Board { get; private set; }

	public void Reset()
	{
		this.currentMarkedHits = new List<HitMark>();
		this.currentMarkedByPowerTiles = new List<TileHitMark>();
		this.pieceInfos.Clear();
		this.currentHit = default(HitMark);
	}

	public bool IsPieceMarkedForRemoval(Piece p)
	{
		return this.pieceInfos.ContainsKey(p) && this.pieceInfos[p].replacementId == PieceId.Empty;
	}

	public IHitResolver CreatePieceResolver(CPPiece p)
	{
		ResolveState.PieceResolver pieceResolver = new ResolveState.PieceResolver(p, this);
		this.pieceInfos[p] = pieceResolver;
		return pieceResolver;
	}

	public IEnumerator ApplyAllHits(Action preAnimationCallback = null)
	{
		int endlessLoopGuard = 100;
		List<HitMark> allHits = new List<HitMark>();
		while (this.currentMarkedHits.Count > 0)
		{
			int num;
			endlessLoopGuard = (num = endlessLoopGuard) - 1;
			if (num <= 0)
			{
				break;
			}
			List<HitMark> markedHits = new List<HitMark>(this.currentMarkedHits);
			this.currentMarkedHits.Clear();
			this.ApplyMarkedHits(markedHits);
			allHits.AddRange(this.currentMarkedHits);
		}
		if (preAnimationCallback != null)
		{
			preAnimationCallback();
		}
		this.AllHitsApplied(allHits);
		this.AllMarkedByPowerTilesApplied(this.currentMarkedByPowerTiles);
		if (this.PreExecute != null)
		{
			yield return this.PreExecute();
		}
		List<IEnumerator> pieceExecutors = new List<IEnumerator>();
		foreach (KeyValuePair<Piece, ResolveState.PieceResolver> keyValuePair in this.pieceInfos)
		{
			pieceExecutors.Add(keyValuePair.Value.Execute(this.Board));
			pieceExecutors.AddRange(keyValuePair.Value.EffectAnimations);
		}
		yield return FiberHelper.RunParallel(pieceExecutors.ToArray());
		this.Reset();
		yield break;
	}

	public bool MarkHit(Tile at, HitCause cause, float delay = 0f, CPPiece overrideCausedBy = null)
	{
		bool result = false;
		TileHitMark tileHitMark = this.currentMarkedByPowerTiles.Find((TileHitMark x) => x.tile == at);
		if (cause == HitCause.Power && tileHitMark == null)
		{
			TileHitMark item = new TileHitMark
			{
				tile = at,
				time = delay
			};
			this.currentMarkedByPowerTiles.Add(item);
		}
		foreach (Piece piece in at.Pieces)
		{
			CPPiece cppiece = piece as CPPiece;
			if (!(cppiece == null))
			{
				if (!this.IsPieceMarkedForRemoval(cppiece))
				{
					HitMark item2 = new HitMark
					{
						piece = cppiece,
						cause = cause,
						causedBy = ((!(overrideCausedBy != null)) ? this.currentHit.piece : overrideCausedBy),
						time = this.currentHit.time + delay
					};
					if (!this.currentMarkedHits.Contains(item2))
					{
						this.currentMarkedHits.Add(item2);
						result = true;
					}
					if (cppiece.BlockHitsOnPiecesUnderneath)
					{
						break;
					}
				}
			}
		}
		return result;
	}

	private void ApplyMarkedHits(List<HitMark> markedHits)
	{
		foreach (HitMark hitMark in markedHits)
		{
			if (!this.pieceInfos.ContainsKey(hitMark.piece))
			{
				this.pieceInfos[hitMark.piece] = new ResolveState.PieceResolver(hitMark.piece, this);
			}
			this.currentHit = hitMark;
			hitMark.piece.Hit(this.pieceInfos[hitMark.piece]);
		}
	}

	public Func<IEnumerator> PreExecute;

	public readonly Dictionary<Tile, Piece> newPiecePerTile = new Dictionary<Tile, Piece>();

	private readonly Dictionary<Piece, ResolveState.PieceResolver> pieceInfos = new Dictionary<Piece, ResolveState.PieceResolver>();

	private HitMark currentHit;

	private List<HitMark> currentMarkedHits;

	private List<TileHitMark> currentMarkedByPowerTiles;

	public class PieceResolver : IHitResolver
	{
		public PieceResolver(CPPiece p, ResolveState state)
		{
			this.piece = p;
			this.replacementId = p.TypeId;
			this.state = state;
		}

		public List<IEnumerator> EffectAnimations
		{
			get
			{
				return this.effectAnimations;
			}
		}

		public IEnumerator Execute(Board board)
		{
			yield return FiberHelper.RunParallel(this.pieceAnimations.ToArray());
			if (this.replacementId.IsEmpty)
			{
				this.piece.TileIndex = -1;
				this.state.PieceRemoved(this.piece, this.pointsForRemoval, this.hitCausingRemoval);
				board.DespawnPiece(this.piece);
			}
			else if (this.replacementId != this.piece.TypeId)
			{
				int tileIndex = this.piece.TileIndex;
				board.DespawnPiece(this.piece);
				board.SpawnPieceAt(tileIndex, this.replacementId);
			}
			yield break;
		}

		bool IHitResolver.MarkHit(Tile at, HitCause cause, float delay)
		{
			return this.state.MarkHit(at, cause, delay, null);
		}

		void IHitResolver.QueueAnimation(IEnumerator func, float delay)
		{
			float time = this.state.currentHit.time;
			this.pieceAnimations.Add(FiberHelper.RunSerial(new IEnumerator[]
			{
				FiberHelper.Wait(time + delay, (FiberHelper.WaitFlag)0),
				func
			}));
		}

		HitMark IHitResolver.Hit
		{
			get
			{
				return this.state.currentHit;
			}
		}

		void IHitResolver.MarkForRemoval(float delay, int pointsOverride)
		{
			if (pointsOverride < 0)
			{
				this.pointsForRemoval = this.piece.PointsForRemoving;
			}
			else
			{
				this.pointsForRemoval = pointsOverride;
			}
			this.hitCausingRemoval = ((IHitResolver)this).Hit;
			((IHitResolver)this).MarkForReplacement(PieceId.Empty, delay);
		}

		void IHitResolver.MarkForReplacement(PieceId newId, float delay)
		{
			if (this.state.IsPieceMarkedForRemoval(this.piece))
			{
				return;
			}
			this.replacementId = newId;
			this.pieceAnimations.Add(FiberHelper.Wait(this.state.currentHit.time + delay, (FiberHelper.WaitFlag)0));
		}

		void IHitResolver.QueueEffect(IEnumerator func, float delay)
		{
			float time = this.state.currentHit.time;
			this.effectAnimations.Add(FiberHelper.RunSerial(new IEnumerator[]
			{
				FiberHelper.Wait(time + delay, (FiberHelper.WaitFlag)0),
				func
			}));
		}

		public PieceId replacementId;

		public List<IEnumerator> pieceAnimations = new List<IEnumerator>();

		public HitMark hitCausingRemoval;

		private CPPiece piece;

		private ResolveState state;

		private int pointsForRemoval;

		private List<IEnumerator> effectAnimations = new List<IEnumerator>();
	}
}
