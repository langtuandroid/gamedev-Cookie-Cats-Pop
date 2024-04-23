using System;
using System.Collections.Generic;

public class BossLevel : LevelAsset
{
	public List<PuzzleLevel.TileInfo> GetTiles()
	{
		List<PuzzleLevel.TileInfo> list = new List<PuzzleLevel.TileInfo>();
		foreach (BossLevelStage bossLevelStage in this.bossLevelStages)
		{
			list.AddRange(bossLevelStage.tiles);
		}
		return list;
	}

	public List<BossLevelStage> bossLevelStages = new List<BossLevelStage>();
}
