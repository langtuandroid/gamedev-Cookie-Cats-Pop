using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	public class MapHeightMap
	{
		public static void UpdateHeightmap(Camera renderCamera, RenderTexture heightMapTexture)
		{
		}

		public static void Initialize(MapAreas areas)
		{
		}

		private static void RenderObjectCreated(GameObject go)
		{
			MapClickableVisibilityDelegator mapClickableVisibilityDelegator = go.AddComponent<MapClickableVisibilityDelegator>();
			MapClickableVisibilityDelegator mapClickableVisibilityDelegator2 = mapClickableVisibilityDelegator;
			if (MapHeightMap._003C_003Ef__mg_0024cache1 == null)
			{
				MapHeightMap._003C_003Ef__mg_0024cache1 = new Action(MapHeightMap.Hidden);
			}
			mapClickableVisibilityDelegator2.OnHidden = MapHeightMap._003C_003Ef__mg_0024cache1;
			MapClickableVisibilityDelegator mapClickableVisibilityDelegator3 = mapClickableVisibilityDelegator;
			if (MapHeightMap._003C_003Ef__mg_0024cache2 == null)
			{
				MapHeightMap._003C_003Ef__mg_0024cache2 = new Action(MapHeightMap.Shown);
			}
			mapClickableVisibilityDelegator3.OnShown = MapHeightMap._003C_003Ef__mg_0024cache2;
		}

		private static void Hidden()
		{
			MapHeightMap.numVisible--;
		}

		private static void Shown()
		{
			MapHeightMap.numVisible++;
		}

		private static int numVisible;

		[CompilerGenerated]
		private static Action<GameObject> _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static Action _003C_003Ef__mg_0024cache1;

		[CompilerGenerated]
		private static Action _003C_003Ef__mg_0024cache2;
	}
}
