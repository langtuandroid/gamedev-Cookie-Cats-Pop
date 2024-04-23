using System;
using System.Collections;
using System.Collections.Generic;
using Spine;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	[RequireComponent(typeof(SkeletonAnimation))]
	public class MapSpine : IMapAnimatable, IMapPropBuildAnimatable
	{
		public string InAnimation
		{
			get
			{
				return this.inAnimation;
			}
			set
			{
				this.inAnimation = value;
			}
		}

		public string IdleAnimation
		{
			get
			{
				return this.idleAnimation;
			}
			set
			{
				this.idleAnimation = value;
			}
		}

		public IEnumerator PlayAnimation(string animationID)
		{
			if (this.skeletonAnimation.HasAnimation(animationID))
			{
				float time = this.skeletonAnimation.PlayAnimation(0, animationID, false, false).animation.duration;
				yield return FiberHelper.Wait(time, (FiberHelper.WaitFlag)0);
			}
			yield break;
		}

		public override IEnumerator PlayAnimation(string animationID, Vector2 direction, string transitionAnimation, int loops)
		{
			return this.PlayAnimation(animationID);
		}

		public override string[] GetAvailableAnimations()
		{
			string[] result;
			try
			{
				this.skeletonAnimation = base.GetComponent<SkeletonAnimation>();
				if (this.skeletonAnimation == null)
				{
					result = new string[0];
				}
				else if (this.skeletonAnimation.skeleton == null)
				{
					SkeletonData skeletonData = this.skeletonAnimation.skeletonDataAsset.GetSkeletonData(true);
					List<string> list = new List<string>();
					foreach (Spine.Animation animation in skeletonData.Animations)
					{
						list.Add(animation.name);
					}
					result = list.ToArray();
				}
				else
				{
					string[] array = new string[this.skeletonAnimation.skeleton.Data.Animations.Count];
					int num = 0;
					foreach (Spine.Animation animation2 in this.skeletonAnimation.skeleton.Data.Animations)
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

		public override bool SupportsTransitionAnimations(string animName)
		{
			return false;
		}

		void IMapPropBuildAnimatable.BuildInitialize(bool isTeardown)
		{
			if (!isTeardown)
			{
				this.meshRenderer.enabled = false;
			}
		}

		IEnumerator IMapPropBuildAnimatable.Build(bool isTeardown)
		{
			if (!isTeardown)
			{
				if (!base.gameObject.activeInHierarchy)
				{
					yield break;
				}
				this.meshRenderer.enabled = true;
				if (!string.IsNullOrEmpty(this.inAnimation) && this.skeletonAnimation.HasAnimation(this.inAnimation))
				{
					float delay = this.skeletonAnimation.PlayAnimation(0, this.inAnimation, false, true).animation.duration;
					if (!string.IsNullOrEmpty(this.idleAnimation) && this.skeletonAnimation.HasAnimation(this.idleAnimation))
					{
						this.skeletonAnimation.AddAnimationInQueue(0, this.idleAnimation, true, 0f, false);
					}
					yield return FiberHelper.Wait(delay, (FiberHelper.WaitFlag)0);
				}
			}
			yield break;
		}

		protected override void Initialized()
		{
			this.skeletonAnimation = base.GetComponent<SkeletonAnimation>();
			this.meshRenderer = base.GetComponent<MeshRenderer>();
		}

		[SerializeField]
		private string inAnimation = "in";

		[SerializeField]
		private string idleAnimation = "idle";

		private SkeletonAnimation skeletonAnimation;

		private MeshRenderer meshRenderer;
	}
}
