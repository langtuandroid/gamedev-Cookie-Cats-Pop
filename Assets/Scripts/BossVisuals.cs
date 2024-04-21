using System;
using System.Collections;
using System.Diagnostics;
using Fibers;
using Spine;
using UnityEngine;

public class BossVisuals : MonoBehaviour
{
	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action DebugOnDoubleClick = delegate ()
    {
    };



    public Vector3 SuckPos
	{
		get
		{
			return this.suckPoint.position;
		}
	}

	public Vector3 SpawnBubblesPos
	{
		get
		{
			return this.spawnBubblesPoint.position;
		}
	}

	public Vector3 KittenSpawnPos
	{
		get
		{
			return this.kittenSpawnPoint.position;
		}
	}

	private void Awake()
	{
		this.SetDestructionLevel(0);
		this.EnableBubbleParticles(false);
		this.EnableMovementAnimation(false);
	}

	private void Start()
	{
		this.skeletonAnimation.AnimationState.SetAnimation(0, "Intro_Fly_up", true);
	}

	private void Update()
	{
		if (!this.animFiber.IsTerminated || !this.enableMovementAnimation)
		{
			return;
		}
		Vector2 vector = base.transform.position;
		Vector2 v = vector - this.lastFramePos;
		string animationName;
		if (vector == this.lastFramePos)
		{
			animationName = "Fly_up";
		}
		else if (Mathf.Abs(v.x) > Mathf.Abs(v.y))
		{
			float num = Vector3.Dot(v, Vector3.right);
			animationName = ((num <= 0f) ? "Fly_Left" : "Fly_Right");
		}
		else
		{
			float num2 = Vector3.Dot(v, Vector3.up);
			animationName = ((num2 <= 0f) ? "Fly_down" : "Fly_up");
		}
		this.skeletonAnimation.loop = true;
		this.skeletonAnimation.AnimationName = animationName;
	}

	private void LateUpdate()
	{
		this.lastFramePos = base.transform.position;
	}

	public void EnableMovementAnimation(bool enable)
	{
		this.enableMovementAnimation = enable;
	}

	public IEnumerator PlayHitAnimation()
	{
		SingletonAsset<SoundDatabase>.Instance.bossHit.Play();
		yield return this.PlayAnimation("Hit", 0f);
		yield break;
	}

	public IEnumerator PlayBubblesAnimation()
	{
		yield return this.PlayAnimation("Bubbles", 0.1f);
		this.EnableBubbleParticles(false);
		yield break;
	}

	public IEnumerator PlayDieAnimation()
	{
		SingletonAsset<SoundDatabase>.Instance.bossDestroyed.Play();
		this.EnableMovementAnimation(false);
		yield return this.PlayAnimation("Die3", 0f);
		base.gameObject.SetActive(false);
		yield break;
	}

	public IEnumerator GetIntroSuckUpKittensAnimation()
	{
		yield return this.PlayAnimation("Intro_steal2", 0f);
		yield break;
	}

	private IEnumerator PlayAnimation(string animationName, float endEarlyTime = 0f)
	{
		TrackEntry trackEntry = this.skeletonAnimation.AnimationState.SetAnimation(0, animationName, false);
		float animDuration = trackEntry.EndTime - endEarlyTime;
		animDuration = Mathf.Clamp(animDuration, 0f, float.MaxValue);
		this.animFiber.Start(FiberHelper.Wait(animDuration, (FiberHelper.WaitFlag)0));
		while (!this.animFiber.IsTerminated)
		{
			yield return null;
		}
		this.skeletonAnimation.AnimationName = string.Empty;
		yield break;
	}

	public void SetDestructionLevel(int destructionLevel)
	{
		for (int i = 0; i < this.stageParticles.Length; i++)
		{
			bool active = destructionLevel > i;
			this.stageParticles[i].gameObject.SetActive(active);
		}
	}

	public void EnableBubbleParticles(bool enable)
	{
		foreach (ParticleSystem particleSystem in this.spawnBubblesParticles.GetComponentsInChildren<ParticleSystem>())
		{
			var _temp_val_290 = particleSystem.emission; _temp_val_290.enabled = enable;
		}
	}

	public void PlayOpenDoorParticles()
	{
		this.openDoorParticles.Play(true);
	}

	[SerializeField]
	private SkeletonAnimation skeletonAnimation;

	[SerializeField]
	private Transform suckPoint;

	[SerializeField]
	private Transform spawnBubblesPoint;

	[SerializeField]
	private Transform kittenSpawnPoint;

	[SerializeField]
	private ParticleSystem[] stageParticles;

	[SerializeField]
	private ParticleSystem spawnBubblesParticles;

	[SerializeField]
	private ParticleSystem openDoorParticles;

	private readonly Fiber animFiber = new Fiber();

	private Vector2 lastFramePos;

	private bool enableMovementAnimation;

	private static class AnimNames
	{
		public const string BUBBLES = "Bubbles";

		public const string DIE = "Die3";

		public const string FLY_LEFT = "Fly_Left";

		public const string FLY_RIGHT = "Fly_Right";

		public const string FLY_DOWN = "Fly_down";

		public const string FLY_UP = "Fly_up";

		public const string HIT = "Hit";

		public const string INTRO_STEAL = "Intro_steal2";

		public const string INTRO_FLY_UP = "Intro_Fly_up";
	}
}
