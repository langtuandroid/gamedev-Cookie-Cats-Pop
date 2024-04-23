using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : Board
{
	public GameBoard(ILevelSession session)
	{
		this.session = session;
	}

	public Vector2 Size { get; private set; }

	public List<MatchFlag> ColorGroups { get; private set; }

	public int TotalRows { get; private set; }

	private LevelAsset Level { get; set; }

	protected virtual bool ShouldAddTopLine
	{
		get
		{
			return true;
		}
	}

	protected virtual bool ShouldSpawnNormalLevelTiles
	{
		get
		{
			return true;
		}
	}

	protected override Vector3 TileOffset
	{
		get
		{
			return new Vector3((float)(this.Level.NumTilesX - 1) * -0.5f, -0.5f, 0f);
		}
	}

	public override ITopology Topology
	{
		get
		{
			return this.topology;
		}
	}

	public override void BuildLevel()
	{
		this.Level = (this.session.Level.LevelAsset as LevelAsset);
		base.TileSize = SingletonAsset<PieceDatabase>.Instance.GetPiece<NormalPiece>("Blue").gamePrefab.gameObject.GetElementSize().x;
		Vector2 vector = Vector2.one * base.TileSize;
		this.topology = BubbleTopology.CreateFromTotal(this.Level.NumTilesX, 1999, 1f);
		this.Size = new Vector2(this.topology.TotalWidth * vector.x, this.topology.TotalHeight * vector.x);
		if (this.ShouldSpawnNormalLevelTiles)
		{
			this.SetupPiecesFromLevel(this.Level.tiles);
		}
		if (this.ShouldAddTopLine)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(SingletonAsset<LevelVisuals>.Instance.topLine, base.Root, true);
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localPosition = Vector3.up * (gameObject.GetElementSize().y * 0.5f - this.GetTrimmedTop());
			gameObject.GetElement().Size = new Vector2(this.Size.x, 16f);
			gameObject.SetLayerRecursively(base.Root.gameObject.layer);
		}
	}

	public void FitToEnclosingElement(UIElement e)
	{
		this.enclosingElement = e;
		base.Root.parent = e.transform;
		float num = e.Size.x / this.Size.x;
		base.Root.localScale = new Vector3(num, num, 1f);
		base.Root.localPosition = this.CalculateScrollPosition();
		PowerResolvementHelper.scale = base.Root.localScale;
	}

	public Piece GetLowestPiece()
	{
		Coord coord = new Coord(-1, -1);
		Tile a = Tile.Invalid;
		foreach (Tile tile in base.GetOccupiedTiles())
		{
			if (tile.Coord.y > coord.y)
			{
				coord = tile.Coord;
				a = tile;
			}
		}
		return (!(a == Tile.Invalid)) ? a.Piece : null;
	}

	public int GetNumberOfRowsClearedByPlayer()
	{
		return Mathf.Max(0, this.TotalRows - (this.GetLowestRowIndex() + 1));
	}

	public int GetLowestRowIndex()
	{
		Piece lowestPiece = this.GetLowestPiece();
		if (lowestPiece == null)
		{
			return 0;
		}
		return base.GetTile(lowestPiece.TileIndex).Coord.y;
	}

	public bool IsPieceBelowRow(Piece piece, int row)
	{
		return Mathf.Max(0, this.GetLowestRowIndex() - row) < base.GetTile(piece.TileIndex).Coord.y;
	}

	public float GetBoardRowNrInWorldSpace(int row)
	{
		float num = base.Root.localScale.y * base.TileSize;
		return (float)row * num * BubbleTopology.LINE_DIST_FACTOR;
	}

	public Vector3 GetBoardRowNrWorldPosition(int row)
	{
		return base.Root.TransformPoint(new Vector3(0f, (float)(-(float)row) * base.TileSize * BubbleTopology.LINE_DIST_FACTOR, 0f));
	}

	public float TileSizeInParentSpace()
	{
		return base.Root.localScale.y * base.TileSize;
	}

	protected Vector3 CalculateScrollPosition()
	{
		if (this.enclosingElement == null)
		{
			return Vector3.zero;
		}
		float num = base.Root.localScale.y * base.TileSize;
		float num2 = float.MaxValue;
		Piece lowestPiece = this.GetLowestPiece();
		if (lowestPiece != null)
		{
			num2 = base.GetTile(lowestPiece.TileIndex).LocalPosition.y * base.Root.localScale.y - num * 0.5f;
		}
		Vector2 parentSpaceEdges = this.GetParentSpaceEdges();
		float num3 = parentSpaceEdges.x - num2;
		if (num3 < parentSpaceEdges.y)
		{
			num3 = parentSpaceEdges.y;
		}
		return new Vector3(0f, num3, 0f);
	}

	public override bool TileIndexDisabled(int index)
	{
		return index < this.trimmedTopIndex;
	}

	public float GetTrimmedTop()
	{
		return (float)this.trimmedTopRow * base.TileSize * BubbleTopology.LINE_DIST_FACTOR;
	}

	public IEnumerator AnimatePanToBottom()
	{
		yield return this.AnimatePanTo(this.CalculateScrollPosition().y);
		yield break;
	}

	private IEnumerator AnimatePanTo(float targetY)
	{
		Vector3 oldPos = base.Root.localPosition;
		Vector3 newPos = oldPos;
		newPos.y = targetY;
		if (Mathf.Approximately(oldPos.y, newPos.y))
		{
			yield break;
		}
		if (this.allowedToPanFunction != null)
		{
			while (!this.allowedToPanFunction())
			{
				yield return null;
			}
		}
		yield return FiberAnimation.MoveLocalTransform(base.Root, oldPos, newPos, AnimationCurve.EaseInOut(0f, 0f, 1f, 1f), Mathf.Abs(oldPos.y - newPos.y) / 500f);
		yield break;
	}

	public virtual bool TileIndexIsAtTop(int tileIndex)
	{
		return this.topology.GetCoordFromIndex(tileIndex).y == this.trimmedTopRow;
	}

	protected void SetupPiecesFromLevel(List<PuzzleLevel.TileInfo> tiles)
	{
		LevelSession levelSession = this.session as LevelSession;
		List<string> list = new List<string>();
		for (int i = 0; i < tiles.Count; i++)
		{
			PuzzleLevel.TileInfo tileInfo = tiles[i];
			if (!tileInfo.piece.id.IsEmpty)
			{
				PieceId id2 = tileInfo.piece.id;
				if (!SingletonAsset<PieceDatabase>.Instance.specialMatchIds.Contains(id2.MatchFlag))
				{
					Piece piece = base.SpawnPieceAt(i, id2);
					piece.Direction = tileInfo.piece.direction;
					if (piece is BossCheckpointPiece)
					{
						BossCheckpointPiece bossCheckpointPiece = (BossCheckpointPiece)piece;
						bossCheckpointPiece.CheckpointIdx = tileInfo.piece.amount;
					}
				}
				if (tileInfo.piece.id.TypeName == "FillPowerPiece")
				{
					for (int j = 0; j < LevelAsset.RANDOM_GROUPS.Length; j++)
					{
						if (id2.MatchFlag == LevelAsset.RANDOM_GROUPS[j] && !list.Contains(id2.MatchFlag))
						{
							list.Add(id2.MatchFlag);
							break;
						}
					}
				}
				foreach (PuzzleLevel.TileElementInfo tileElementInfo in tileInfo.attachments)
				{
					Piece piece2 = base.SpawnPieceAt(i, tileElementInfo.id);
					piece2.Direction = tileElementInfo.direction;
				}
			}
		}
		int[] array = CollectionExtensions.Range(0, tiles.Count);
		array.Shuffle<int>();
		if (list.Count > 0)
		{
			List<MatchFlag> list2 = new List<MatchFlag>();
			List<MatchFlag> list3 = new List<MatchFlag>(levelSession.AdjustedEnabledPowerupColors);
			list3.Shuffle<MatchFlag>();
			List<MatchFlag> spawnColors = levelSession.GetSpawnColors(null);
			for (int k = 0; k < spawnColors.Count; k++)
			{
				if (!list3.Contains(spawnColors[k]))
				{
					list2.Add(spawnColors[k]);
				}
			}
			list2.Shuffle<MatchFlag>();
			this.ColorGroups = new List<MatchFlag>();
			for (int l = 0; l < spawnColors.Count; l++)
			{
				this.ColorGroups.Add(string.Empty);
			}
			for (int m = 0; m < list.Count; m++)
			{
				for (int n = 0; n < LevelAsset.RANDOM_GROUPS.Length; n++)
				{
					if (list[m] == LevelAsset.RANDOM_GROUPS[n])
					{
						int num = n;
						if (num < this.ColorGroups.Count)
						{
							if (list3.Count > 0)
							{
								this.ColorGroups[num] = list3[0];
								list3.RemoveAt(0);
							}
							else if (list2.Count > 0)
							{
								this.ColorGroups[num] = list2[0];
								list2.RemoveAt(0);
							}
						}
						break;
					}
				}
			}
			if (list3.Count > 0)
			{
				list2.AddRange(list3);
				list2.Shuffle<MatchFlag>();
			}
			for (int num2 = 0; num2 < this.ColorGroups.Count; num2++)
			{
				if (this.ColorGroups[num2] == string.Empty && list2.Count > 0)
				{
					this.ColorGroups[num2] = list2[0];
					list2.RemoveAt(0);
				}
			}
		}
		else
		{
			this.ColorGroups = new List<MatchFlag>(levelSession.GetSpawnColors(null));
			this.ColorGroups.Shuffle<MatchFlag>();
		}
		int[] array2 = array;
		for (int num3 = 0; num3 < array2.Length; num3++)
		{
			int num4 = array2[num3];
			PuzzleLevel.TileInfo tileInfo2 = tiles[num4];
			if (!tileInfo2.piece.id.IsEmpty)
			{
				PieceId id = tileInfo2.piece.id;
				if (id.MatchFlag == "?")
				{
					base.SpawnPieceAt(num4, new PieceId(id.TypeName, levelSession.GetSpawnColors((MatchFlag c) => this.Level == null || id.TypeName != "FillPowerPiece" || levelSession.AdjustedEnabledPowerupColors.Contains(c)).GetRandom<MatchFlag>()));
				}
				else
				{
					for (int num5 = 0; num5 < LevelAsset.RANDOM_GROUPS.Length; num5++)
					{
						if (id.MatchFlag == LevelAsset.RANDOM_GROUPS[num5])
						{
							base.SpawnPieceAt(num4, new PieceId(id.TypeName, this.ColorGroups[num5]));
						}
					}
				}
			}
		}
		this.CalculateTrimmedTop();
		this.TotalRows += tiles.Count / 23 * 2;
	}

	protected virtual void CalculateTrimmedTop()
	{
		this.trimmedTopRow = int.MaxValue;
		foreach (Tile tile in base.GetOccupiedTiles())
		{
			foreach (Piece piece in tile.Pieces)
			{
				Coord coordFromIndex = this.topology.GetCoordFromIndex(tile.Index);
				if (coordFromIndex.y < this.trimmedTopRow)
				{
					this.trimmedTopRow = coordFromIndex.y;
				}
				piece.AlignToTile();
				piece.AfterBoardSetup(this);
			}
		}
		this.trimmedTopIndex = this.topology.GetIndexFromCoord(new Coord(0, this.trimmedTopRow));
	}

	private bool PieceIsCloseToBottom(Tile pieceTile, Piece lowest)
	{
		return lowest == null || base.GetTile(lowest.TileIndex).Coord.y - pieceTile.Coord.y < 10;
	}

	public virtual List<MatchFlag> GetColorsAmongPieces(bool withinBoundsOnly)
	{
		Piece lowestPiece = this.GetLowestPiece();
		List<MatchFlag> list = new List<MatchFlag>();
		foreach (Tile pieceTile in base.GetOccupiedTiles())
		{
			if (!(pieceTile.Piece == null) && !(pieceTile.Piece.MatchFlag == string.Empty))
			{
				if (!withinBoundsOnly || this.PieceIsCloseToBottom(pieceTile, lowestPiece))
				{
					if (!list.Contains(pieceTile.Piece.MatchFlag))
					{
						list.Add(pieceTile.Piece.MatchFlag);
					}
				}
			}
		}
		return list;
	}

	public List<Piece> GetAllPiecesOnBoard()
	{
		List<Piece> list = new List<Piece>();
		foreach (KeyValuePair<int, List<Piece>> keyValuePair in this.elementsAtIndex)
		{
			foreach (Piece item in keyValuePair.Value)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public List<CPPiece> GetPiecesInRange(Vector2 pos, float range)
	{
		List<int> tilesInRange = this.GetTilesInRange(pos, range);
		List<CPPiece> list = new List<CPPiece>();
		foreach (int index in tilesInRange)
		{
			if (base.IsTileOccupied(index))
			{
				Tile tile = base.GetTile(index);
				if (!(tile.Piece == null) && tile.Piece.gameObject.activeSelf)
				{
					list.Add(tile.Piece as CPPiece);
				}
			}
		}
		return list;
	}

	private List<int> GetTilesInRange(Vector2 pos, float range)
	{
		List<int> list = new List<int>();
		int tileAt = base.GetTileAt(pos);
		if (tileAt == -1)
		{
			return list;
		}
		Stack<int> stack = new Stack<int>();
		HashSet<int> hashSet = new HashSet<int>();
		stack.Push(tileAt);
		while (stack.Count > 0)
		{
			int num = stack.Pop();
			if (!hashSet.Contains(num))
			{
				float num2 = Vector2.Distance(base.GetTilePosition(num), pos);
				if (num2 < range)
				{
					list.Add(num);
					foreach (int num3 in this.topology.AllValidNeighbours(num))
					{
						if (!hashSet.Contains(num3))
						{
							stack.Push(num3);
						}
					}
				}
				hashSet.Add(num);
			}
		}
		return list;
	}

	private Vector2 GetParentSpaceEdges()
	{
		float num = base.Root.localScale.y * base.TileSize;
		Rect rectInWorldPos = this.enclosingElement.GetRectInWorldPos();
		Vector3 position = new Vector3(0f, rectInWorldPos.BottomRight().y, 0f);
		float y = base.Root.parent.InverseTransformPoint(position).y;
		Vector3 position2 = new Vector3(0f, rectInWorldPos.TopLeft().y + (float)this.trimmedTopRow * num * BubbleTopology.LINE_DIST_FACTOR, 0f);
		float y2 = base.Root.parent.InverseTransformPoint(position2).y;
		return new Vector2(y, y2);
	}

	private const int NUMBER_OF_TILES_FOR_TWO_ROWS = 23;

	public Func<bool> allowedToPanFunction;

	protected readonly ILevelSession session;

	private UIElement enclosingElement;

	private int trimmedTopRow;

	private int trimmedTopIndex;

	private BubbleTopology topology;
}
