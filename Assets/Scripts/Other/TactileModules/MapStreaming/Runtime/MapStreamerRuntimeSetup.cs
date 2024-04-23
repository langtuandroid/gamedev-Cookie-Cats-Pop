using System;
using UnityEngine;

namespace TactileModules.MapStreaming.Runtime
{
	public class MapStreamerRuntimeSetup
	{
		public Vector2 totalContentSize;

		public bool spawnDynamicEndPiece;

		public Vector2 endPieceSpawnPosition = Vector2.zero;

		public GameObject endPiecePrefab;
	}
}
