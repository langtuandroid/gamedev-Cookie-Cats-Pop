using System;
using Spine;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	[RequireComponent(typeof(SkeletonAnimation))]
	public class MapSpineRootMotion : MapComponent
	{
		public string RootBoneId
		{
			get
			{
				return this.rootBoneID;
			}
			set
			{
				this.rootBoneID = value;
			}
		}

		protected override void Initialized()
		{
		}

		public float GetDistanceTravelledThisFrame()
		{
			return this.frameDistance;
		}

		private void LateUpdate()
		{
			this.ApplyRootMotion();
		}

		private void ApplyRootMotion()
		{
			if (this.skeletonAnimation == null)
			{
				this.skeletonAnimation = base.GetComponent<SkeletonAnimation>();
			}
			if (this.bone == null)
			{
				if (this.skeletonAnimation.skeleton == null)
				{
					return;
				}
				this.bone = this.skeletonAnimation.skeleton.FindBone(this.rootBoneID);
			}
			if (this.bone != null)
			{
				Vector3 position = new Vector3(this.bone.worldX, this.bone.worldY);
				Vector3 a = base.transform.TransformPoint(position);
				Vector3 b = (!(base.transform != null)) ? Vector3.zero : base.transform.parent.position;
				Vector3 b2 = a - b;
				base.transform.position = base.transform.position - b2;
				if (this.skeletonAnimation.AnimationState == null || this.skeletonAnimation.AnimationState.Tracks[0] == null)
				{
					return;
				}
				float time = this.skeletonAnimation.AnimationState.Tracks[0].time;
				string name = this.skeletonAnimation.AnimationState.Tracks[0].animation.name;
				int num = Mathf.FloorToInt(time / this.skeletonAnimation.AnimationState.Tracks[0].animation.duration);
				if (time < this.lastUpdateTime || num != this.loops)
				{
					b2 = default(Vector3);
				}
				else if (b2.magnitude < 50f)
				{
					this.frameDistance = b2.magnitude;
				}
				if (name != this.currentAnimationID)
				{
					this.currentAnimationID = name;
					b2 = default(Vector3);
					this.frameDistance = 0f;
				}
				this.loops = num;
				this.lastUpdateTime = time;
			}
		}

		[SerializeField]
		private string rootBoneID = string.Empty;

		private SkeletonAnimation skeletonAnimation;

		private string currentAnimationID = string.Empty;

		private Bone bone;

		private float frameDistance;

		private float lastUpdateTime;

		private int loops = -1;

		private const float maxMovementPerFrame = 50f;
	}
}
