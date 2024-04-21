using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using JetBrains.Annotations;
using NinjaUI;
using TactileModules.Foundation;
using TactileModules.PuzzleGames.GameCore;
using UnityEngine;

public class LevelResultView : UIView
{
	private SingingBand Band
	{
		get
		{
			return this.singingBand.GetInstance<SingingBand>();
		}
	}

	private CloudClient CloudClient
	{
		get
		{
			return ManagerRepository.Get<CloudClient>();
		}
	}

	protected override void ViewLoad(object[] parameters)
	{
		this.session = (parameters[0] as LevelSession);
		this.scoreLabel.text = this.session.Points.ToString();
		if (this.session.FirstTimeCompleted)
		{
			this.buttonMap.SetActive(false);
			this.buttonRetry.SetActive(false);
			this.buttonNext.transform.parent = this.centerButton;
			this.buttonNext.transform.localPosition = Vector3.zero;
		}
		this.SetDefaults();
		AudioManager.Instance.SetMusic(null, true);
		HighscorePanel instance = this.highScorePanel.GetInstance<HighscorePanel>();
		instance.Initialize(this.session.Level);
		instance.UIUpdated = new Action(this.UpdateButtonsPosition);
		bool isHardLevel = this.session.Level.LevelDifficulty == LevelDifficulty.Hard;
		for (int i = 0; i < this.stars.Length; i++)
		{
			this.stars[i].Initialize(isHardLevel);
		}
		this.fiberRunner = new FiberRunner(FiberBucket.Manual);
		int num = Mathf.Clamp(this.session.Level.NumberOfStarsFromPoints(this.session.Points), 1, 3);
		this.kittenArea.Initialize(this.numberOfKittensPerStar[num - 1]);
	}

	protected override void ViewDidAppear()
	{
		this.Band.ConfigureSingers(false, this.GetRandomBand());
		this.Band.SetMultiTrack(SingletonAsset<MultiTrackDatabase>.Instance.GetRandomVictorySong());
		this.fiber.Start(this.Animate());
		this.UpdateButtonsPosition();
	}

	protected override void ViewDidDisappear()
	{
		Fiber.TerminateIfAble(this.fiber);
	}

	private void Update()
	{
		if (this.fiberRunner != null)
		{
			this.fiberRunner.Step();
		}
	}

	private void ApplyFirstFrameOfAnimation(Animation anim)
	{
		anim.Play();
		IEnumerator enumerator = anim.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				AnimationState animationState = (AnimationState)obj;
				animationState.time = 0.01f;
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		anim.Sample();
		anim.Stop();
	}

	private void SetDefaults()
	{
		this.scoreLabel.text = string.Empty;
		for (int i = 0; i < 3; i++)
		{
			this.stars[i].SetActive(false);
		}
		foreach (GameObject gameObject in this.hideUntilAnimated)
		{
			gameObject.SetActive(false);
		}
		this.ApplyFirstFrameOfAnimation(this.appearAnimation);
		this.ApplyFirstFrameOfAnimation(this.endUIAnimation);
	}

	private IEnumerator WaitForAnimation(Animation anim)
	{
		while (anim.isPlaying)
		{
			yield return null;
		}
		yield break;
	}

	private IEnumerator Animate()
	{
		int target = this.session.Points;
		int numStars = this.session.Level.NumberOfStarsFromPoints(target);
		this.SetDefaults();
		this.appearAnimation.Play();
		yield return this.WaitForAnimation(this.appearAnimation);
		yield return FiberHelper.RunParallel(new IEnumerator[]
		{
			this.AnimatePoints(target, numStars),
			this.AnimateStars(numStars)
		});
		this.FinalStep();
		yield break;
	}

	private IEnumerator AnimateStars(int num)
	{
		Transform cameraTransform = base.ViewCamera.transform;
		SingletonAsset<SoundDatabase>.Instance.levelResultStar.ResetSequential();
		IEnumerator[] enums = new IEnumerator[num];
		for (int i = 0; i < num; i++)
		{
			enums[i] = this.stars[i].Animate(SingletonAsset<LevelVisuals>.Instance.levelResultVisuals.starAnimationDuration * (float)i, delegate
			{
				SingletonAsset<SoundDatabase>.Instance.levelResultStar.PlaySequential();
				this.fiberRunner.Run(FiberAnimation.ShakeLocalPosition(cameraTransform, cameraTransform.localPosition, null, 0.3f, 10f, 20f, 0f), false);
			});
		}
		yield return FiberHelper.RunParallel(enums);
		yield break;
	}

	private IEnumerator AnimatePoints(int target, int numStars)
	{
		yield return FiberAnimation.Animate(SingletonAsset<LevelVisuals>.Instance.levelResultVisuals.starHitDelay + (float)(numStars - 1) * SingletonAsset<LevelVisuals>.Instance.levelResultVisuals.starAnimationDuration, delegate(float t)
		{
			this.scoreLabel.text = L.FormatNumber(Mathf.RoundToInt((float)target * t));
		});
		yield break;
	}

