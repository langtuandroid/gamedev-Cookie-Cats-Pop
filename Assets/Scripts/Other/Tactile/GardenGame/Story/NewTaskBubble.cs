using System;
using UnityEngine;

namespace Tactile.GardenGame.Story
{
	public class NewTaskBubble : MonoBehaviour
	{
		public void Initialize(MapTask task)
		{
			this.taskImage.SetTexture(task.Icon);
			this.title.text = L.Get(task.Title);
			this.starPrice.text = task.StarsRequired.ToString();
			this.starPivot.SetActive(task.StarsRequired > 0);
		}

		public UITextureQuad taskImage;

		public UILabel title;

		public UILabel starPrice;

		public GameObject starPivot;

		public Vector3 offscreenOffset;
	}
}
