using System;
using System.Collections;
using Spine;
using TactileModules.PuzzleGame.PiggyBank.Interfaces;
using TactileModules.PuzzleGame.PiggyBank.UI;
using UnityEngine;

public class PiggyBankViewExtension : MonoBehaviour, IPiggyBankViewExtension
{
	public float YOffSetFreeOpeningState
	{
		get
		{
			return -25f;
		}
	}

	public void Initialize(PiggyBankView view)
	{
		this.coinsButton = UIViewManager.Instance.ObtainOverlay<CurrencyOverlay>(view).coinButton;
		this.PauseRefreshingCoins(true);
	}

	public void PauseRefreshingCoins(bool pause)
	{
		if (this.coinsButton != null)
		{
			this.coinsButton.PauseRefreshingCoins(pause);
		}
	}

	public IEnumerator PlayOpeningAnimation()
	{
		this.piggySkeletonAnimation.PlayAnimation(0, "PiggieBankFree", false, false);
		yield return FiberHelper.Wait(3f, (FiberHelper.WaitFlag)0);
		yield break;
	}

	public IEnumerator AnimateCoinAmount(int numCoins, float duration)
	{
		UIViewManager.Instance.FindOverlay<CurrencyOverlay>().coinAnimator.GiveCoins(this.coinAnimationOrigin.position, numCoins, duration, null, null);
		yield return null;
		yield break;
	}

	public void PlayOpeningAvailableIdleAnimation()
	{
		this.piggySkeletonAnimation.PlayAnimation(0, "PiggieBankIdleCoinPop", true, false);
		this.shimmerParticleSystem.Play();
	}

	public void PlayLockedJumpingAnimation()
	{
		this.piggySkeletonAnimation.PlayAnimation(0, "PiggiebankJumping", true, false);
	}

	public TrackEntry PlayEmptyBankAnimation()
	{
		return this.piggySkeletonAnimation.PlayAnimation(0, "PiggieBankEmpty", false, false);
	}

	[SerializeField]
	private Transform coinAnimationOrigin;

	[SerializeField]
	private SkeletonAnimation piggySkeletonAnimation;

	[SerializeField]
	private ParticleSystem shimmerParticleSystem;

	private CoinsButton coinsButton;
}
