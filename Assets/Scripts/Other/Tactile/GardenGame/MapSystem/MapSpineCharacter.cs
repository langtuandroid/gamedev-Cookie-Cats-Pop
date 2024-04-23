using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using Spine;
using Tactile.GardenGame.Helpers;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	public class MapSpineCharacter : IMapAnimatable
	{
		public SkeletonAnimation SpinePrefab
		{
			get
			{
				return this.spinePrefab;
			}
			set
			{
				this.spinePrefab = value;
				this.spineDataDirty = true;
			}
		}

		public List<MapSpineCharacter.IdleAnimation> IdleAnims
		{
			get
			{
				return this.idleAnims;
			}
			set
			{
				this.idleAnims = value;
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

		public string RunAnim
		{
			get
			{
				return this.runAnim;
			}
			set
			{
				this.runAnim = value;
			}
		}

		public bool ScaleAnimationSpeedWithMovement
		{
			get
			{
				return this.scaleAnimationSpeedWithMovement;
			}
			set
			{
				this.scaleAnimationSpeedWithMovement = value;
			}
		}

		public override bool SupportsTransitionAnimations(string animName)
		{
			return false;
		}

		public override string[] GetAvailableAnimations()
		{
			if (this.availableAnimationList != null && this.availableAnimationList.Length > 0)
			{
				return this.availableAnimationList;
			}
			List<string> list = new List<string>();
			string[] allAnimationNames = this.GetAllAnimationNames();
			if (allAnimationNames == null)
			{
				return null;
			}
			foreach (string animationID in allAnimationNames)
			{
				string item = this.StripAnimationPostfixes(animationID);
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}
			string[] array2 = list.ToArray();
			if (array2.Length > 0)
			{
				this.availableAnimationList = array2;
			}
			return array2;
		}

		public MapSpineCharacter.IdleAnimation GetAppropriateIdleAnimation(CardinalDirection direction, string avoidAnimation)
		{
			bool flag = base.OwnerMap != null && !base.OwnerMap.InteractionEnabled;
			MapSpineCharacter.IdleAnimation idleAnimation = null;
			List<MapSpineCharacter.IdleAnimation> list = new List<MapSpineCharacter.IdleAnimation>();
			for (int i = 0; i < this.idleAnims.Count; i++)
			{
				if (this.SupportsDirection(this.idleAnims[i].animationName, direction))
				{
					if (this.idleAnims[i].animationName == avoidAnimation)
					{
						idleAnimation = this.idleAnims[i];
					}
					else if (!this.idleAnims[i].notDuringStory || !flag)
					{
						for (int j = 0; j < this.idleAnims[i].probabilityWeighting; j++)
						{
							list.Add(this.idleAnims[i]);
						}
					}
				}
			}
			if (list.Count <= 0 && idleAnimation != null)
			{
				return idleAnimation;
			}
			if (list.Count == 1)
			{
				return list[0];
			}
			if (list.Count > 0)
			{
				int index = UnityEngine.Random.Range(0, list.Count);
				return list[index];
			}
			return null;
		}

		public override IEnumerator PlayAnimation(string animationID, Vector2 direction, string transitionAnim, int loops)
		{
			if (direction.magnitude > 0f)
			{
				this.currentFacingDirection = IsometricUtils.GetNearestIsoCardinalDirection(direction);
			}
			if (this.moveIdleAnimFiber != null && !this.moveIdleAnimFiber.IsTerminated)
			{
				this.moveIdleAnimFiber.Terminate();
			}
			this.isPlayingMoveAnimation = false;
			yield return this.PlayAnimation(animationID, this.currentFacingDirection, false, loops);
			yield break;
		}

		private CardinalDirection GetCurrentMapMovableDirection()
		{
			return IsometricUtils.GetNearestIsoCardinalDirection(this.mapMoveable.Direction);
		}

		public List<CardinalDirection> GetSupportedDirections(string action)
		{
			List<CardinalDirection> list = new List<CardinalDirection>();
			if (this.HasAnimation(action))
			{
				for (CardinalDirection cardinalDirection = CardinalDirection.N; cardinalDirection < CardinalDirection.NONE; cardinalDirection++)
				{
					list.Add(cardinalDirection);
				}
			}
			else
			{
				for (CardinalDirection cardinalDirection2 = CardinalDirection.N; cardinalDirection2 < CardinalDirection.NONE; cardinalDirection2++)
				{
					if (this.SupportsDirection(action, cardinalDirection2))
					{
						list.Add(cardinalDirection2);
					}
				}
			}
			return list;
		}

		public bool SupportsDirection(string action, CardinalDirection direction)
		{
			if (direction == CardinalDirection.NONE)
			{
				return this.HasAnimation(action);
			}
			bool flag;
			return this.GetAnimationID(action, direction, out flag) != string.Empty;
		}

		private string FormatAnimationName(string action, CardinalDirection direction)
		{
			if (direction == CardinalDirection.NONE)
			{
				return action;
			}
			return action + "_" + direction.ToString();
		}

		private string StripAnimationPostfixes(string animationID)
		{
			for (CardinalDirection cardinalDirection = CardinalDirection.N; cardinalDirection < CardinalDirection.NONE; cardinalDirection++)
			{
				if (animationID.EndsWith("_" + cardinalDirection.ToString()))
				{
					int length = animationID.LastIndexOf("_" + cardinalDirection.ToString());
					animationID = animationID.Substring(0, length);
					break;
				}
			}
			animationID = animationID.Replace("_In", string.Empty);
			return animationID;
		}

		private string[] GetAllAnimationNames()
		{
			string[] array = new string[this.supportedSpineAnimations.Count];
			int num = 0;
			foreach (KeyValuePair<string, bool> keyValuePair in this.supportedSpineAnimations)
			{
				array[num] = keyValuePair.Key;
				num++;
			}
			return array;
		}

		protected bool HasAnimation(string name)
		{
			if (this.supportedSpineAnimations.Count <= 0 && this.spineInstance != null)
			{
				this.BuildAnimationLookupTable();
			}
			return this.supportedSpineAnimations.ContainsKey(name);
		}

		private float PlaySpineAnimation(string animationID, bool loop, bool preserveFrameFromPrevious, bool flipx)
		{
			if (!this.supportedSpineAnimations.ContainsKey(animationID))
			{
				return 0f;
			}
			Vector3 localScale = base.transform.localScale;
			bool flag = localScale.x < 0f;
			if ((flipx && localScale.x > 0f) || (!flipx && flag))
			{
				localScale.x = -localScale.x;
			}
			base.transform.localScale = localScale;
			float num = 0f;
			if (preserveFrameFromPrevious && this.spineInstance.gameObject.activeInHierarchy && this.spineInstance.state.Tracks[0] != null)
			{
				num = this.spineInstance.state.Tracks[0].time;
			}
			this.spineInstance.gameObject.SetActive(true);
			if (this.spineInstance.state != null)
			{
				TrackEntry current = this.spineInstance.state.GetCurrent(0);
				if (current != null && current.ToString() == animationID)
				{
					return current.animation.duration;
				}
			}
			float num2 = this.spineInstance.PlayAnimation(0, animationID, loop, false).animation.Duration;
			if (this.ScaleAnimationSpeedWithMovement && this.mapMoveable == null && this.mapMoveable.MovementSpeedMultiplier > 0f)
			{
				TrackEntry current2 = this.spineInstance.state.GetCurrent(0);
				current2.TimeScale += this.mapMoveable.MovementSpeedMultiplier;
				num2 /= this.mapMoveable.MovementSpeedMultiplier;
			}
			if (num > num2)
			{
				while (num > num2)
				{
					num -= num2;
				}
			}
			if (preserveFrameFromPrevious)
			{
				if (this.offsetFramePreservationOnFlip && flipx != flag)
				{
					num += num2 * 0.5f;
				}
				if (num > num2)
				{
					num -= num2;
				}
				num2 -= num;
			}
			this.spineInstance.state.Tracks[0].time = num;
			this.spineInstance.Update(0f);
			return num2;
		}

		private IEnumerator PlayAnimation(string action, CardinalDirection direction, bool preserveFrameFromPrevious = false, int loops = 1)
		{
			if (!this.playAnimationFiber.IsTerminated)
			{
				this.playAnimationFiber.Terminate();
			}
			this.playAnimationFiber.Start(this.PlayAnimationInternal(action, direction, preserveFrameFromPrevious, loops));
			yield return null;
			while (!this.playAnimationFiber.IsTerminated && this.isPlayingAnimation == action)
			{
				yield return null;
			}
			yield break;
		}

		private IEnumerator PlayAnimationInternal(string action, CardinalDirection direction, bool preserveFrameFromPrevious, int loops)
		{
			this.isPlayingAnimation = action;
			bool outShouldFlip = false;
			string outAnim = this.GetAnimationID(action + "_Out", direction, out outShouldFlip);
			bool shouldFlipX = false;
			string mainAnimation = this.GetAnimationID(action, direction, out shouldFlipX);
			bool inAnimationShouldFlipX = false;
			string inAnimation = this.GetAnimationID(action + "_In", direction, out inAnimationShouldFlipX);
			if (!string.IsNullOrEmpty(this.currentRequiredOutAnimation))
			{
				this.PlaySpineAnimation(this.currentRequiredOutAnimation, false, false, this.currentRequiredOutAnimationShouldFlipX);
				yield return this.WaitForSpineToFinish(this.spineInstance);
			}
			this.currentRequiredOutAnimation = string.Empty;
			if (!string.IsNullOrEmpty(inAnimation))
			{
				this.PlaySpineAnimation(inAnimation, false, false, inAnimationShouldFlipX);
				yield return this.WaitForSpineToFinish(this.spineInstance);
			}
			if (!string.IsNullOrEmpty(outAnim))
			{
				this.currentRequiredOutAnimation = outAnim;
				this.currentRequiredOutAnimationShouldFlipX = outShouldFlip;
			}
			if (!string.IsNullOrEmpty(mainAnimation))
			{
				float time = 0f;
				if (loops <= 0)
				{
					time = this.PlaySpineAnimation(mainAnimation, true, preserveFrameFromPrevious, shouldFlipX);
					yield return FiberHelper.Wait(time, (FiberHelper.WaitFlag)0);
				}
				else
				{
					while (loops > 0)
					{
						this.PlaySpineAnimation(mainAnimation, false, preserveFrameFromPrevious, shouldFlipX);
						yield return this.WaitForSpineToFinish(this.spineInstance);
						loops--;
					}
					if (!string.IsNullOrEmpty(outAnim))
					{
						this.currentRequiredOutAnimation = string.Empty;
						time = this.PlaySpineAnimation(outAnim, false, false, outShouldFlip);
						yield return this.WaitForSpineToFinish(this.spineInstance);
					}
				}
			}
			this.isPlayingAnimation = string.Empty;
			yield break;
		}

		public bool HasRunAnimation()
		{
			return this.HasAnimation(this.FormatAnimationName(this.RunAnim, CardinalDirection.N));
		}

		public void PlayMoveAnimation(Vector2 direction)
		{
			string action = this.WalkAnim;
			if (this.mapMoveable.DesiredMoveBehaviour == MapMoveable.MoveBehaviour.Run && this.HasRunAnimation())
			{
				action = this.RunAnim;
			}
			if (this.moveIdleAnimFiber == null)
			{
				this.moveIdleAnimFiber = new Fiber(FiberBucket.Update);
			}
			CardinalDirection nearestIsoCardinalDirection = IsometricUtils.GetNearestIsoCardinalDirection(direction);
			if (!this.isPlayingMoveAnimation || nearestIsoCardinalDirection != this.currentFacingDirection)
			{
				this.moveIdleAnimFiber.Start(this.PlayAnimation(action, nearestIsoCardinalDirection, this.isPlayingMoveAnimation && this.preserveWalkFrameOnDirectionChange, -1));
				this.isPlayingMoveAnimation = true;
				this.currentFacingDirection = nearestIsoCardinalDirection;
			}
		}

		private IEnumerator WaitForSpineToFinish(SkeletonAnimation skeletonAnimation)
		{
			while (skeletonAnimation != null && skeletonAnimation.state != null && skeletonAnimation.state.GetCurrent(0) != null)
			{
				yield return null;
			}
			yield break;
		}

		private string GetAnimationID(string action, CardinalDirection direction, out bool shouldFlipX)
		{
			string text = this.FormatAnimationName(action, direction);
			shouldFlipX = false;
			if (!this.HasAnimation(text.ToString()))
			{
				switch (direction)
				{
				case CardinalDirection.NE:
					direction = CardinalDirection.NW;
					break;
				case CardinalDirection.E:
					direction = CardinalDirection.W;
					break;
				case CardinalDirection.SE:
					direction = CardinalDirection.SW;
					break;
				case CardinalDirection.SW:
					direction = CardinalDirection.SE;
					break;
				case CardinalDirection.W:
					direction = CardinalDirection.E;
					break;
				case CardinalDirection.NW:
					direction = CardinalDirection.NE;
					break;
				}
				text = this.FormatAnimationName(action, direction);
				if (this.HasAnimation(text))
				{
					shouldFlipX = true;
				}
				else
				{
					text = this.FormatAnimationName(action, CardinalDirection.NONE);
				}
			}
			if (this.HasAnimation(text))
			{
				return text;
			}
			return string.Empty;
		}

		protected override void Initialized()
		{
			this.InitSpineData();
			this.mapMoveable = base.GetComponent<MapMoveable>();
			base.Initialized();
			this.initialised = true;
		}

		private void InitSpineData()
		{
			if (this.SpinePrefab == null)
			{
				return;
			}
			if (!this.spineDataDirty)
			{
				return;
			}
			this.spineDataDirty = false;
			if (this.spineInstance != null)
			{
				if (Application.isPlaying)
				{
					UnityEngine.Object.Destroy(this.spineInstance.gameObject);
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(this.spineInstance.gameObject);
				}
			}
			this.supportedSpineAnimations.Clear();
			this.spineInstance = UnityEngine.Object.Instantiate<SkeletonAnimation>(this.SpinePrefab);
			this.spineInstance.transform.parent = base.transform;
			this.spineInstance.transform.localPosition = Vector3.zero;
			this.spineInstance.transform.localScale = Vector3.one;
			this.spineInstance.gameObject.SetLayerRecursively(base.gameObject.layer);
			string[] animationNames = this.GetAnimationNames(this.spineInstance);
			foreach (string key in animationNames)
			{
				this.supportedSpineAnimations[key] = true;
			}
			this.spineInstance.gameObject.SetActive(false);
			if (!Application.isPlaying)
			{
				this.spineInstance.gameObject.SetHideFlagsRecursively(HideFlags.HideAndDontSave);
			}
		}

		private void BuildAnimationLookupTable()
		{
			this.supportedSpineAnimations.Clear();
			string[] animationNames = this.GetAnimationNames(this.spineInstance);
			foreach (string key in animationNames)
			{
				this.supportedSpineAnimations[key] = true;
			}
		}

		private void LateUpdate()
		{
			if (this.mapMoveable == null)
			{
				return;
			}
			if (!this.initialised)
			{
				return;
			}
			if (this.framesSinceMove > 2 && !this.isMoving)
			{
				if (this.isPlayingMoveAnimation)
				{
					this.isPlayingMoveAnimation = false;
					this.PlayIdleAnimation();
				}
				else if (this.spineInstance != null)
				{
					bool flag = false;
					if (this.moveIdleAnimFiber != null && !this.moveIdleAnimFiber.IsTerminated)
					{
						flag = true;
					}
					else if (!string.IsNullOrEmpty(this.isPlayingAnimation))
					{
						flag = true;
					}
					else if (this.spineInstance.gameObject.activeSelf && this.spineInstance.state != null && this.spineInstance.state.GetCurrent(0) != null)
					{
						flag = true;
					}
					if (!flag)
					{
						this.PlayIdleAnimation();
					}
				}
			}
			this.isMoving = this.mapMoveable.IsMoving;
			if (this.isMoving)
			{
				this.PlayMoveAnimation(this.mapMoveable.Direction);
				this.framesSinceMove = 0;
			}
			else
			{
				this.framesSinceMove++;
			}
		}

		public void PlayIdleAnimation(Vector2 direction)
		{
			this.currentFacingDirection = IsometricUtils.GetNearestIsoCardinalDirection(direction);
			this.PlayIdleAnimation();
		}

		public void PlayIdleAnimation()
		{
			if (this.moveIdleAnimFiber == null)
			{
				this.moveIdleAnimFiber = new Fiber(FiberBucket.Update);
			}
			MapSpineCharacter.IdleAnimation appropriateIdleAnimation = this.GetAppropriateIdleAnimation(this.currentFacingDirection, this.lastIdleAnimationID);
			if (appropriateIdleAnimation != null)
			{
				this.lastIdleAnimationID = appropriateIdleAnimation.animationName;
				this.moveIdleAnimFiber.Start(this.PlayAnimation(appropriateIdleAnimation.animationName, this.currentFacingDirection, false, (!appropriateIdleAnimation.loop) ? 1 : -1));
			}
		}

		private string[] GetAnimationNames(SkeletonAnimation spine)
		{
			string[] result;
			try
			{
				if (spine.skeleton == null)
				{
					SkeletonData skeletonData = spine.skeletonDataAsset.GetSkeletonData(true);
					List<string> list = new List<string>();
					foreach (Spine.Animation animation in skeletonData.Animations)
					{
						list.Add(animation.name);
					}
					result = list.ToArray();
				}
				else
				{
					string[] array = new string[spine.skeleton.Data.Animations.Count];
					int num = 0;
					foreach (Spine.Animation animation2 in spine.skeleton.Data.Animations)
					{
						array[num] = animation2.Name;
						num++;
					}
					result = array;
				}
			}
			catch
			{
				result = new string[0];
			}
			return result;
		}

		private const string inAnimationPostFix = "_In";

		private const string outAnimationPostFix = "_Out";

		[SerializeField]
		private SkeletonAnimation spinePrefab;

		[SerializeField]
		private List<MapSpineCharacter.IdleAnimation> idleAnims = new List<MapSpineCharacter.IdleAnimation>();

		[SerializeField]
		private string walkAnim = "Walk";

		[SerializeField]
		private string runAnim = "Run";

		[SerializeField]
		private bool preserveWalkFrameOnDirectionChange = true;

		[SerializeField]
		private bool offsetFramePreservationOnFlip = true;

		[SerializeField]
		private bool scaleAnimationSpeedWithMovement;

		private MapMoveable mapMoveable;

		private SkeletonAnimation spineInstance;

		private Dictionary<string, bool> supportedSpineAnimations = new Dictionary<string, bool>();

		private bool spineDataDirty = true;

		private bool initialised;

		private Fiber playAnimationFiber = new Fiber(FiberBucket.Update);

		private CardinalDirection currentFacingDirection = CardinalDirection.SE;

		private bool isPlayingMoveAnimation;

		private Fiber moveIdleAnimFiber;

		private string currentRequiredOutAnimation = string.Empty;

		private bool currentRequiredOutAnimationShouldFlipX;

		private string lastIdleAnimationID = string.Empty;

		private bool isMoving;

		private int framesSinceMove;

		private string isPlayingAnimation = string.Empty;

		private string[] availableAnimationList;

		[Serializable]
		public class IdleAnimation
		{
			public string animationName = "Idle";

			public int probabilityWeighting = 1;

			public bool loop = true;

			public bool notDuringStory;
		}
	}
}
