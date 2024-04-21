using System;
using System.Collections;
using TactileModules.GameCore.StreamingAssets;
using UnityEngine;

namespace Tactile.GardenGame.Story
{
	public class MapActionShowImage : MapAction
	{
		public string ImagePath
		{
			get
			{
				return this.imagePath;
			}
			set
			{
				this.imagePath = value;
			}
		}

		public override IEnumerator Logic(IStoryMapController map)
		{
			yield return map.ShowFullScreenImage(this.image);
			yield break;
		}

		[SerializeField]
		private string imagePath;

		[SerializeField]
		[StreamingAssetGroup("StoryImages")]
		public StreamingAsset image;
	}
}
