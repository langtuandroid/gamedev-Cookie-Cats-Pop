using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossKittenController
{
	public BossKittenController(BossCharacterController bossCharacterController, BossLevelGameBoard gameBoard, BossLevelAreas bossLevelAreas)
	{
		this.bossCharacterController = bossCharacterController;
		this.gameBoard = gameBoard;
		this.bossLevelAreas = bossLevelAreas;
		this.bossLevelAreas.EnableKittens(this.KittenCount);
		for (int i = 0; i < this.KittenCount; i++)
		{
			BossLevelIntroKitten kitten = bossLevelAreas.GetKitten(i);
			kitten.PlayHappyAnimation();
		}
	}

	private int KittenCount
	{
		get
		{
			return this.gameBoard.BossStagesLeft;
		}
	}

	public IEnumerator GetSuckUpKittensAnimation()
	{
		float interval = (BossLevelDatabase.Database.allKittensSuckUpDuration - BossLevelDatabase.Database.singleKittenSuckUpFullDuration) / ((float)(this.KittenCount - 1) + float.Epsilon);
		List<IEnumerator> suckUpKittensAnimations = new List<IEnumerator>();
		for (int i = 0; i < this.KittenCount; i++)
		{
			float delay = interval * (float)i;
			BossLevelIntroKitten kitten = this.bossLevelAreas.GetKitten(i);
			suckUpKittensAnimations.Add(this.SuckUpKitten(kitten, delay));
		}
		yield return FiberHelper.RunParallel(suckUpKittensAnimations);
		yield break;
	}

	private IEnumerator SuckUpKitten(BossLevelIntroKitten kitten, float delay)
	{
		yield return FiberHelper.Wait(delay, (FiberHelper.WaitFlag)0);
		SingletonAsset<SoundDatabase>.Instance.bossKittenMeow.Play();
		SingletonAsset<SoundDatabase>.Instance.bossKittenSwoosh.Play();
		kitten.StartSuckUp();
		Vector3 startPos = kitten.transform.position;
		Vector3 endPos = this.bossCharacterController.Visuals.SuckPos;
		IEnumerator posAnim = FiberAnimation.Animate(BossLevelDatabase.Database.singleKittenSuckUpMoveDuration, delegate(float t)
		{
			Vector3 position = FiberAnimation.LerpNoClamp(startPos, endPos, t);
			position.y -= Mathf.Sin(t * 3.14159274f) * 20f;
			position.x += Mathf.Sin(t * 3.14159274f) * 120f;
			kitten.transform.position = position;
		});
		IEnumerator rotAnim = FiberAnimation.Animate(BossLevelDatabase.Database.singleKittenSuckUpFullDuration, delegate(float t)
		{
			float z = t * t * t * 360f * 4f;
			kitten.SpineRotation = new Vector3(0f, 0f, z);
		});
		Vector3 startScale = kitten.transform.localScale;
		Vector3 endScale = Vector3.one * BossLevelDatabase.Database.singleKittenSuckedUpScale;
		IEnumerator scaleAnim = FiberAnimation.Animate(BossLevelDatabase.Database.singleKittenSuckUpFullDuration, delegate(float t)
		{
			Vector3 localScale = Vector3.LerpUnclamped(startScale, endScale, t * t);
			kitten.transform.localScale = localScale;
		});
		float fadeDuration = BossLevelDatabase.Database.singleKittenSuckUpFullDuration - BossLevelDatabase.Database.singleKittenSuckUpFadeOutDelay;
		IEnumerator fadeAnim = FiberHelper.RunSerial(new IEnumerator[]
		{
			FiberHelper.Wait(BossLevelDatabase.Database.singleKittenSuckUpFadeOutDelay, (FiberHelper.WaitFlag)0),
			FiberAnimation.Animate(fadeDuration, delegate(float t)
			{
				kitten.SetAlpha(1f - t);
			})
		});
		yield return FiberHelper.RunParallel(new IEnumerator[]
		{
			posAnim,
			rotAnim,
			scaleAnim,
			fadeAnim
		});
		kitten.gameObject.SetActive(false);
		yield break;
	}

	public IEnumerator GetKittensSurprisedAnimation()
	{
		yield return FiberHelper.Wait(BossLevelDatabase.Database.introKittenSurpriseDelay, (FiberHelper.WaitFlag)0);
		float interval = (BossLevelDatabase.Database.allKittensSuckUpDuration - BossLevelDatabase.Database.singleKittenSuckUpFullDuration) / ((float)(this.KittenCount - 1) + float.Epsilon);
		List<IEnumerator> surprisedAnimations = new List<IEnumerator>();
		for (int i = 0; i < this.KittenCount; i++)
		{
			float delay = interval * (float)i;
			BossLevelIntroKitten kitten = this.bossLevelAreas.GetKitten(i);
			surprisedAnimations.Add(this.SurprisedKitten(kitten, delay));
		}
		IEnumerator delayedSurprisedSoundsAnim = FiberHelper.RunDelayed(BossLevelDatabase.Database.introKittenSurpriseDelaySoundDelay, delegate
		{
			SingletonAsset<SoundDatabase>.Instance.bossKittenUhOh.Play();
		});
		surprisedAnimations.Add(delayedSurprisedSoundsAnim);
		yield return FiberHelper.RunParallel(surprisedAnimations);
		yield break;
	}

	private IEnumerator SurprisedKitten(BossLevelIntroKitten kitten, float delay)
	{
		yield return FiberHelper.Wait(delay, (FiberHelper.WaitFlag)0);
		yield return kitten.PlayIntroAnimation();
		yield break;
	}

	private readonly BossCharacterController bossCharacterController;

	private readonly BossLevelGameBoard gameBoard;

	private readonly BossLevelAreas bossLevelAreas;
}
