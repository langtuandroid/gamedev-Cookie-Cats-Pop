using System;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	public class MapIdlepoint : MapComponent
	{
		protected override void Initialized()
		{
			if (base.OwnerMap == null)
			{
				return;
			}
			this.ownerProp = this.FindPropOwner();
		}

		private MapProp FindPropOwner()
		{
			MapProp result = null;
			Transform transform = base.transform;
			while (transform != null)
			{
				MapProp component = transform.GetComponent<MapProp>();
				if (component != null)
				{
					result = component;
				}
				transform = transform.parent;
			}
			return result;
		}

		public bool Enabled
		{
			get
			{
				return this.ownerProp == null || (this.ownerProp.CurrentVariation != null && this.ownerProp.CurrentVariation.IsPickable);
			}
		}

		[SerializeField]
		public Location Location;

		private MapProp ownerProp;
	}
}
