using System;
using System.Collections;
using UnityEngine;

namespace Tactile.GardenGame.Story
{
	public class MapActionMusic : MapAction
	{
		public SoundDefinition SoundDefinition
		{
			get
			{
				return this.soundDefinition;
			}
			set
			{
				this.soundDefinition = value;
			}
		}

		public override IEnumerator Logic(IStoryMapController map)
		{
			map.PlayMusic(this.soundDefinition);
			yield break;
		}

		[SerializeField]
		private SoundDefinition soundDefinition;
	}
}
