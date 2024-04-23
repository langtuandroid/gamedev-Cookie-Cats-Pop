using System;
using System.Collections;
using System.Collections.Generic;
using Spine;
using UnityEngine;

public class PowerCatYellow : PowerCat
{
	protected override void OnInitialize()
	{
		this.bubblePivot.SetActive(false);
		this.ninjaStarPrefab.SetActive(false);
	}

	protected override IEnumerator ChargedState()
	{
		yield return this.spine.PlayUntilEvent("Eating", "Idle start");
		base.ChargedAnimIsLooping = true;
		yield return this.spine.PlayLoopBetweenEvents("Eating", "Idle start", "Idle End", -1f);
		yield break;
	}

	public override IEnumerator AnimatePowerPiece(PieceId pieceId, Transform targetPivot, Action bubbleSpawned)
	{
		float startTime = this.spine.PlayStartingFromEvent("Eating", "Idle End");
		base.ScheduleIdle(3.33333325f);
		GameObject bubble = UnityEngine.Object.Instantiate<GameObject>(this.bubblePivot);
		bubble.transform.parent = targetPivot;
		bubble.transform.localPosition = Vector3.zero;
		bubble.transform.localScale = Vector3.one;
		bubble.SetActive(true);
		bubbleSpawned();
		Dictionary<Spine.Event, float> events = this.spine.GetEvents("Eating");
		float starZPosition = bubble.transform.position.z + 0.1f;
		List<IEnumerator> enums = new List<IEnumerator>();
		enums.Add(FiberAnimation.ScaleTransform(bubble.transform, Vector3.zero, Vector3.one, SingletonAsset<PowerVisualSettings>.Instance.pieceGrowCurve, 0.3f));
		foreach (KeyValuePair<Spine.Event, float> keyValuePair in events)
		{
			if (keyValuePair.Key.Data.Name == "FireNinjaStar")
			{
				enums.Add(this.FireNinjaStar(keyValuePair.Value - startTime, this.ninjaStarBones[keyValuePair.Key.Int], targetPivot, starZPosition, bubble.transform));
			}
		}
		if (enums.Count > 0)
		{
			yield return FiberHelper.RunParallel(enums.ToArray());
		}
		yield return FiberHelper.Wait(0.1f, (FiberHelper.WaitFlag)0);
		yield break;
	}

	private IEnumerator FireNinjaStar(float delay, Transform source, Transform dest, float zPos, Transform bubbleTransform)
	{
		yield return FiberHelper.Wait(delay, (FiberHelper.WaitFlag)0);
		Vector3 sourcePosition = source.position;
		GameObject ninjaStar = UnityEngine.Object.Instantiate<GameObject>(this.ninjaStarPrefab);
		ninjaStar.transform.parent = dest;
		ninjaStar.transform.position = sourcePosition;
		ninjaStar.transform.localRotation = Quaternion.identity;
		ninjaStar.SetActive(true);
		Vector3 startScale = ninjaStar.transform.localScale;
		Vector3 endScale = new Vector3(startScale.x * this.ninjaStarEndScale, startScale.y * this.ninjaStarEndScale, startScale.z);
		yield return FiberAnimation.Animate(0.3f, this.ninjaStarMoveCurve, delegate(float t)
		{
			Vector3 position = Vector3.Lerp(sourcePosition, dest.position, t);
			position.z = zPos;
			ninjaStar.transform.position = position;
			ninjaStar.transform.localScale = Vector3.Lerp(startScale, endScale, t);
		}, false);
		yield return FiberAnimation.ScaleTransform(bubbleTransform, Vector3.zero, Vector3.one, this.ninjaStarHitBubbleCurve, 0.4f);
		yield break;
	}

	public GameObject bubblePivot;

	public AnimationCurve ninjaStarMoveCurve;

	public GameObject ninjaStarPrefab;

	public Transform[] ninjaStarBones;

	public AnimationCurve ninjaStarHitBubbleCurve;

	public float ninjaStarEndScale;
}
