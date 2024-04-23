using System;
using System.Collections;
using Fibers;
using JetBrains.Annotations;
using UnityEngine;

namespace TactileModules.PuzzleGame.SlidesAndLadders.UI
{
	public class SlidesAndLaddersWheelWidget : ExtensibleVisual<ISlidesAndLaddersWheelWidget>
	{
		public Action OnWheelClicked { get; set; }

		public Action OnAnimatingIsDone { get; set; }

		public void SetWheelState(bool canSpinWheel)
		{
			this.wheelReady = canSpinWheel;
			if (canSpinWheel)
			{
				this.wheel.Color = Color.white;
				this.wheelPin.Color = Color.white;
			}
			else
			{
				this.wheel.Color = this.wheelUnavailableColor;
				this.wheelPin.Color = this.wheelUnavailableColor;
				base.transform.localScale = new Vector3(1f, 1f, 0.987f);
			}
		}

		[UsedImplicitly]
		private void WheelClicked(UIEvent e)
		{
			if (this.OnWheelClicked != null)
			{
				this.OnWheelClicked();
			}
		}

		public void AnimateWheelToAngle(Vector3 endAngle)
		{
			this.wheelFiber.Start(this.AnimateToAngleCr(endAngle));
		}

		private IEnumerator AnimateToAngleCr(Vector3 endAngle)
		{
			this.spinning = true;
			base.transform.localScale = new Vector3(1f, 1f, 0.987f);
			Vector3 startAngle = this.wheel.transform.localEulerAngles;
			this.wheelBlurFiber.Start(this.AnimateBlurEffect(1.4f));
			if (base.Extension != null)
			{
				base.Extension.PlayWheelSpinningSound();
			}
			yield return FiberAnimation.RotateTransform(this.wheel.transform, startAngle, new Vector3(0f, 0f, -360f) + startAngle, this.wheelStartAnimationCurve, 0.4f);
			for (int i = 0; i < 4; i++)
			{
				yield return FiberAnimation.RotateTransform(this.wheel.transform, startAngle, new Vector3(0f, 0f, -360f) + startAngle, this.wheelSpinningAnimationCurve, 0.2f);
			}
			yield return FiberAnimation.RotateTransform(this.wheel.transform, startAngle, new Vector3(0f, 0f, -360f) + endAngle, this.wheelEndAnimationCurve, 0.65f);
			yield return FiberHelper.Wait(0.4f, (FiberHelper.WaitFlag)0);
			if (base.Extension != null)
			{
				base.Extension.PlayWheelStoppedSound();
			}
			if (this.OnAnimatingIsDone != null)
			{
				this.OnAnimatingIsDone();
			}
			this.spinning = false;
			yield break;
		}

		private IEnumerator AnimateBlurEffect(float duration)
		{
			float time = 0f;
			float timeScale = 1f / duration;
			while (time < duration)
			{
				this.wheelBlur.Alpha = this.wheelAlphaCurve.Evaluate(time * timeScale);
				this.wheelBlur.transform.localEulerAngles = this.wheel.transform.localEulerAngles;
				time += Time.deltaTime;
				yield return FiberHelper.Wait(Time.deltaTime, (FiberHelper.WaitFlag)0);
			}
			this.wheelBlur.Alpha = 0f;
			yield break;
		}

		public void WheelAvailableEffect()
		{
			if (!this.wheelParticleSystem.isPlaying)
			{
				this.wheelParticleSystem.Clear();
				this.wheelParticleSystem.Play();
			}
		}

		private void Update()
		{
			if (this.wheelReady && !this.spinning)
			{
				float num = 0.01f;
				float f = Time.timeSinceLevelLoad * (1f + this.animationOffset * 0.1f) * (4f + this.animationOffset) + (base.transform.position.x + base.transform.position.y) * 0.1f;
				base.transform.localScale = new Vector3(1f + Mathf.Sin(f) * num * this.animationOffset, 1f + Mathf.Cos(f) * num * this.animationOffset, 0.987f);
			}
		}

		private void OnDisable()
		{
			this.wheelFiber.Terminate();
			this.wheelStateFiber.Terminate();
			this.wheelBlurFiber.Terminate();
		}

		[SerializeField]
		private UIResourceQuad wheel;

		[SerializeField]
		private UIResourceQuad wheelBlur;

		[SerializeField]
		private UIResourceQuad wheelPin;

		[SerializeField]
		private AnimationCurve wheelStartAnimationCurve;

		[SerializeField]
		private AnimationCurve wheelEndAnimationCurve;

		[SerializeField]
		private AnimationCurve wheelSpinningAnimationCurve;

		[SerializeField]
		private AnimationCurve wheelAlphaCurve;

		[SerializeField]
		private ParticleSystem wheelParticleSystem;

		[SerializeField]
		private Color wheelUnavailableColor;

		[SerializeField]
		private float animationOffset;

		private readonly Fiber wheelFiber = new Fiber();

		private readonly Fiber wheelStateFiber = new Fiber();

		private readonly Fiber wheelBlurFiber = new Fiber();

		private bool wheelReady;

		private bool spinning;
	}
}
