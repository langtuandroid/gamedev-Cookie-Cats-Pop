using System;
using UnityEngine;

namespace Tactile.GardenGame.Story
{
	public class BrowseTasksButton : MonoBehaviour
	{
		public void Initialize(bool shouldGiveAttention)
		{
			this.scalePulsator.enabled = shouldGiveAttention;
		}

		[SerializeField]
		private ScalePulsator scalePulsator;

		private StoryManager storyManager;
	}
}
