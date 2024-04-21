using System;
using UnityEngine;

public class SlidesAndLaddersMapCurves : MonoBehaviour
{
	public AnimationCurve SlideCurve
	{
		get
		{
			return this.slideCurve;
		}
	}

	public AnimationCurve LadderCurve
	{
		get
		{
			return this.ladderCurve;
		}
	}

	public AnimationCurve AvatarMoveCurve
	{
		get
		{
			return this.avatarMoveCurve;
		}
	}

	public AnimationCurve AvatarMoveOffsetCurve
	{
		get
		{
			return this.avatarMoveOffsetCurve;
		}
	}

	[SerializeField]
	private AnimationCurve ladderCurve;

	[SerializeField]
	private AnimationCurve slideCurve;

	[SerializeField]
	private AnimationCurve avatarMoveCurve;

	[SerializeField]
	private AnimationCurve avatarMoveOffsetCurve;
}
