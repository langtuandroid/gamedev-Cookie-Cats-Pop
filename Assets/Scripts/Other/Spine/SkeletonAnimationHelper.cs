using System;
using System.Collections.Generic;

namespace Spine
{
	public static class SkeletonAnimationHelper
	{
		public static void AddDefaultKeysToSkeletonData(SkeletonData skeletonData)
		{
			for (int i = 0; i < skeletonData.Animations.Count; i++)
			{
				SkeletonAnimationHelper.AddDefaultKeysToTimeLines(skeletonData.Animations[i].Timelines, skeletonData);
			}
		}

		private static void AddDefaultKeysToTimeLines(ExposedList<Timeline> animationTimelines, SkeletonData skeletonData)
		{
			bool flag = false;
			for (int i = 0; i < animationTimelines.Count; i++)
			{
				Timeline timeline = animationTimelines[i];
				if (timeline is ColorTimeline)
				{
					SkeletonAnimationHelper.colortimeLines.Add((timeline as ColorTimeline).slotIndex);
				}
				else if (timeline is AttachmentTimeline)
				{
					SkeletonAnimationHelper.attachmentTimelines.Add((timeline as AttachmentTimeline).slotIndex);
				}
				else if (timeline is ScaleTimeline)
				{
					SkeletonAnimationHelper.scaleTimelines.Add((timeline as ScaleTimeline).boneIndex);
				}
				else if (timeline is TranslateTimeline)
				{
					SkeletonAnimationHelper.translateTimelines.Add((timeline as TranslateTimeline).boneIndex);
				}
				else if (timeline is RotateTimeline)
				{
					SkeletonAnimationHelper.rotateTimelines.Add((timeline as RotateTimeline).boneIndex);
				}
				else if (timeline is DrawOrderTimeline)
				{
					flag = true;
				}
			}
			for (int j = 0; j < skeletonData.slots.Count; j++)
			{
				if (!SkeletonAnimationHelper.colortimeLines.Contains(j))
				{
					SlotData slotData = skeletonData.slots[j];
					ColorTimeline colorTimeline = new ColorTimeline(1)
					{
						slotIndex = j
					};
					colorTimeline.SetFrame(0, 0f, slotData.R, slotData.G, slotData.B, slotData.A);
					animationTimelines.Add(colorTimeline);
				}
			}
			SkeletonAnimationHelper.colortimeLines.Clear();
			for (int k = 0; k < skeletonData.slots.Count; k++)
			{
				if (!SkeletonAnimationHelper.attachmentTimelines.Contains(k))
				{
					AttachmentTimeline attachmentTimeline = new AttachmentTimeline(1);
					attachmentTimeline.slotIndex = k;
					attachmentTimeline.SetFrame(0, 0f, skeletonData.slots[k].attachmentName);
					animationTimelines.Add(attachmentTimeline);
				}
			}
			SkeletonAnimationHelper.attachmentTimelines.Clear();
			for (int l = 0; l < skeletonData.bones.Count; l++)
			{
				if (!SkeletonAnimationHelper.scaleTimelines.Contains(l))
				{
					ScaleTimeline scaleTimeline = new ScaleTimeline(1);
					scaleTimeline.boneIndex = l;
					scaleTimeline.SetFrame(0, 0f, 1f, 1f);
					animationTimelines.Add(scaleTimeline);
				}
			}
			SkeletonAnimationHelper.scaleTimelines.Clear();
			for (int m = 0; m < skeletonData.bones.Count; m++)
			{
				if (!SkeletonAnimationHelper.translateTimelines.Contains(m))
				{
					TranslateTimeline translateTimeline = new TranslateTimeline(1);
					translateTimeline.boneIndex = m;
					translateTimeline.SetFrame(0, 0f, 0f, 0f);
					animationTimelines.Add(translateTimeline);
				}
			}
			SkeletonAnimationHelper.translateTimelines.Clear();
			for (int n = 0; n < skeletonData.bones.Count; n++)
			{
				if (!SkeletonAnimationHelper.rotateTimelines.Contains(n))
				{
					RotateTimeline rotateTimeline = new RotateTimeline(1);
					rotateTimeline.boneIndex = n;
					rotateTimeline.SetFrame(0, 0f, 0f);
					animationTimelines.Add(rotateTimeline);
				}
			}
			SkeletonAnimationHelper.rotateTimelines.Clear();
			if (!flag)
			{
				DrawOrderTimeline drawOrderTimeline = new DrawOrderTimeline(1);
				drawOrderTimeline.SetFrame(0, 0f, null);
				animationTimelines.Add(drawOrderTimeline);
			}
		}

		private static readonly HashSet<int> colortimeLines = new HashSet<int>();

		private static readonly HashSet<int> attachmentTimelines = new HashSet<int>();

		private static readonly HashSet<int> scaleTimelines = new HashSet<int>();

		private static readonly HashSet<int> translateTimelines = new HashSet<int>();

		private static readonly HashSet<int> rotateTimelines = new HashSet<int>();
	}
}
