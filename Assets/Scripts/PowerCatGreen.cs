using System;
using System.Collections;
using UnityEngine;

public class PowerCatGreen : PowerCat
{
	protected override void OnInitialize()
	{
		this.frog.SetActive(false);
		this.bubblePivot.SetActive(false);
	}

	protected override IEnumerator ChargedState()
	{
		yield return this.spine.PlayUntilEvent("EatingPop", "Active_Loop_Start");
		base.ChargedAnimIsLooping = true;
		yield return this.spine.PlayLoopBetweenEvents("EatingPop", "Active_Loop_Start", "Active_Loop_End", -1f);
		yield break;
	}

	private Vector3 GetFrogPosition(Vector3 startPosition, Transform targetPivot, float t, float curveHeight = 60f)
	{
		Vector3 position = targetPivot.position;
		Vector3 result = Vector3.Lerp(startPosition, position, t);
		result.y += Mathf.Sin(t * 3.14159274f) * curveHeight;
		return result;
	}

	public override IEnumerator AnimatePowerPiece(PieceId pieceId, Transform targetPivot, Action bubbleSpawned)
	{
		if (this.currentState != PowerCat.State.Charged)
		{
			yield return this.spine.PlayTimeline("EatingPop", 2.66666675f, 3.33333325f, 0.6666667f, 1f, null);
		}
		this.spine.PlayStartingFromEvent("EatingPop", "Active_Loop_End");
		Vector3 frogSpawnPosition = Vector3.zero;
		float startTime = 280f;
		GameObject bubble = UnityEngine.Object.Instantiate<GameObject>(this.bubblePivot);
		bubble.transform.parent = targetPivot;
		bubble.transform.localPosition = Vector3.zero;
		bubble.transform.localScale = Vector3.one;
		bubble.SetActive(false);
		SkeletonAnimation bubbleSpine = bubble.GetComponentInChildren<SkeletonAnimation>();
		base.ScheduleIdle(2.33333325f);
		yield return FiberHelper.RunParallel(new IEnumerator[]
		{
			FiberHelper.RunSerial(new IEnumerator[]
			{
				FiberHelper.RunDelayed((323.85f - startTime) / 30f, delegate
				{
					this.frog.SetActive(true);
					frogSpawnPosition = this.frogSpawnPivot.position;
				}),
				FiberAnimation.Animate(1f, delegate(float t)
				{
					Vector3 frogPosition = this.GetFrogPosition(frogSpawnPosition, targetPivot, t, 60f);
					Vector3 frogPosition2 = this.GetFrogPosition(frogSpawnPosition, targetPivot, t + 0.001f, 60f);
					Vector3 vector = frogPosition - frogPosition2;
					float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
					this.frog.transform.position = frogPosition;
					this.frog.transform.localRotation = Quaternion.Euler(0f, 0f, num - 90f);
					this.frogSpine.state.GetCurrent(0).time = ((t <= 0.5f) ? 0f : 0.0333333351f);
				})
			}),
			FiberHelper.RunDelayed((300f - startTime) / 30f, delegate
			{
				this.waterSplash.Play();
			}),
			FiberHelper.RunSerial(new IEnumerator[]
			{
				FiberHelper.RunDelayed((326f - startTime) / 30f, delegate
				{
					bubble.SetActive(true);
					bubbleSpawned();
				}),
				FiberAnimation.ScaleTransform(bubble.transform, Vector3.zero, Vector3.one, SingletonAsset<PowerVisualSettings>.Instance.pieceGrowCurve, 0.3f)
			}),
			FiberHelper.RunDelayed((323.85f - startTime) / 30f + 0.9f, delegate
			{
				this.frog.SetActive(false);
				bubbleSpine.PlayAnimation(0, "FrogIntoBubble", true, true);
				bubbleSpine.timeScale = 1f;
				bubbleSpine.Update(0f);
				bubbleSpine.skeleton.UpdateWorldTransform();
			})
		});
		this.frog.SetActive(false);
		yield return FiberHelper.Wait(0.25f, (FiberHelper.WaitFlag)0);
		yield break;
	}

	public Transform muzzlePivot;

	public Transform frogSpawnPivot;

	public GameObject frog;

	public SkeletonAnimation frogSpine;

	public ParticleSystem waterSplash;

	public GameObject bubblePivot;

	private const string chargedAnim = "EatingPop";
}
