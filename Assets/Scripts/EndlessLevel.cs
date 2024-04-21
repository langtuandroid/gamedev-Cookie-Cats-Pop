using System;
using System.Collections.Generic;

public class EndlessLevel : LevelAsset
{
	public List<PuzzleLevel.TileInfo> GetTiles()
	{
		List<PuzzleLevel.TileInfo> list = new List<PuzzleLevel.TileInfo>();
		list.AddRange(this.GetBlocksGroupTiles(this.endlessLevelBlocksEasy));
		list.AddRange(this.GetBlocksGroupTiles(this.endlessLevelBlocksMedium));
		list.AddRange(this.GetBlocksGroupTiles(this.endlessLevelBlocksHard));
		return list;
	}

	private List<PuzzleLevel.TileInfo> GetBlocksGroupTiles(List<EndlessLevelBlock> blocks)
	{
		List<PuzzleLevel.TileInfo> list = new List<PuzzleLevel.TileInfo>();
		foreach (EndlessLevelBlock endlessLevelBlock in blocks)
		{
			list.AddRange(endlessLevelBlock.tiles);
		}
		return list;
	}

	public List<EndlessLevelBlock> endlessLevelBlocksEasy = new List<EndlessLevelBlock>();

	public List<EndlessLevelBlock> endlessLevelBlocksMedium = new List<EndlessLevelBlock>();

	public List<EndlessLevelBlock> endlessLevelBlocksHard = new List<EndlessLevelBlock>();
}
