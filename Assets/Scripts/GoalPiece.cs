using System;
using Spine;
using UnityEngine;

public class GoalPiece : NormalPiece
{
	public override void AfterBoardSetup(Board board)
	{
		TrackEntry current = this.part.GetInstance<Transform>().gameObject.GetComponentInChildren<SkeletonAnimation>().state.GetCurrent(0);
		if (current != null)
		{
			current.time = UnityEngine.Random.value * current.endTime;
		}
	}

	public override bool IsBasicPiece
	{
		get
		{
			return false;
		}
	}

	public override int PointsForRemoving
	{
		get
		{
			return 500;
		}
	}

	public UIInstantiator part;
}
