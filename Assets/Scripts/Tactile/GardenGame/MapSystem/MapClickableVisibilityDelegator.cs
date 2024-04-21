using System;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	public class MapClickableVisibilityDelegator : MonoBehaviour
	{
		private void OnBecameVisible()
		{
			this.OnShown();
		}

		private void OnBecameInvisible()
		{
			this.OnHidden();
		}

		public Action OnShown;

		public Action OnHidden;
	}
}
