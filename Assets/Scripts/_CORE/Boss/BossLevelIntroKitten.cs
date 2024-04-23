using System;
using System.Collections;
using Spine;
using UnityEngine;

public class BossLevelIntroKitten : MonoBehaviour
{
	public Vector3 SpineRotation
	{
		set
		{
			this.spinePivot.eulerAngles = value;
		}
	}

	public void PlayHappyAnimation()
	{
		this.skeletonAnimation.AnimationState.SetAnimation(0, "KittenBoss_happyIdle", true);
		this.skeletonAnimation.AnimationState.GetCurrent(0).Time = UnityEngine.Random.Range(0f, this.skeletonAnimation.AnimationState.GetCurrent(0).EndTime);
	}

	public IEnumerator PlayIntroAnimation()
	{
		TrackEntry trackEntry = this.skeletonAnimation.AnimationState.SetAnimation(0, "KittenBoss_Surprised", false);
		yield return FiberHelper.Wait(trackEntry.EndTime, (FiberHelper.WaitFlag)0);
		this.skeletonAnimation.AnimationState.SetAnimation(0, "KittenBoss_SadIdle", true);
		this.skeletonAnimation.AnimationState.GetCurrent(0).Time = UnityEngine.Random.Range(0f, this.skeletonAnimation.AnimationState.GetCurrent(0).EndTime);
		yield break;
	}

	public void StartSuckUp()
	{
		this.shadowPivot.SetActive(false);
	}

	public void SetAlpha(float alpha)
	{
		Color color = Color.Lerp(Color.clear, Color.white, alpha);
		this.skeletonAnimation.skeleton.SetColor(color);
	}

	private const string HAPPY_IDLE = "KittenBoss_happyIdle";

	private const string SAD_IDLE = "KittenBoss_SadIdle";

	private const string SURPRISED = "KittenBoss_Surprised";

	[SerializeField]
	private SkeletonAnimation skeletonAnimation;

	[SerializeField]
	private GameObject shadowPivot;

	[SerializeField]
	private Transform spinePivot;
}
