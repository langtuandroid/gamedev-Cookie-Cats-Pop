using System;
using System.Collections;
using Fibers;
using UnityEngine;

public class ShieldVisuals : MonoBehaviour
{
	public void Initialize(LevelSession levelSession)
	{
		this.levelSession = levelSession;
		levelSession.ShieldActivated += this.StopAnySpineBlink;
		levelSession.ShieldActivated += this.HandleShieldActivated;
		levelSession.ShieldModified += this.HandleShieldModified;
		base.gameObject.SetActive(false);
	}

	public void Destroy()
	{
		if (this.fiber != null)
		{
			Fiber.TerminateIfAble(this.fiber);
			if (this.ballQueue != null)
			{
				this.ballQueue.MinusPieceMoveFinished -= this.HandleMinusPieceMoveFinished;
				this.ballQueue.DeathPieceMoveFinished -= this.HandleDeathPieceMoveFinished;
			}
		}
	}

	private void StartSpineBlink()
	{
		this.spine.gameObject.GetComponent<SpineColorPulsator>().enabled = true;
	}

	private void StopAnySpineBlink()
	{
		this.spine.gameObject.GetComponent<SpineColorPulsator>().enabled = false;
	}

	private IEnumerator AnimateUmbrellaAppear()
	{
		base.gameObject.SetActive(true);
		yield return FiberAnimation.ScaleTransform(this.scaler, Vector3.zero, Vector3.one, this.scaleCurve, 0.4f);
		this.spine.PlayAnimation(0, "animation", false, true);
		this.ballQueue.MinusPieceMoveFinished += this.HandleMinusPieceMoveFinished;
		this.ballQueue.DeathPieceMoveFinished += this.HandleDeathPieceMoveFinished;
		this.ballQueue.minusPieceTarget = this.pieceTarget;
		yield break;
	}

	private IEnumerator AnimateUmbrellaDisappear()
	{
		while (this.ballQueue.IsAnimatingEffect)
		{
			yield return null;
		}
		EffectPool.Instance.SpawnEffect("BubblePopSmoke", this.spine.transform.position + new Vector3(-20f, 80f, 0f), base.gameObject.layer, new object[0]);
		EffectPool.Instance.SpawnEffect("BubblePopSmoke", this.spine.transform.position + new Vector3(20f, 80f, 0f), base.gameObject.layer, new object[0]);
		yield return FiberAnimation.ScaleTransform(this.scaler, Vector3.one, Vector3.zero, this.scaleCurve, 0.4f);
		this.spine.PlayAnimation(0, "animation", false, true);
		base.gameObject.SetActive(false);
		this.ballQueue.MinusPieceMoveFinished -= this.HandleMinusPieceMoveFinished;
		this.ballQueue.DeathPieceMoveFinished -= this.HandleDeathPieceMoveFinished;
		this.ballQueue.minusPieceTarget = null;
		yield break;
	}

	private void HandleShieldActivated()
	{
		this.levelSession.ShieldActivated -= this.HandleShieldActivated;
		this.levelSession.ShieldDeactivated += this.HandleShieldDeactivated;
		this.fiber.Terminate();
		this.fiber.Start(this.AnimateUmbrellaAppear());
	}

	private void HandleShieldDeactivated()
	{
		this.levelSession.ShieldActivated += this.HandleShieldActivated;
		this.levelSession.ShieldDeactivated -= this.HandleShieldDeactivated;
		this.fiber.Start(this.AnimateUmbrellaDisappear());
	}

	private void HandleShieldModified(LevelSession.ShieldModification modification)
	{
		if (modification == LevelSession.ShieldModification.Blink)
		{
			this.StartSpineBlink();
		}
	}

	private void HandleMinusPieceMoveFinished()
	{
		this.spine.PlayAnimation(0, "hit", false, true);
		EffectPool.Instance.SpawnEffect("BubblePopSmoke", this.pieceTarget.position, base.gameObject.layer, new object[0]);
	}

	private void HandleDeathPieceMoveFinished()
	{
		this.spine.PlayAnimation(0, "hit", false, true);
		EffectPool.Instance.SpawnEffect("InkSplatter", this.pieceTarget.position, base.gameObject.layer, new object[0]);
	}

	public Transform scaler;

	public AnimationCurve scaleCurve;

	public SkeletonAnimation spine;

	public Transform pieceTarget;

	public CannonBallQueue ballQueue;

	private Fiber fiber = new Fiber();

	private LevelSession levelSession;
}
