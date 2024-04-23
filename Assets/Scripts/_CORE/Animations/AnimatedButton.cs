using System;
using System.Collections;
using Fibers;
using UnityEngine;

[RequireComponent(typeof(UIButton))]
public class AnimatedButton : MonoBehaviour
{
	[Instantiator.SerializeProperty]
	public int Mood
	{
		get
		{
			return (int)this.mood;
		}
		set
		{
			this.mood = (AnimatedButton.ButtonMood)value;
		}
	}

	[Instantiator.SerializeProperty(true)]
	public bool ConstantWobble
	{
		get
		{
			return this.constantWobble;
		}
		set
		{
			this.constantWobble = value;
			this.ReflectState();
		}
	}

	[Instantiator.SerializeProperty(false)]
	public bool Disabled
	{
		get
		{
			return !this.UIButton.enabled;
		}
		set
		{
			this.UIButton.enabled = !value;
			this.ReflectState();
		}
	}

	private void Awake()
	{
		this.UIButton.clickType = this.UIButton.clickType;
	}

	private UIButton UIButton
	{
		get
		{
			if (this.cachedUIButton == null)
			{
				this.cachedUIButton = this.GetButton();
				this.cachedUIButton.Clicked += this.HandleButtonClicked;
			}
			return this.cachedUIButton;
		}
	}

	private void ViewWillAppear()
	{
		this.animationOffset = UnityEngine.Random.value;
	}

	protected virtual void HandleButtonClicked(UIButton b)
	{
        
		this.animFiber.Start(this.AnimateClicked());
		UISetup instance = SingletonAsset<UISetup>.Instance;
		AnimatedButton.ButtonMood buttonMood = this.mood;
		if (buttonMood != AnimatedButton.ButtonMood.Confirm)
		{
			if (buttonMood != AnimatedButton.ButtonMood.Dismiss)
			{
				instance.soundButtonNormal.Play();
			}
			else
			{
				instance.soundButtonDismiss.Play();
			}
		}
		else
		{
			instance.soundButtonConfirm.Play();
		}
	}

	protected virtual void ReflectState()
	{
		base.transform.localScale = new Vector3(1f, 1f, 0.987f);
	}

	private void OnDisable()
	{
		this.animFiber.Terminate();
	}

	private void Update()
	{
		if (this.animFiber.IsTerminated && this.constantWobble && !this.Disabled)
		{
			float num = 0.01f;
			float f = Time.timeSinceLevelLoad * (1f + this.animationOffset * 0.1f) * (4f + this.animationOffset) + (base.transform.position.x + base.transform.position.y) * 0.1f;
			base.transform.localScale = new Vector3(1f + Mathf.Sin(f) * num, 1f + Mathf.Cos(f) * num, 0.987f);
		}
		this.animFiber.Step();
	}

	public void PrepareAppearAnim()
	{
		base.transform.localScale = Vector3.zero;
	}

	public void PlayAppearAnim(float delay)
	{
		this.animFiber.Start(this.WobbleAnim(delay));
	}

	private IEnumerator WobbleAnim(float delay)
	{
		yield return new Fiber.OnExit(delegate()
		{
			this.transform.localScale = new Vector3(1f, 1f, 0.987f);
			this.transform.localRotation = Quaternion.identity;
		});
		Vector3 wobble = new Vector3(0.5f, 1.5f, 0.987f);
		base.transform.localScale = Vector3.zero;
		yield return FiberHelper.Wait(delay, (FiberHelper.WaitFlag)0);
		yield return FiberAnimation.Animate(0f, SingletonAsset<UISetup>.Instance.buttonWobblyScaleIn, delegate(float t)
		{
			float d = Mathf.Clamp01(t * 2f);
			this.transform.localScale = FiberAnimation.LerpNoClamp(wobble * d, Vector3.one, t);
		}, false);
		yield break;
	}

	private IEnumerator AnimateClicked()
	{
		yield return FiberAnimation.ScaleTransform(base.transform, Vector3.one * 0.7f, Vector3.one, SingletonAsset<UISetup>.Instance.buttonWobblyPress, 0f);
		yield break;
	}

	[SerializeField]
	private bool constantWobble = true;

	[SerializeField]
	private AnimatedButton.ButtonMood mood;

	private float animationOffset;

	private Fiber animFiber = new Fiber(FiberBucket.Manual);

	private UIButton cachedUIButton;

	public enum ButtonMood
	{
		Neutral,
		Confirm,
		Dismiss
	}
}
