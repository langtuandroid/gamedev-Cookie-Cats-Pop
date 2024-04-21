using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct Tile
{
	public Tile(Board board, int index)
	{
		this = default(Tile);
		this.Board = board;
		this.Index = index;
	}

	public Board Board { get; private set; }

	private IBoardTileAccess BoardPrivateAccess
	{
		get
		{
			return this.Board;
		}
	}

	public int Index { get; private set; }

	public Coord Coord
	{
		get
		{
			return this.Board.Topology.GetCoordFromIndex(this.Index);
		}
	}

	public Piece Piece
	{
		get
		{
			for (int i = 0; i < this.Pieces.Count; i++)
			{
				Piece piece = this.Pieces[i];
				if (piece.TileLayer == 0)
				{
					return piece;
				}
			}
			return null;
		}
	}

	public List<Piece> Pieces
	{
		get
		{
			if (this.BoardPrivateAccess == null)
			{
				return Tile.staticEmptyDummyList;
			}
			List<Piece> piecesAtTileIndex = this.BoardPrivateAccess.GetPiecesAtTileIndex(this.Index);
			if (piecesAtTileIndex == null)
			{
				return Tile.staticEmptyDummyList;
			}
			return piecesAtTileIndex;
		}
	}

	public IEnumerable<Piece> PieceAttachments
	{
		get
		{
			foreach (Piece piece in this.Pieces)
			{
				if (piece != this.Piece)
				{
					yield return piece;
				}
			}
			yield break;
		}
	}

	public T FindPiece<T>() where T : Piece
	{
		foreach (Piece piece in this.Pieces)
		{
			if (piece is T)
			{
				return piece as T;
			}
		}
		return (T)((object)null);
	}

	public IEnumerable<Tile.PieceWithComponent<T>> PiecesWithComponent<T>() where T : Component
	{
		foreach (Piece p in this.Pieces)
		{
			T component = p.gameObject.GetComponent<T>();
			if (component != null)
			{
				yield return new Tile.PieceWithComponent<T>
				{
					piece = p,
					component = component
				};
			}
		}
		yield break;
	}

	public bool Disabled
	{
		get
		{
			return this.BoardPrivateAccess.TileIndexDisabled(this.Index);
		}
	}

	public IEnumerable<Tile> GetNeighbours()
	{
		foreach (Direction d in this.Board.Topology.AllDirections())
		{
			int i = this.Board.Topology.GetNeighbourIndex(this.Index, d);
			if (i >= 0)
			{
				yield return new Tile(this.Board, i);
			}
		}
		yield break;
	}

	public IEnumerable<int> GetNeighbourIndexes()
	{
		foreach (Direction d in this.Board.Topology.AllDirections())
		{
			int i = this.Board.Topology.GetNeighbourIndex(this.Index, d);
			if (i >= 0)
			{
				yield return i;
			}
		}
		yield break;
	}

	public IEnumerable<Tile> GetOccupiedNeighbours()
	{
		foreach (Direction d in this.Board.Topology.AllDirections())
		{
			int i = this.Board.Topology.GetNeighbourIndex(this.Index, d);
			if (i >= 0)
			{
				if (this.Board.IsTileOccupied(i))
				{
					yield return new Tile(this.Board, i);
				}
			}
		}
		yield break;
	}

	public IEnumerable<int> GetOccupiedNeighbourIndexes()
	{
		foreach (Direction d in this.Board.Topology.AllDirections())
		{
			int i = this.Board.Topology.GetNeighbourIndex(this.Index, d);
			if (i >= 0)
			{
				if (this.Board.IsTileOccupied(i))
				{
					yield return i;
				}
			}
		}
		yield break;
	}

	public Vector3 LocalPosition
	{
		get
		{
			return this.BoardPrivateAccess.GetTilePosition(this.Index);
		}
	}

	public Vector3 WorldPosition
	{
		get
		{
			return this.Board.Root.TransformPoint(this.LocalPosition);
		}
	}

	public Tile GetNeighbour(Direction dir)
	{
		int neighbourIndex = this.Board.Topology.GetNeighbourIndex(this.Index, dir);
		if (neighbourIndex >= 0)
		{
			return new Tile(this.Board, neighbourIndex);
		}
		return Tile.Invalid;
	}

	public Tile GetRelative(Coord offset)
	{
		Coord c = this.Coord + offset;
		int indexFromCoord = this.Board.Topology.GetIndexFromCoord(c);
		if (indexFromCoord >= 0)
		{
			return new Tile(this.Board, indexFromCoord);
		}
		return Tile.Invalid;
	}

	public override bool Equals(object obj)
	{
		if (obj is Tile)
		{
			return ((Tile)obj).Index == this.Index;
		}
		return base.Equals(obj);
	}

	public override int GetHashCode()
	{
		return this.Index;
	}

	public static bool operator !=(Tile a, Tile b)
	{
		return !(a == b);
	}

	public static bool operator ==(Tile a, Tile b)
	{
		return a.Index == b.Index;
	}

	public static readonly Tile Invalid = new Tile(null, -1);

	private static readonly List<Piece> staticEmptyDummyList = new List<Piece>();

	public struct PieceWithComponent<T>
	{
		public Piece piece;

		public T component;
	}
}
