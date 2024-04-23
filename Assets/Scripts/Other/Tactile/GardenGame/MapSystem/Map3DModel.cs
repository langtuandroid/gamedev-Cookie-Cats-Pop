using System;
using System.Collections;
using Fibers;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	public class Map3DModel : IMapAnimatable
	{
		public GameObject Prefab
		{
			get
			{
				return this.prefab;
			}
			set
			{
				this.prefab = value;
			}
		}

		public string WalkAnim
		{
			get
			{
				return this.walkAnim;
			}
			set
			{
				this.walkAnim = value;
			}
		}

		public string IdleAnim
		{
			get
			{
				return this.idleAnim;
			}
			set
			{
				this.idleAnim = value;
			}
		}

		public float WalkAnimationSpeed
		{
			get
			{
				return this.walkAnimationSpeed;
			}
			set
			{
				this.walkAnimationSpeed = value;
			}
		}

		protected override void Initialized()
		{
			this.mapMoveable = base.GetComponent<MapMoveable>();
			this.CreateInstance();
			this.RegisterMoveableAnimations();
			this.PlayAnimation(this.idleAnim, null);
		}

		private void CreateInstance()
		{
			if (this.instance != null)
			{
				if (Application.isPlaying)
				{
					UnityEngine.Object.Destroy(this.instance);
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(this.instance);
				}
				this.instance = null;
			}
			if (this.prefab != null)
			{
				this.instance = UnityEngine.Object.Instantiate<GameObject>(this.prefab);
				this.instance.transform.parent = base.transform;
				this.instance.transform.localPosition = Vector3.zero;
				this.instance.transform.localRotation = Quaternion.identity;
				this.instance.transform.localScale = Vector3.one;
				this.instance.SetLayerRecursively(base.gameObject.layer);
				this.animator = this.instance.GetComponentInChildren<Animation>();
				if (!Application.isPlaying)
				{
					this.instance.SetHideFlagsRecursively(HideFlags.HideAndDontSave);
				}
			}
		}

		private void OnEnable()
		{
			if (Application.isPlaying)
			{
				this.InitializeAnimations();
			}
			else
			{
				this.Initialized();
			}
		}

		private void InitializeAnimations()
		{
			if (this.animator == null)
			{
				return;
			}
			IEnumerator enumerator = this.animator.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					AnimationState animationState = (AnimationState)obj;
					animationState.weight = 0f;
					animationState.enabled = false;
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
			if (!string.IsNullOrEmpty(this.previousAnimation))
			{
				this.animator.Blend(this.previousAnimation, 1f, 0f);
			}
		}

		private void LateUpdate()
		{
			if (this.mapMoveable == null || this.instance == null)
			{
				return;
			}
			this.UpdateDirection(this.mapMoveable.Direction, Time.deltaTime * 10f);
		}

		public void UpdateDirection(Vector2 direction, float deltaTime = 0f)
		{
			if (this.instance == null)
			{
				return;
			}
			Quaternion quaternion = Quaternion.Euler(-30f, 0f, 0f) * Quaternion.Euler(0f, 180f + Mathf.Atan2(direction.x, direction.y) * 57.29578f, 0f);
			this.instance.transform.localRotation = ((deltaTime > 0.001f) ? Quaternion.Slerp(this.instance.transform.localRotation, quaternion, deltaTime) : quaternion);
		}

		private void RegisterMoveableAnimations()
		{
			if (this.mapMoveable == null)
			{
				return;
			}
			if (this.animator == null)
			{
				return;
			}
			this.mapMoveable.MovementStarted += this.MapMoveableOnMovementStarted;
			this.mapMoveable.MovementStopped += this.MapMoveableOnMovementStopped;
			this.mapMoveable.Moved += this.MapMoveableOnMoved;
		}

		private void MapMoveableOnMoved(MapMoveable mapMoveable, float totalLength, float currentPosition)
		{
			if (!this.isPlayingWalk)
			{
				return;
			}
			if (currentPosition > totalLength - mapMoveable.MovementSpeed * 0.2f)
			{
				this.MapMoveableOnMovementStopped(mapMoveable);
			}
		}

		private void MapMoveableOnMovementStopped(MapMoveable mapMoveable)
		{
			if (this.isPlayingWalk)
			{
				this.PlayAnimation(this.idleAnim, null);
				this.isPlayingWalk = false;
			}
		}

		private void MapMoveableOnMovementStarted(MapMoveable mapMoveable)
		{
			this.PlayAnimation(this.walkAnim, null);
			this.isPlayingWalk = true;
		}

		public override bool SupportsTransitionAnimations(string animName)
		{
			Animation animation = (!(this.prefab == null)) ? this.prefab.GetComponentInChildren<Animation>() : null;
			if (animation == null)
			{
				return false;
			}
			IEnumerator enumerator = animation.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					AnimationState animationState = (AnimationState)obj;
					if (animName == animationState.name)
					{
						return animationState.clip.wrapMode != WrapMode.Loop;
					}
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
			return false;
		}

		public override string[] GetAvailableAnimations()
		{
			Animation animation = (!(this.prefab == null)) ? this.prefab.GetComponentInChildren<Animation>() : null;
			if (animation == null)
			{
				return new string[]
				{
					"Animation component doens't exists"
				};
			}
			int clipCount = animation.GetClipCount();
			string[] array = new string[clipCount];
			try
			{
				int num = 0;
				IEnumerator enumerator = animation.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						AnimationState animationState = (AnimationState)obj;
						array[num] = animationState.name;
						num++;
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
			}
			catch (Exception ex)
			{
			}
			return array;
		}

		public override IEnumerator PlayAnimation(string anim, Vector2 direction, string transitionAnim = null, int loops = 1)
		{
			this.PlayAnimation(anim, transitionAnim);
			yield break;
		}

		private void PlayAnimation(string anim, string transitionAnim = null)
		{
			if (anim == this.previousAnimation)
			{
				return;
			}
			if (!string.IsNullOrEmpty(anim))
			{
				if (!this.IsAnimationValid(anim))
				{
					return;
				}
				this.animator.Blend(anim, 1f, 0.2f);
				if (anim == this.walkAnim)
				{
					this.animator[anim].speed = this.walkAnimationSpeed;
				}
			}
			if (!string.IsNullOrEmpty(this.previousAnimation))
			{
				if (!this.IsAnimationValid(this.previousAnimation))
				{
					return;
				}
				this.animator.Blend(this.previousAnimation, 0f, 0.2f);
			}
			this.previousAnimation = anim;
			if (!string.IsNullOrEmpty(transitionAnim))
			{
				if (!this.IsAnimationValid(transitionAnim))
				{
					return;
				}
				if (this.transitionAnimationFiber == null)
				{
					this.transitionAnimationFiber = new Fiber(FiberBucket.Manual);
				}
				this.transitionAnimationFiber.Start(this.WaitAndTransition(anim, transitionAnim));
			}
			else if (this.transitionAnimationFiber != null)
			{
				this.transitionAnimationFiber.Terminate();
			}
		}

		private void Update()
		{
			if (this.transitionAnimationFiber != null)
			{
				this.transitionAnimationFiber.Step();
			}
		}

		private IEnumerator WaitAndTransition(string currentAnim, string transitionAnim)
		{
			if (this.IsAnimationValid(currentAnim))
			{
				AnimationState state = this.animator[currentAnim];
				if (state.wrapMode == WrapMode.Loop)
				{
					yield break;
				}
				float waitTime = state.length - 0.2f;
				yield return FiberHelper.Wait(waitTime, (FiberHelper.WaitFlag)0);
			}
			this.PlayAnimation(transitionAnim, null);
			yield break;
		}

		private bool IsAnimationValid(string animationName)
		{
			return !string.IsNullOrEmpty(animationName) && this.animator != null && this.animator.GetClip(animationName) != null;
		}

		[SerializeField]
		private GameObject prefab;

		[SerializeField]
		private string walkAnim;

		[SerializeField]
		private string idleAnim;

		[SerializeField]
		private float walkAnimationSpeed = 1f;

		private const float blendDuration = 0.2f;

		private GameObject instance;

		private MapMoveable mapMoveable;

		private Animation animator;

		private string previousAnimation;

		private bool isPlayingWalk;

		private MaterialPropertyBlock materialPropertyBlock;

		private Fiber transitionAnimationFiber;
	}
}
