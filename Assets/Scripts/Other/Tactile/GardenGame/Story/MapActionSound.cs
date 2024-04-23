using System;
using System.Collections;
using UnityEngine;

namespace Tactile.GardenGame.Story
{
	public class MapActionSound : MapAction
	{
		public ISoundDefinition SoundDefinition
		{
			get
			{
				return this.soundDefinition;
			}
		}

		public override IEnumerator Logic(IStoryMapController map)
		{
			this.soundDefinition.Play();
			yield break;
		}

		[SerializeField]
		private SoundDefinition soundDefinition;
	}
}
