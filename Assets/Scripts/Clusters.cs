using System;
using System.Collections.Generic;

public class Clusters
{
	public static List<Clusters.ClusterHit> FindAllClusters(IEnumerable<Tile> originTiles)
	{
		List<Clusters.ClusterHit> list = new List<Clusters.ClusterHit>();
		foreach (Tile tile in originTiles)
		{
			List<Clusters.ClusterHit> list2 = Clusters.FindClusterFromTile(tile);
			if (list2.Count >= 3 || tile.Piece is RainbowPiece)
			{
				list.AddRange(list2);
			}
		}
		return list;
	}

	public static List<Clusters.ClusterHit> FindClusterFromTile(Tile tile)
	{
		List<Clusters.ClusterHit> list = new List<Clusters.ClusterHit>();
		if (tile.Piece != null)
		{
			Clusters.ignoreTiles.Clear();
			list.Add(new Clusters.ClusterHit
			{
				tileIndex = tile.Index,
				step = 0
			});
			Clusters.BuildClusterFromTile2(tile.Index, tile.Board, list, 1, false);
		}
		return list;
	}

	public static bool ClusterContainsTile(int tileIndex, List<Clusters.ClusterHit> inList)
	{
		foreach (Clusters.ClusterHit clusterHit in inList)
		{
			if (clusterHit.tileIndex == tileIndex)
			{
				return true;
			}
		}
		return false;
	}

	private static bool TileCanClusterFurther(Tile t)
	{
		foreach (Piece piece in t.Pieces)
		{
			CPPiece cppiece = piece as CPPiece;
			if (cppiece.PreventFurtherClustering)
			{
				return false;
			}
		}
		return true;
	}

	private static bool TileCanBeClustered(Tile tile, Piece p)
	{
		foreach (Piece piece in tile.Pieces)
		{
			CPPiece cppiece = piece as CPPiece;
			if (cppiece.PreventClustering)
			{
				return false;
			}
		}
		return p is RainbowPiece || (p.MatchFlag == tile.Piece.MatchFlag && p.MatchFlag != string.Empty);
	}

	private static void BuildClusterFromTile2(int tileIndex, Board board, List<Clusters.ClusterHit> cluster, int level, bool matchAny)
	{
		Tile tile = board.GetTile(tileIndex);
		List<Tile> list = new List<Tile>();
		foreach (Tile tile2 in tile.GetNeighbours())
		{
			if (!(tile2.Piece == null))
			{
				if (!Clusters.ClusterContainsTile(tile2.Index, cluster))
				{
					if (!Clusters.ignoreTiles.Contains(tile2.Index))
					{
						if (matchAny || Clusters.TileCanBeClustered(tile2, tile.Piece))
						{
							cluster.Add(new Clusters.ClusterHit
							{
								tileIndex = tile2.Index,
								step = level
							});
							Clusters.ignoreTiles.Add(tile.Index);
							if (Clusters.TileCanClusterFurther(tile2))
							{
								list.Add(tile2);
							}
						}
					}
				}
			}
		}
		foreach (Tile tile3 in list)
		{
			Clusters.BuildClusterFromTile2(tile3.Index, board, cluster, level + 1, matchAny);
		}
	}

	private static void BuildClusterFromTile(int tileIndex, Board board, List<int> cluster, bool matchAny)
	{
		Tile tile = board.GetTile(tileIndex);
		if (tile.Piece == null)
		{
			return;
		}
		if (cluster.Contains(tile.Index))
		{
			return;
		}
		if (Clusters.ignoreTiles.Contains(tile.Index))
		{
			return;
		}
		cluster.Add(tile.Index);
		Clusters.ignoreTiles.Add(tile.Index);
		foreach (Tile tile2 in tile.GetNeighbours())
		{
			if (!(tile2.Piece == null))
			{
				if (matchAny || (tile2.Piece.MatchFlag == tile.Piece.MatchFlag && tile2.Piece.MatchFlag != string.Empty))
				{
					Clusters.BuildClusterFromTile(tile2.Index, board, cluster, matchAny);
				}
			}
		}
	}

	private static void BuildClusterFromTileSemiOptimized(int tileIndex, Board board, List<int> cluster, bool matchAny)
	{
		Tile tile = board.GetTile(tileIndex);
		if (tile.Piece == null)
		{
			return;
		}
		if (cluster.Contains(tile.Index))
		{
			return;
		}
		if (Clusters.ignoreTiles.Contains(tile.Index))
		{
			return;
		}
		cluster.Add(tile.Index);
		Clusters.ignoreTiles.Add(tile.Index);
		foreach (int num in tile.GetNeighbourIndexes())
		{
			if (!Clusters.ignoreTiles.Contains(num))
			{
				Tile tile2 = board.GetTile(num);
				if (!(tile2.Piece == null))
				{
					if (matchAny || (tile2.Piece.MatchFlag == tile.Piece.MatchFlag && tile2.Piece.MatchFlag != string.Empty))
					{
						Clusters.BuildClusterFromTileSemiOptimized(num, board, cluster, matchAny);
					}
				}
			}
		}
	}

	public static List<int> FindUnattachedTilesSemiOptimized(Board board, List<int> unattachedClouds)
	{
		List<int> list = new List<int>();
		Clusters.ignoreTiles = new HashSet<int>(unattachedClouds);
		foreach (int item in board.GetOccupiedTilesIndexes())
		{
			if (!Clusters.ignoreTiles.Contains(item))
			{
				List<int> list2 = new List<int>
				{
					item
				};
				HashSet<int> hashSet = new HashSet<int>
				{
					item
				};
				bool flag = false;
				for (int i = 0; i < list2.Count; i++)
				{
					Tile tile = board.GetTile(list2[i]);
					bool flag2 = ((GameBoard)board).TileIndexIsAtTop(tile.Index);
					bool flag3 = tile.Piece is CloudPiece;
					bool flag4;
					if (tile.Pieces.Count > 1)
					{
						flag4 = (tile.Pieces.Find((Piece x) => x is PropellerPiece) != null);
					}
					else
					{
						flag4 = false;
					}
					bool flag5 = flag4;
					if (!flag && (flag2 || flag3 || flag5))
					{
						flag = true;
					}
					foreach (int item2 in tile.GetOccupiedNeighbourIndexes())
					{
						if (!hashSet.Contains(item2))
						{
							list2.Add(item2);
							hashSet.Add(item2);
						}
					}
				}
				Clusters.ignoreTiles.UnionWith(hashSet);
				if (!flag)
				{
					list.AddRange(list2);
				}
			}
		}
		return list;
	}

	public static List<int> FindUnattachedClouds(Board board)
	{
		List<int> list = new List<int>();
		Clusters.ignoreTiles.Clear();
		foreach (Tile tile in board.GetOccupiedTiles())
		{
			CloudPiece x = tile.Piece as CloudPiece;
			if (!(x == null))
			{
				bool flag = false;
				foreach (Tile tile2 in tile.GetNeighbours())
				{
					if (tile2.Piece != null && !(tile2.Piece is CloudPiece))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					list.Add(tile.Index);
				}
			}
		}
		return list;
	}

	private static HashSet<int> ignoreTiles = new HashSet<int>();

	public struct ClusterHit
	{
		public int tileIndex;

		public int step;
	}
}
