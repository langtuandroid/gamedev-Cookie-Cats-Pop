using System;
using System.Collections.Generic;
using UnityEngine;

public class BossLevelStage : ScriptableObject
{
	[HideInInspector]
	public List<PuzzleLevel.TileInfo> tiles = new List<PuzzleLevel.TileInfo>();

	[HideInInspector]
	public BossPathType bossPathType;
}
