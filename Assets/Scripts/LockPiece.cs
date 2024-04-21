using System;

public class LockPiece : CPPiece
{
	public override bool IsAttachment
	{
		get
		{
			return true;
		}
	}

	public override int TileLayer
	{
		get
		{
			return 1;
		}
	}

	public override bool PreventFurtherClustering
	{
		get
		{
			return true;
		}
	}

	public override bool BlockHitsOnPiecesUnderneath
	{
		get
		{
			return false;
		}
	}

	public override bool IsRotatable
	{
		get
		{
			return true;
		}
	}

	public override void SpawnedByBoard(Board board)
	{
		base.SpawnedByBoard(board);
		this.hasTriggered = false;
	}

	public override void Hit(IHitResolver resolver)
	{
		if ((resolver.Hit.cause == HitCause.Cluster || resolver.Hit.cause == HitCause.Power) && !this.hasTriggered)
		{
			this.hasTriggered = true;
			Tile tile = base.GetTile();
			Direction dir = base.Board.Topology.ApproximateFromAngle((float)base.Direction);
			this.BreakNeighboorChain(tile, dir, resolver, 1);
			this.BreakNeighboorChain(tile, dir.Opposite(), resolver, 1);
			resolver.QueueAnimation(this.AnimatePop(), 0f);
			resolver.MarkForRemoval(0f, -1);
		}
	}

	private void BreakNeighboorChain(Tile origin, Direction dir, IHitResolver resolver, int step)
	{
		Tile neighbour = origin.GetNeighbour(dir);
		foreach (Piece piece in neighbour.Pieces)
		{
			if (piece is ChainPiece && (piece.Direction == dir.Value || piece.Direction == dir.Opposite().Value))
			{
				resolver.MarkHit(neighbour, HitCause.Power, (float)step * 0.4f);
				this.BreakNeighboorChain(neighbour, dir, resolver, step + 1);
				break;
			}
		}
	}

	private bool hasTriggered;
}
