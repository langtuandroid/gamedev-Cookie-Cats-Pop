using System;
using UnityEngine;

namespace Tactile.GardenGame.Story
{
	public class StoryImageView : UIView
	{
		public void ToggleBackground(bool enable)
		{
			this.background.gameObject.SetActive(enable);
		}

		public UITextureQuad image;

		[SerializeField]
		private GameObject background;
	}
}
