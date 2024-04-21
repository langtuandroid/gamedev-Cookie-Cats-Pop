using System;
using System.Collections;
using System.Collections.Generic;
using Spine;
using UnityEngine;

public class FinalPowerHitEffect : SpawnedEffect
{
	private void OnSpawned()
	{
		if (this.baitPivot != null)
		{
			this.baitPivot.transform.localPosition = Vector3.zero;
		}
	}

	public void SetCenter(Vector3 worldPositionCenter)
	{
		Vector3 position = this.baitPivot.transform.position;
		Vector3 position2 = base.transform.position;
		position2.x = worldPositionCenter.x;
		base.transform.position = position2;
		this.baitPivot.transform.position = position;
	}

	protected override IEnumerator AnimationLogic(object[] parameters)
	{
		this.baitPivot.SetActive(true);
		this.sharkPivot.SetActive(false);
		yield return FiberAnimation.ScaleTransform(this.baitPivot.transform, Vector3.zero, Vector3.one, this.baitScaleCurve, 0.4f);
		this.sharkPivot.SetActive(true);
		yield return FiberHelper.RunParallel(new IEnumerator[]
		{
			FiberHelper.RunSerial(new IEnumerator[]
			{
				FiberHelper.Wait(0.5f, (FiberHelper.WaitFlag)0),
				FiberAnimation.ScaleTransform(this.sharkBaitLabel.transform, this.sharkBaitLabel.transform.localScale, Vector3.zero, AnimationCurve.EaseInOut(0f, 0f, 1f, 1f), 0.4f)
			}),
			FiberHelper.RunDelayed(this.baitRemoveTime, delegate
			{
				this.baitPivot.SetActive(false);
			}),
			this.MoveShark(),
			this.AnimateShark()
		});
		yield break;
	}

	private IEnumerator AnimateShark()
	{
		yield return this.sharkSpine.PlayTimeline("animation", "IdleStart", "IdleEnd", this.movementCurve.Duration(), 1f, null);
		yield return this.sharkSpine.PlayTimeline("animation", "IdleEnd", "ActiveLoopStart", this.moveToFrenzyCurve.Duration(), 1f, null);
		yield return this.sharkSpine.PlayTimeline("animation", "ActiveLoopStart", "ActiveLoopEnd", this.frenzyCurve.Duration(), 2f, null);
		yield break;
	}

	private IEnumerator MoveShark()
	{
		yield return FiberAnimation.MoveLocalTransform(this.sharkPivot.transform, this.movementStart.localPosition, this.movementEnd.localPosition, this.movementCurve, 0f);
		yield return FiberAnimation.MoveLocalTransform(this.sharkPivot.transform, this.movementEnd.localPosition, this.frenzyStart.localPosition, this.moveToFrenzyCurve, 0f);
		yield return FiberAnimation.MoveLocalTransform(this.sharkPivot.transform, this.frenzyStart.localPosition, this.frenzyEnd.localPosition, this.frenzyCurve, 0f);
		yield return FiberAnimation.MoveLocalTransform(this.sharkPivot.transform, this.frenzyEnd.localPosition, this.moveOut.localPosition, this.moveOutCurve, 0f);
		yield break;
	}

