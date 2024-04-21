using System;
using System.Collections.Generic;
using UnityEngine;

public interface IBoardTileAccess
{
	void SetPieceTileIndex(Piece e, int newIndex);

	Piece GetPieceAtTileIndex(int index);

	bool TileIndexDisabled(int index);

	Vector3 GetTilePosition(int index);

	List<Piece> GetPiecesAtTileIndex(int index);
}
