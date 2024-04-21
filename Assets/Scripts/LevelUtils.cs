using System;
using System.Collections.Generic;

public static class LevelUtils
{
	public static List<PuzzleLevel.TileInfo> GetAllPiecesWithType<T>(LevelAsset level, MatchFlag[] colors = null)
	{
		List<PuzzleLevel.TileInfo> tiles;
		if (level is EndlessLevel)
		{
			tiles = ((EndlessLevel)level).GetTiles();
		}
		else if (level is BossLevel)
		{
			tiles = ((BossLevel)level).GetTiles();
		}
		else
		{
			tiles = level.tiles;
		}
		if (colors != null)
		{
			List<PieceId> matchables = new List<PieceId>();
			for (int i = 0; i < colors.Length; i++)
			{
				matchables.Add(PieceId.Create<T>(colors[i]));
			}
			return tiles.FindAll(delegate(PuzzleLevel.TileInfo tileInfo)
			{
				for (int j = 0; j < matchables.Count; j++)
				{
					if (tileInfo.piece.id == matchables[j])
					{
						return true;
					}
				}
				return false;
			});
		}
		string typeName = typeof(T).Name;
		return tiles.FindAll((PuzzleLevel.TileInfo tileInfo) => tileInfo.piece.id.TypeName == typeName);
	}

	public static bool PieceWithTypeExist<T>(LevelAsset level, MatchFlag[] colors = null)
	{
		List<PuzzleLevel.TileInfo> allPiecesWithType = LevelUtils.GetAllPiecesWithType<T>(level, colors);
		return allPiecesWithType.Count > 0;
	}
}
