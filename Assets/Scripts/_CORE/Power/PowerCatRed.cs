using System;
using System.Collections;
using UnityEngine;

public class PowerCatRed : PowerCat
{
	protected override void OnInitialize()
	{
		this.bubblePivot.SetActive(false);
	}

	protected override IEnumerator ChargedState()
	{
		yield return this.spine.PlayUntilEvent("EatingPOP", "ReadyIdleStart");
		base.ChargedAnimIsLooping = true;
		yield return this.spine.PlayLoopBetweenEvents("EatingPOP", "ReadyIdleStart", "ReadyIdleStop", -1f);
		yield break;
	}

	public override IEnumerator AnimatePowerPiece(PieceId pieceId, Transform targetPivot, Action bubbleSpawned)
	{
		GameObject bubble = UnityEngine.Object.Instantiate<GameObject>(this.bubblePivot);
		bubble.transform.parent = targetPivot;
		bubble.transform.localPosition = Vector3.zero;
		bubble.transform.localScale = Vector3.one;
		bubble.SetActive(true);
		GameObject fireball = bubble.GetComponentInChildren<SkeletonAnimation>().gameObject;
		fireball.SetActive(false);
		bubbleSpawned();
		ParticleSystem[] particleSystems = bubble.GetComponentsInChildren<ParticleSystem>();
		float startTime = this.spine.PlayStartingFromEvent("EatingPOP", "ReadyIdleStop");
		base.ScheduleIdle(2.3f);
		yield return FiberHelper.RunParallel(new IEnumerator[]
		{
			FiberAnimation.ScaleTransform(bubble.transform, Vector3.zero, Vector3.one, SingletonAsset<PowerVisualSettings>.Instance.pieceGrowCurve, 0.3f),
			FiberHelper.RunDelayed(7.5f - startTime, delegate
			{
				for (int i = 0; i < particleSystems.Length; i++)
				{
					particleSystems[i].Play();
				}
				fireball.SetActive(true);
			}),
			FiberHelper.RunDelayed(6.83333349f - startTime, delegate
			{
				this.fireSpray.Play();
			}),
			FiberHelper.RunDelayed(8.066667f - startTime, delegate
			{
				this.fireSpray.Stop();
			})
		});
		yield return FiberHelper.Wait(1f, (FiberHelper.WaitFlag)0);
		yield break;
	}

	public GameObject bubblePivot;

	public ParticleSystem fireSpray;
}
