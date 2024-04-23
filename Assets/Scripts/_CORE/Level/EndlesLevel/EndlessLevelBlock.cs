using System;
using System.Collections.Generic;
using UnityEngine;

public class EndlessLevelBlock : ScriptableObject
{
	[HideInInspector]
	public List<PuzzleLevel.TileInfo> tiles = new List<PuzzleLevel.TileInfo>();
}
