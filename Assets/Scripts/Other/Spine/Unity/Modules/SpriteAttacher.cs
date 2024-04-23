using System;
using UnityEngine;

namespace Spine.Unity.Modules
{
	public class SpriteAttacher : MonoBehaviour
	{
		private void Start()
		{
			if (this.attachOnStart)
			{
				this.Attach();
			}
		}

		public void Attach()
		{
			SkeletonRenderer component = base.GetComponent<SkeletonRenderer>();
			if (this.loader == null)
			{
				this.loader = new SpriteAttachmentLoader(this.sprite, Shader.Find("Spine/Skeleton"));
			}
			if (this.attachment == null)
			{
				this.attachment = this.loader.NewRegionAttachment(null, this.sprite.name, string.Empty);
			}
			component.skeleton.FindSlot(this.slot).Attachment = this.attachment;
			if (!this.keepLoaderInMemory)
			{
				this.loader = null;
			}
		}

		public bool attachOnStart = true;

		public bool keepLoaderInMemory = true;

		public Sprite sprite;

		[SpineSlot("", "", false)]
		public string slot;

		private SpriteAttachmentLoader loader;

		private RegionAttachment attachment;
	}
}
