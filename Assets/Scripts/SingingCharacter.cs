using System;
using Fibers;
using Spine;
using UnityEngine;

public class SingingCharacter : AudioAnalyser
{
	public SkeletonAnimation Spine { get; private set; }

	public UISprite Shadow
	{
		get
		{
			return this.shadow;
		}
	}

	public MultiTrack.Voice Voice { get; private set; }

	public string SingerId
	{
		get
		{
			return this.singerId;
		}
	}

	public SingerInfo SingerInfo
	{
		get
		{
			return SingletonAsset<SingerDatabase>.Instance.FindInfo(this.singerId);
		}
	}

	public void SetVoice(MultiTrack song, MultiTrack.Voice voice)
	{
		this.Voice = voice;
		this.lowFFTBand = SingletonAsset<MultiTrackDatabase>.Instance.lowBand;
		this.highFFTBand = SingletonAsset<MultiTrackDatabase>.Instance.highBand;
		this.mouthReactionValue.coolDownThreshold = song.mouthThreshold;
		this.bodyReactionValue.coolDownThreshold = song.mouthThreshold;
		base.AudioSource.clip = ((voice == null) ? null : voice.clip);
	}

	protected override void Update()
	{
		base.Update();
		if (this.Spine != null && !this.isSingingStarted)
		{
			Spine.Animation animation = this.Spine.skeletonDataAsset.GetSkeletonData(true).FindAnimation(this.openCloseAnimName);
			if (animation != null)
			{
				animation.Apply(this.Spine.skeleton, 0f, 1f, false, null);
				animation.Apply(this.Spine.skeleton, 1f, 0f, false, null);
			}
		}
	}

	protected override void SingingStarted()
	{
		base.SingingStarted();
		this.isSingingStarted = true;
	}

	protected override void SingingStopped()
	{
		base.SingingStopped();
		this.isSingingStarted = false;
	}

	protected override bool IsAudioEnabled
	{
		get
		{
			return AudioManager.Instance.SoundEffectsActive || AudioManager.Instance.MusicActive;
		}
	}

	protected override void SingingUpdated(float rawInputLevel)
	{
		if (!this.basicAnimFiber.IsTerminated)
		{
			this.basicAnimFiber.Terminate();
		}
		this.bodyReactionValue.Feed(Mathf.Clamp01(rawInputLevel * rawInputLevel * 5f));
		this.mouthReactionValue.Feed(rawInputLevel);
		float value = this.bodyReactionValue.Value;
		this.squashStretchPivot.transform.localScale = Vector3.Lerp(Vector3.one, new Vector3(0.85f, 1.2f, 1f), value);
		if (this.Spine != null)
		{
			Spine.Animation animation = this.Spine.skeletonDataAsset.GetSkeletonData(true).FindAnimation(this.openCloseAnimName);
			if (animation != null)
			{
				float time = 0f;
				if (this.mouthReactionValue.Value > this.mouthReactionValue.coolDownThreshold)
				{
					time = animation.duration;
				}
				animation.Apply(this.Spine.skeleton, this.lastTime, time, false, null);
				this.lastTime = time;
			}
		}
	}

	public void SetSinger(string singerId)
	{
		this.singerId = singerId;
		Transform transform = SingletonAsset<SingerDatabase>.Instance.LoadPrefabFromResources(singerId);
		if (transform == null)
		{
			if (Application.isPlaying)
			{
			}
			return;
		}
		if (this.Spine != null)
		{
			if (Application.isPlaying)
			{
				UnityEngine.Object.Destroy(this.Spine.gameObject);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(this.Spine.gameObject);
			}
		}
		this.Spine = UnityEngine.Object.Instantiate<Transform>(transform).GetComponent<SkeletonAnimation>();
		this.Spine.transform.parent = this.squashStretchPivot;
		this.Spine.transform.localPosition = Vector3.back;
		this.Spine.transform.localScale = Vector3.one;
		this.Spine.transform.gameObject.SetLayerRecursively(base.gameObject.layer);
		this.shadow.gameObject.SetActive(true);
	}

	public void HideShadow()
	{
		this.shadow.gameObject.SetActive(false);
	}

	private void OnDisable()
	{
		if (this.animationFiber != null)
		{
			this.animationFiber.Terminate();
		}
	}

	[SerializeField]
	private Transform scalePivot;

	[SerializeField]
	private UISprite shadow;

	public Transform squashStretchPivot;

	public Transform dropPivot;

	public string openCloseAnimName = "Singing";

	private float dipValue;

	private AudioAnalyser.SmoothingValue bodyReactionValue = new AudioAnalyser.SmoothingValue(5f, 20f, 0f, 0f);

	private AudioAnalyser.SmoothingValue mouthReactionValue = new AudioAnalyser.SmoothingValue(200f, 0f, 0.1f, 0.05f);

	private Fiber basicAnimFiber = new Fiber();

	private float lastTime;

	private Fiber animationFiber = new Fiber(FiberBucket.Manual);

	private string singerId;

	private bool isSingingStarted;
}
