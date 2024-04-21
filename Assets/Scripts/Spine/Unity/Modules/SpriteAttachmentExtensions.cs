using System;
using UnityEngine;

namespace Spine.Unity.Modules
{
	public static class SpriteAttachmentExtensions
	{
		public static Attachment AttachUnitySprite(this Skeleton skeleton, string slotName, Sprite sprite, string shaderName = "Spine/Skeleton")
		{
			RegionAttachment regionAttachment = sprite.ToRegionAttachment(shaderName);
			skeleton.FindSlot(slotName).Attachment = regionAttachment;
			return regionAttachment;
		}

		public static Attachment AddUnitySprite(this SkeletonData skeletonData, string slotName, Sprite sprite, string skinName = "", string shaderName = "Spine/Skeleton")
		{
			RegionAttachment regionAttachment = sprite.ToRegionAttachment(shaderName);
			int slotIndex = skeletonData.FindSlotIndex(slotName);
			Skin skin = skeletonData.defaultSkin;
			if (skinName != string.Empty)
			{
				skin = skeletonData.FindSkin(skinName);
			}
			skin.AddAttachment(slotIndex, regionAttachment.Name, regionAttachment);
			return regionAttachment;
		}

		public static RegionAttachment ToRegionAttachment(this Sprite sprite, string shaderName = "Spine/Skeleton")
		{
			SpriteAttachmentLoader spriteAttachmentLoader = new SpriteAttachmentLoader(sprite, Shader.Find(shaderName));
			return spriteAttachmentLoader.NewRegionAttachment(null, sprite.name, string.Empty);
		}
	}
}
