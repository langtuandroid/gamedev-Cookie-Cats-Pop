using System;
using UnityEngine;

public class ChainPiece : CPPiece
{
	public override bool IsAttachment
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

	public override bool PreventClustering
	{
		get
		{
			return true;
		}
	}

	public override void Hit(IHitResolver resolver)
	{
		HitMark hit = resolver.Hit;
		if (hit.cause == HitCause.Power && (hit.causedBy is ChainPiece || hit.causedBy is LockPiece))
		{
			resolver.MarkForRemoval(0f, -1);
			resolver.QueueAnimation(this.AnimatePop(), 0f);
		}
	}

	public override bool IsRotatable
	{
		get
		{
			return true;
		}
	}

	public override void AfterBoardSetup(Board board)
	{
		base.AfterBoardSetup(board);
		this.sprite.localRotation = Quaternion.Euler(0f, 0f, (UnityEngine.Random.value - 0.5f) * 10f);
	}

	public Transform sprite;
}
