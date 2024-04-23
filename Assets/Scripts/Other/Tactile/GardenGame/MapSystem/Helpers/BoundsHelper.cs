using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem.Helpers
{
	public static class BoundsHelper
	{
		public static bool TryGetRendererBounds(GameObject gameObject, out Bounds bounds)
		{
			BoundsHelper.rendererResults.Clear();
			gameObject.GetComponentsInChildren<Renderer>(BoundsHelper.rendererResults);
			return BoundsHelper.TryGetRendererBounds(BoundsHelper.rendererResults, out bounds);
		}

		public static bool TryGetRendererBounds(List<Renderer> renderers, out Bounds bounds)
		{
			bounds = default(Bounds);
			if (renderers.Count == 0)
			{
				return false;
			}
			for (int i = 0; i < renderers.Count; i++)
			{
				if (i == 0)
				{
					bounds = renderers[i].bounds;
				}
				else
				{
					bounds.Encapsulate(renderers[i].bounds);
				}
			}
			return true;
		}

		private static readonly List<Renderer> rendererResults = new List<Renderer>();
	}
}
