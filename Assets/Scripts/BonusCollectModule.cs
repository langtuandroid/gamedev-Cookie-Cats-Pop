using System;
using UnityEngine;

public class BonusCollectModule : LogicModule
{
	public override void Begin(LevelSession session)
	{
		this.levelSession = session;
		this.levelSession.TurnLogic.PieceCleared += this.HandlePieceCleared;
	}

	public override void End(LevelSession session)
	{
		this.levelSession.TurnLogic.PieceCleared -= this.HandlePieceCleared;
	}

	private void HandlePieceCleared(CPPiece piece, int pointsToGive, HitMark hit)
	{
		if (piece is BonusPiece)
		{
			Singleton<BonusDropManager>.Instance.CollectDrop();
			this.levelSession.Stats.IncrementBonusDropsCollected();
			SpawnedEffect spawnedEffect = EffectPool.Instance.SpawnEffect("BonusPieceCollectedEffect", Vector3.zero, piece.gameObject.layer, new object[]
			{
				piece.MatchFlag,
				piece.transform.position
			});
			this.levelSession.Cannon.InputEnabled += false;
			this.levelSession.BallQueue.SwappingEnabled += false;
			spawnedEffect.onFinished = delegate()
			{
				this.levelSession.Cannon.InputEnabled += true;
				this.levelSession.BallQueue.SwappingEnabled += true;
			};
		}
	}

	private LevelSession levelSession;
}