	private void FinalStep()
	{
		SingletonAsset<SoundDatabase>.Instance.levelResultFireworks.Play();
		foreach (GameObject gameObject in this.hideUntilAnimated)
		{
			gameObject.SetActive(true);
		}
		this.endUIAnimation.Play();
		this.Band.StartSinging(false);
	}

	private void UpdateButtonsPosition()
	{
		this.buttonsPivot.localPosition = ((!this.CloudClient.HasValidUser) ? this.notLoggedInButtonsLocation.localPosition : this.loggedInButtonsLocation.localPosition);
	}

	private List<string> GetRandomBand()
	{
		int num = this.session.Level.NumberOfStarsFromPoints(this.session.Points);
		int num2 = (num > 1) ? ((num != 2) ? 4 : 2) : 1;
		List<string> defaultBand = SingletonAsset<SingerDatabase>.Instance.GetDefaultBand();
		defaultBand.Shuffle<string>();
		while (defaultBand.Count > num2)
		{
			defaultBand.RemoveAt(defaultBand.Count - 1);
		}
		defaultBand.Shuffle<string>();
		return defaultBand;
	}

	[UsedImplicitly]
	protected void DismissClicked(UIEvent e)
	{
		base.Close(PostLevelPlayedAction.Exit);
	}

	[UsedImplicitly]
	protected void RetryButtonClicked(UIEvent e)
	{
		base.Close(PostLevelPlayedAction.Retry);
	}

	[UsedImplicitly]
	protected void NextButtonClicked(UIEvent e)
	{
		base.Close(PostLevelPlayedAction.NextLevel);
	}

	[SerializeField]
	private UILabel scoreLabel;

	[SerializeField]
	private LevelResultView.AnimatedStar[] stars;

	[SerializeField]
	private GameObject[] hideUntilAnimated;

	[SerializeField]
	private GameObject buttonMap;

	[SerializeField]
	private GameObject buttonRetry;

	[SerializeField]
	private GameObject buttonNext;

	[SerializeField]
	private Transform centerButton;

	[SerializeField]
	private UIInstantiator highScorePanel;

	[SerializeField]
	private UIInstantiator singingBand;

	[SerializeField]
	private Transform notLoggedInButtonsLocation;

	[SerializeField]
	private Transform loggedInButtonsLocation;

	[SerializeField]
	private Transform buttonsPivot;

	[SerializeField]
	private Animation appearAnimation;

	[SerializeField]
	private Animation endUIAnimation;

	[SerializeField]
	private KittenArea kittenArea;

	[SerializeField]
	private int[] numberOfKittensPerStar;

	private readonly Fiber fiber = new Fiber();

	private FiberRunner fiberRunner;

	private LevelSession session;

	[Serializable]
	public class AnimatedStar
	{
		public void SetActive(bool active)
		{
			this.star.gameObject.SetActive(active);
		}

		public void Initialize(bool isHardLevel)
		{
			this.endPosition = this.star.transform.localPosition;
			this.star.SpriteName = ((!isHardLevel) ? this.normalSpriteName : this.hardSpriteName);
		}

		public IEnumerator Animate(float delay, Action starHit)
		{
			LevelVisuals.LevelResultVisuals v = SingletonAsset<LevelVisuals>.Instance.levelResultVisuals;
			yield return FiberHelper.Wait(delay, (FiberHelper.WaitFlag)0);
			this.SetActive(true);
			this.colorPulsator.enabled = false;
			yield return FiberHelper.RunParallel(new IEnumerator[]
			{
				FiberHelper.RunDelayed(v.starHitDelay, delegate
				{
					this.starGlitter.Play();
					this.confetti.Play();
					starHit();
				}),
				FiberAnimation.MoveLocalTransform(this.star.transform, this.start.localPosition, this.endPosition, v.starMoveCurve, 0f),
				FiberAnimation.ScaleTransform(this.star.transform, Vector3.zero, Vector3.one, v.starScaleCurve, 0f),
				FiberAnimation.Animate(0f, v.starColorCurve, delegate(float t)
				{
					this.star.Color = Color.Lerp(new Color(0.5f, 0.5f, 0.5f, 0.5f), new Color(1f, 1f, 1f, 0.5f), t);
				}, false),
				FiberAnimation.Animate(0f, v.glowCurve, delegate(float t)
				{
					this.glow.Alpha = t;
				}, false)
			});
			this.colorPulsator.enabled = true;
			yield break;
		}

		public UISprite star;

		public UIWidget glow;

		public Transform start;

		public ColorPulsator colorPulsator;

		public ParticleSystem starGlitter;

		public ParticleSystem confetti;

		[UISpriteName]
		public string normalSpriteName;

		[UISpriteName]
		public string hardSpriteName;

		private Vector3 endPosition;
	}
}
