using System;
using TactileModules.PuzzleGame.PiggyBank.UI;
using UnityEngine;

public class CCPPiggyBankSideMapButton : PiggyBankSideMapButton
{
	protected override void PlayAnimation(string animationName)
	{
		this.buttonSpineSkeletonAnimation.AddAnimationInQueue(0, animationName, false, 0f, false);
	}

	protected override bool ShouldAddAnimation(string animationName)
	{
		return !string.IsNullOrEmpty(animationName) && (this.buttonSpineSkeletonAnimation.state.GetCurrent(0) == null || !(this.buttonSpineSkeletonAnimation.state.GetCurrent(0).Animation.Name == animationName));
	}

	protected override void PlayUnlockSpine(string animationName)
	{
		this.buttonSpineSkeletonAnimation.PlayAnimation(0, animationName, false, false);
		this.stateController.UnlockVisualsHandled();
	}

	[SerializeField]
	private SkeletonAnimation buttonSpineSkeletonAnimation;
}
