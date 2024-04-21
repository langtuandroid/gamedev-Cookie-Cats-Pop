using System;
using System.Collections;
using Fibers;
using UnityEngine;

public class TutorialFinger : MonoBehaviour
{
	public void Begin(Vector3 worldPos, GameCannon cannon, bool slowAiming, IEnumerator customWaitCode = null)
	{
		this.worldPos = worldPos;
		this.cannon = cannon;
		this.slowAiming = slowAiming;
		base.gameObject.SetActive(true);
		this.fingerPing.gameObject.SetActive(false);
		this.UpdateFingerSprites(TutorialFinger.FingerState.Disabled);
		this.fiber.Start(this.Logic(customWaitCode));
	}

	public void End()
	{
		this.fiber.Terminate();
		base.gameObject.SetActive(false);
	}

	private void Awake()
	{
		this.fingerSpriteUp.gameObject.SetActive(false);
	}

	private void OnDestroy()
	{
		this.fiber.Terminate();
	}

	private void UpdateFingerSprites(TutorialFinger.FingerState state)
	{
		this.fingerSpriteUp.gameObject.SetActive(state == TutorialFinger.FingerState.Up);
		this.fingerSpriteDown.gameObject.SetActive(state == TutorialFinger.FingerState.Down);
		this.fingerPing.gameObject.SetActive(state != TutorialFinger.FingerState.Disabled);
	}

	private void Update()
	{
		this.fiber.Step();
		if (Input.GetMouseButtonDown(0))
		{
			this.fiber.Terminate();
		}
		if (!Input.GetMouseButton(0))
		{
			if (this.fiber.IsTerminated)
			{
				this.fiber.Start(this.Logic(null));
			}
			if (this.cannon != null && this.enableTracer)
			{
				this.cannon.CalculateAim(base.transform.position);
			}
		}
		this.fiber.Step();
	}

	private IEnumerator Logic(IEnumerator customWaitCode)
	{
		if (customWaitCode != null)
		{
			this.UpdateFingerSprites(TutorialFinger.FingerState.Disabled);
			yield return customWaitCode;
		}
		yield return new Fiber.OnExit(delegate()
		{
			this.UpdateFingerSprites(TutorialFinger.FingerState.Disabled);
			this.enableTracer = false;
			if (this.cannon != null && !Input.GetMouseButton(0))
			{
				this.cannon.CalculateAim(Vector3.down * 10000f);
				this.cannon.EndAiming();
			}
		});
		yield return FiberHelper.Wait(0.5f, (FiberHelper.WaitFlag)0);
		this.UpdateFingerSprites(TutorialFinger.FingerState.Up);
		base.transform.position = this.worldPos;
		yield return UIFiberAnimations.FadeAlpha(this.fingerSpriteUp, 0f, 1f, 0.2f, null);
		yield return FiberHelper.Wait(0.5f, (FiberHelper.WaitFlag)0);
		for (;;)
		{
			this.UpdateFingerSprites(TutorialFinger.FingerState.Down);
			this.enableTracer = true;
			if (this.cannon != null)
			{
				this.cannon.CalculateAim(base.transform.position);
				this.cannon.StartAiming();
			}
			this.fingerPing.Play();
			yield return FiberHelper.Wait(0.5f, (FiberHelper.WaitFlag)0);
			if (this.cannon != null)
			{
				AnimationCurve curve = (!this.slowAiming) ? this.wiggleCurve : this.bigWiggleCurve;
				yield return FiberAnimation.MoveTransform(base.transform, this.worldPos, this.worldPos + Vector3.right, curve, 0f);
			}
			else
			{
				yield return FiberHelper.Wait(0.5f, (FiberHelper.WaitFlag)0);
			}
			this.enableTracer = false;
			if (this.cannon != null)
			{
				this.cannon.CalculateAim(Vector3.down * 10000f);
				this.cannon.EndAiming();
			}
			this.UpdateFingerSprites(TutorialFinger.FingerState.Up);
			yield return FiberHelper.Wait(0.5f, (FiberHelper.WaitFlag)0);
			yield return FiberHelper.Wait(1f, (FiberHelper.WaitFlag)0);
		}
		yield break;
	}

	public UIWidget fingerSpriteUp;

	public UIWidget fingerSpriteDown;

	public ParticleSystem fingerPing;

	public AnimationCurve wiggleCurve;

	public AnimationCurve bigWiggleCurve;

	private Vector3 worldPos;

	private GameCannon cannon;

	private bool enableTracer;

	private bool slowAiming;

	private Fiber fiber = new Fiber(FiberBucket.Manual);

	public enum FingerState
	{
		Disabled,
		Up,
		Down
	}
}
