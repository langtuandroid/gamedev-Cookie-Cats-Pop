using System;
using System.Collections;
using UnityEngine;

public class InkPiece : CPPiece
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
			return 2;
		}
	}

	public override bool PreventClustering
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
			return false;
		}
	}

	public override void SpawnedByBoard(Board board)
	{
		base.SpawnedByBoard(board);
	}

	public override void Hit(IHitResolver resolver)
	{
		if (resolver.Hit.cause == HitCause.DirectHit || resolver.Hit.cause == HitCause.Power || resolver.Hit.cause == HitCause.ClusterSplash)
		{
			resolver.QueueAnimation(this.AnimatePop(), 0f);
			resolver.MarkForRemoval(0f, -1);
		}
	}

	private void BreakNeighboorChain(Tile origin, Direction dir, IHitResolver resolver, int step)
	{
	}

	public override IEnumerator AnimatePop()
	{
		yield return FiberAnimation.ScaleTransform(base.transform, Vector3.one, Vector3.one * 1.3f, null, 0.1f);
		base.transform.localScale = Vector3.zero;
		EffectPool.Instance.SpawnEffect("SquidLandEffect", base.transform.position, base.gameObject.layer, new object[0]);
		yield return FiberHelper.Wait(0.1f, (FiberHelper.WaitFlag)0);
		yield break;
	}
}
