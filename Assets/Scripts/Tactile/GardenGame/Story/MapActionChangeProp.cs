using System;
using System.Collections;
using Tactile.GardenGame.MapSystem;
using UnityEngine;

namespace Tactile.GardenGame.Story
{
	public class MapActionChangeProp : MapAction
	{
		public string DecorationObjectId
		{
			get
			{
				return this.decorationObjectId;
			}
			set
			{
				this.decorationObjectId = value;
			}
		}

		public int DecorationID
		{
			get
			{
				return this.decorationID;
			}
			set
			{
				this.decorationID = value;
			}
		}

		public override IEnumerator Logic(IStoryMapController map)
		{
			map.PropsManager.SetPropSkin(this.DecorationObjectId, this.DecorationID);
			MapProp mapProp = map.GetMapComponent<MapProp>(this.decorationObjectId);
			if (mapProp == null)
			{
				yield break;
			}
			mapProp.SetCurrentVariation(this.decorationID);
			yield return mapProp.WaitForBuildAnimation();
			yield break;
		}

		public override bool IsAllowedWhenSkipping
		{
			get
			{
				return true;
			}
		}

		[SerializeField]
		private string decorationObjectId;

		[SerializeField]
		private int decorationID;
	}
}
