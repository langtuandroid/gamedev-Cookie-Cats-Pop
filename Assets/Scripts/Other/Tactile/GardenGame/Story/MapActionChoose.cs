using System;
using System.Collections;
using Tactile.GardenGame.MapSystem;
using UnityEngine;

namespace Tactile.GardenGame.Story
{
	public class MapActionChoose : MapAction
	{
		public string MapPropId
		{
			get
			{
				return this.mapDecorationId;
			}
			set
			{
				this.mapDecorationId = value;
			}
		}

		public override IEnumerator Logic(IStoryMapController map)
		{
			MapProp mapProp = map.GetMapComponent<MapProp>(this.mapDecorationId);
			if (mapProp == null)
			{
				yield break;
			}
			yield return map.Dialog.CloseDialog();
			yield return map.ChooseProp(mapProp);
			yield break;
		}

		public void ChooseAutomatically(IStoryMapController map)
		{
			MapProp mapComponent = map.GetMapComponent<MapProp>(this.mapDecorationId);
			if (mapComponent == null)
			{
				return;
			}
			foreach (MapProp.Variation variation in mapComponent.Variations)
			{
				if (variation.IsPickable)
				{
					mapComponent.CurrentVariation = variation;
					break;
				}
			}
		}

		public override bool IsAllowedWhenSkipping
		{
			get
			{
				return true;
			}
		}

		[SerializeField]
		private string mapDecorationId;
	}
}
