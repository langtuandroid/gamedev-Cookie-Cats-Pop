using System;
using Spine;
using Spine.Unity.Modules;
using UnityEngine;

public static class SkeletonExtensions
{
	public static Color GetColor(this Skeleton s)
	{
		return new Color(s.r, s.g, s.b, s.a);
	}

	public static Color GetColor(this RegionAttachment a)
	{
		return new Color(a.r, a.g, a.b, a.a);
	}

	public static Color GetColor(this MeshAttachment a)
	{
		return new Color(a.r, a.g, a.b, a.a);
	}

	public static void SetColor(this Skeleton skeleton, Color color)
	{
		skeleton.A = color.a;
		skeleton.R = color.r;
		skeleton.G = color.g;
		skeleton.B = color.b;
	}

	public static void SetColor(this Skeleton skeleton, Color32 color)
	{
		skeleton.A = (float)color.a * 0.003921569f;
		skeleton.R = (float)color.r * 0.003921569f;
		skeleton.G = (float)color.g * 0.003921569f;
		skeleton.B = (float)color.b * 0.003921569f;
	}

	public static void SetColor(this Slot slot, Color color)
	{
		slot.A = color.a;
		slot.R = color.r;
		slot.G = color.g;
		slot.B = color.b;
	}

	public static void SetColor(this Slot slot, Color32 color)
	{
		slot.A = (float)color.a * 0.003921569f;
		slot.R = (float)color.r * 0.003921569f;
		slot.G = (float)color.g * 0.003921569f;
		slot.B = (float)color.b * 0.003921569f;
	}

	public static void SetColor(this RegionAttachment attachment, Color color)
	{
		attachment.A = color.a;
		attachment.R = color.r;
		attachment.G = color.g;
		attachment.B = color.b;
	}

	public static void SetColor(this RegionAttachment attachment, Color32 color)
	{
		attachment.A = (float)color.a * 0.003921569f;
		attachment.R = (float)color.r * 0.003921569f;
		attachment.G = (float)color.g * 0.003921569f;
		attachment.B = (float)color.b * 0.003921569f;
	}

	public static void SetColor(this MeshAttachment attachment, Color color)
	{
		attachment.A = color.a;
		attachment.R = color.r;
		attachment.G = color.g;
		attachment.B = color.b;
	}

	public static void SetColor(this MeshAttachment attachment, Color32 color)
	{
		attachment.A = (float)color.a * 0.003921569f;
		attachment.R = (float)color.r * 0.003921569f;
		attachment.G = (float)color.g * 0.003921569f;
		attachment.B = (float)color.b * 0.003921569f;
	}

	public static void SetPosition(this Bone bone, Vector2 position)
	{
		bone.X = position.x;
		bone.Y = position.y;
	}

	public static void SetPosition(this Bone bone, Vector3 position)
	{
		bone.X = position.x;
		bone.Y = position.y;
	}

	public static Vector2 GetSkeletonSpacePosition(this Bone bone)
	{
		return new Vector2(bone.worldX, bone.worldY);
	}

	public static Vector3 GetWorldPosition(this Bone bone, Transform parentTransform)
	{
		return parentTransform.TransformPoint(new Vector3(bone.worldX, bone.worldY));
	}

	public static void PoseWithAnimation(this Skeleton skeleton, string animationName, float time, bool loop)
	{
		Spine.Animation animation = skeleton.data.FindAnimation(animationName);
		if (animation == null)
		{
			return;
		}
		animation.Apply(skeleton, 0f, time, loop, null);
	}

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

	private const float ByteToFloat = 0.003921569f;
}
