using System;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	[ExecuteInEditMode]
	public class MapComponent : MonoBehaviour
	{
		public Map OwnerMap { get; private set; }

		public void Initialize(Map map)
		{
			this.OwnerMap = map;
			this.Initialized();
		}

		public virtual void Destroy()
		{
		}

		protected virtual void Initialized()
		{
		}

		public virtual int IntializationOrder
		{
			get
			{
				return 0;
			}
		}
	}
}
