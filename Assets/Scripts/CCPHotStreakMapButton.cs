using System;
using TactileModules.PuzzleGame.HotStreak.UI;
using UnityEngine;

public class CCPHotStreakMapButton : HotStreakMapButton
{
	protected override void Setup()
	{
		base.Setup();
		int num = Mathf.Min(base.Manager.Progress, base.Manager.MaxWins);
		string animationName = "Smoke";
		if (num > 0 && num <= base.Manager.MaxWins)
		{
			animationName = "Jug_Filling_" + num;
		}
		if (this.spineAnimation.HasAnimation(animationName))
		{
			this.spineAnimation.AnimationState.SetAnimation(0, animationName, false);
		}
	}

	[SerializeField]
	private SkeletonAnimation spineAnimation;

	private const string SmokeString = "Smoke";

	private const string JugFillingString = "Jug_Filling_";
}
