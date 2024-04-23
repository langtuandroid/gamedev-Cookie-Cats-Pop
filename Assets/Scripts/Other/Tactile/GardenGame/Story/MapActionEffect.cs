using System;
using System.Collections;
using Tactile.GardenGame.MapSystem;
using UnityEngine;

namespace Tactile.GardenGame.Story
{
	public class MapActionEffect : MapAction
	{
		public string EffectId
		{
			get
			{
				return this.effectId;
			}
			set
			{
				this.effectId = value;
			}
		}

		public override IEnumerator Logic(IStoryMapController map)
		{
			MapEffect mapEffect = map.GetMapComponent<MapEffect>(this.EffectId);
			if (mapEffect == null)
			{
				yield break;
			}
			yield return mapEffect.Play();
			yield break;
		}

		[SerializeField]
		private string effectId;
	}
}
