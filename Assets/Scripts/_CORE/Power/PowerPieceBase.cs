using System;

public abstract class PowerPieceBase : CPPiece
{
	public override void SpawnedByBoard(Board board)
	{
		base.SpawnedByBoard(board);
		this.hasTriggered = false;
	}

	public override void Hit(IHitResolver resolver)
	{
		if (resolver.Hit.cause == HitCause.DirectHit || resolver.Hit.cause == HitCause.Power || resolver.Hit.cause == HitCause.PlacedByShot)
		{
			Tile tile = base.GetTile();
			resolver.MarkForRemoval(0f, -1);
			if (!this.hasTriggered)
			{
				this.hasTriggered = true;
				this.TriggerPower(tile, resolver);
			}
		}
	}

	protected abstract void TriggerPower(Tile origin, IHitResolver resolver);

	private bool hasTriggered;
}
