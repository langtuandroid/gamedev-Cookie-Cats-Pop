using System;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	[ExecuteInEditMode]
	public class MapObjectID : MapComponent
	{
		public string Id
		{
			get
			{
				return this.id;
			}
		}

		[SerializeField]
		private string id;
	}
}
