using System;
using UnityEngine;

namespace Tactile.GardenGame
{
	public class MapTile : MonoBehaviour
	{
		public Bounds GetBoundsInParentSpace()
		{
			Renderer component = base.GetComponent<Renderer>();
			Bounds bounds = component.bounds;
			bounds.min += base.transform.localPosition;
			bounds.max += base.transform.localPosition;
			return bounds;
		}

		private void OnEnable()
		{
			foreach (Renderer renderer in base.GetComponentsInChildren<Renderer>())
			{
				renderer.sortingOrder = 0;
			}
		}
	}
}
