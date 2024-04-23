using System;
using Shared.PiggyBank.Module.Interfaces;
using Spine;
using UnityEngine;

public class PiggyBankLevelDotIndicatorExtension : MonoBehaviour, IPiggyBankLevelDotIndicatorExtension
{
	public TrackEntry PlayAnimation(string animationName)
	{
		return this.piggySkeletonAnimation.PlayAnimation(0, animationName, false, false);
	}

	[SerializeField]
	private SkeletonAnimation piggySkeletonAnimation;
}