	public void PositionTransformFromBone(Transform thisTransform, Bone bone)
	{
		Vector3 position = this.sharkSpine.transform.TransformPoint(new Vector3(bone.worldX, bone.worldY, 0f));
		thisTransform.position = position;
		Vector3 eulerAngles = this.sharkSpine.transform.rotation.eulerAngles;
		thisTransform.rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, this.sharkSpine.transform.rotation.eulerAngles.z + bone.WorldRotationX);
	}

	public void CalculateCollisionDelays(int sampleResolution = 100)
	{
		Bone bone = this.sharkSpine.skeleton.FindBone("Head");
		TrackEntry trackEntry = this.sharkSpine.PlayAnimation(0, "animation", false, true);
		float eventTime = this.sharkSpine.GetEventTime("animation", "ActiveLoopStart");
		float eventTime2 = this.sharkSpine.GetEventTime("animation", "ActiveLoopEnd");
		this.normalizedCollisionDelays = new float[462];
		float[] array = new float[462];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = float.MaxValue;
		}
		List<float> eventTimings = this.sharkSpine.GetEventTimings("animation", "Bite");
		float num = 1f / (float)sampleResolution;
		for (int j = 0; j <= sampleResolution; j++)
		{
			float num2 = num * (float)j;
			this.sharkPivot.transform.localPosition = FiberAnimation.LerpNoClamp(this.frenzyStart.localPosition, this.frenzyEnd.localPosition, this.frenzyCurve.Evaluate(num2 * this.frenzyCurve.Duration()));
			this.sharkSpine.Update(num);
			trackEntry.time = eventTime + (eventTime2 - eventTime) * Mathf.Repeat(num2 * 2f, 1f);
			this.sharkSpine.Update(num);
			this.sharkSpine.skeleton.UpdateWorldTransform();
			this.PositionTransformFromBone(this.collisionHeadBone, bone);
			if (this.CloseToTiming(eventTimings, trackEntry.time))
			{
				Vector3 v = this.collisionArea.transform.InverseTransformPoint(this.collisionTransform.TransformPoint(Vector3.zero));
				Vector2 b = v;
				for (int k = 0; k < this.normalizedCollisionDelays.Length; k++)
				{
					Vector2 midPointFromIndex = this.GetMidPointFromIndex(k);
					float num3 = Vector2.SqrMagnitude(midPointFromIndex - b);
					if (num3 < array[k])
					{
						array[k] = num3;
						this.normalizedCollisionDelays[k] = num2;
					}
				}
			}
		}
	}

	private bool CloseToTiming(List<float> timings, float time)
	{
		for (int i = 0; i < timings.Count; i++)
		{
			float num = Mathf.Abs(time - timings[i]);
			if (num < 0.06666667f)
			{
				return true;
			}
		}
		return false;
	}

	public float GetDelayFromWorldPos(Vector3 worldPosition)
	{
		if (this.normalizedCollisionDelays == null || this.normalizedCollisionDelays.Length == 0)
		{
			return 0.4f;
		}
		int collisionGridIndexFromWorldPos = this.GetCollisionGridIndexFromWorldPos(worldPosition);
		return 0.4f + this.movementCurve.Duration() + this.moveToFrenzyCurve.Duration() + this.frenzyCurve.Duration() * this.normalizedCollisionDelays[collisionGridIndexFromWorldPos];
	}

	private int GetCollisionGridIndexFromWorldPos(Vector3 worldPosition)
	{
		return this.GetCollisionGridIndex(this.collisionArea.transform.InverseTransformPoint(worldPosition));
	}

	private int GetCollisionGridIndex(Vector3 localPosition)
	{
		Rect rectInLocalPos = this.collisionArea.GetRectInLocalPos();
		Vector2 vector = new Vector2(localPosition.x - rectInLocalPos.xMin, localPosition.y - rectInLocalPos.yMin);
		float num = rectInLocalPos.width / 22f;
		float num2 = rectInLocalPos.height / 21f;
		int num3 = Mathf.FloorToInt(vector.x / num);
		num3 = Mathf.Clamp(num3, 0, 21);
		int num4 = Mathf.FloorToInt(vector.y / num2);
		num4 = Mathf.Clamp(num4, 0, 20);
		return num4 * 22 + num3;
	}

	public Vector2 GetMidPointFromIndex(int index)
	{
		Rect rectInLocalPos = this.collisionArea.GetRectInLocalPos();
		float num = rectInLocalPos.width / 22f;
		float num2 = rectInLocalPos.height / 21f;
		int num3 = index % 22;
		int num4 = index / 22;
		return new Vector2(rectInLocalPos.xMin + (float)num3 * num + num * 0.5f, rectInLocalPos.yMin + (float)num4 * num2 + num2 * 0.5f);
	}

	public GameObject baitPivot;

	public AnimationCurve baitScaleCurve;

	public GameObject sharkBaitLabel;

	public GameObject sharkPivot;

	public Transform movementStart;

	public Transform movementEnd;

	public Transform frenzyStart;

	public Transform frenzyEnd;

	public Transform moveOut;

	public AnimationCurve movementCurve;

	public AnimationCurve moveToFrenzyCurve;

	public AnimationCurve frenzyCurve;

	public AnimationCurve moveOutCurve;

	public SkeletonAnimation sharkSpine;

	public float baitRemoveTime = 2f;

	public Transform collisionHeadBone;

	public Transform collisionTransform;

	public UIElement collisionArea;

	public float[] normalizedCollisionDelays;

	public const int collisionGridWidth = 22;

	public const int collisionGridHeight = 21;

	public ParticleSystem biteParticleSystem;

	private float rotationMaxAngle;

	private const string sharkAnimation = "animation";
}
