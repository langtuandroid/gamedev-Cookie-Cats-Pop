using System;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using TactileModules.Validation;
using UnityEngine;

namespace Tactile.GardenGame.Story.Dialog
{
	public class DialogCharacter : MonoBehaviour
	{
		public string LocalizedName
		{
			get
			{
				return L.Get(this.Name);
			}
		}

		public SkeletonAnimation GetSpinePose(string poseId, GameObject existingGameObject = null)
		{
			if (this.spineAnim == null)
			{
				return null;
			}
			SkeletonAnimation skeletonAnimation;
			if (existingGameObject == null)
			{
				skeletonAnimation = new GameObject(this.Name, new Type[]
				{
					typeof(MeshRenderer)
				})
				{
					transform = 
					{
						localScale = Vector3.one,
						localPosition = Vector3.zero,
						localRotation = Quaternion.identity
					}
				}.AddComponent<SkeletonAnimation>();
			}
			else
			{
				skeletonAnimation = existingGameObject.GetComponent<SkeletonAnimation>();
			}
			SkeletonDataAsset skeletonDataAsset = skeletonAnimation.skeletonDataAsset;
			skeletonAnimation.skeletonDataAsset = this.spineAnim;
			skeletonAnimation.Reset();
			skeletonAnimation.Update();
			skeletonAnimation.LateUpdate();
			string animationName;
			float time;
			if (this.posesAreLoopingAnimations)
			{
				skeletonAnimation.PlayAnimation(0, poseId, true, false);
			}
			else if (skeletonAnimation.skeleton.data.Events.Count == 0)
			{
				skeletonAnimation.skeleton.PoseWithAnimation(poseId, 0f, false);
			}
			else if (this.FindAnimationAndTimeFromEventName(poseId, out animationName, out time))
			{
				skeletonAnimation.skeleton.PoseWithAnimation(animationName, time, false);
			}
			skeletonAnimation.Update();
			skeletonAnimation.LateUpdate();
			return skeletonAnimation;
		}

		public IEnumerable<string> IteratePoses()
		{
			if (this.spineAnim == null)
			{
				yield return "Unknown";
				yield break;
			}
			if (this.skeletonData == null)
			{
				this.skeletonData = this.spineAnim.GetSkeletonData(false);
			}
			if (this.skeletonData.Events.Count > 0)
			{
				for (int i = 0; i < this.skeletonData.Events.Count; i++)
				{
					yield return this.skeletonData.Events[i].Name;
				}
			}
			else
			{
				for (int j = 0; j < this.skeletonData.Animations.Count; j++)
				{
					yield return this.skeletonData.Animations[j].Name;
				}
			}
			yield break;
		}

		private bool FindAnimationAndTimeFromEventName(string poseId, out string animation, out float time)
		{
			if (this.skeletonData == null)
			{
				this.skeletonData = this.spineAnim.GetSkeletonData(false);
			}
			animation = poseId;
			time = 0f;
			foreach (Spine.Animation animation2 in this.skeletonData.Animations)
			{
				foreach (Timeline timeline in animation2.timelines)
				{
					if (timeline is EventTimeline)
					{
						EventTimeline eventTimeline = timeline as EventTimeline;
						for (int i = 0; i < eventTimeline.Events.Length; i++)
						{
							if (eventTimeline.Events[i].Data.Name == poseId)
							{
								animation = animation2.Name;
								time = eventTimeline.Frames[i];
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		private void ReloadSpine(SkeletonAnimation spine)
		{
			if (spine.skeletonDataAsset != null)
			{
				foreach (AtlasAsset atlasAsset in spine.skeletonDataAsset.atlasAssets)
				{
					if (atlasAsset != null)
					{
						atlasAsset.Reset();
					}
				}
				spine.skeletonDataAsset.Reset();
			}
			spine.Initialize(true);
		}

		public string Name;

		[OptionalSerializedField]
		public SkeletonDataAsset spineAnim;

		[SerializeField]
		public bool posesAreLoopingAnimations;

		[SerializeField]
		public bool enableXFade = true;

		private SkeletonData skeletonData;
	}
}
