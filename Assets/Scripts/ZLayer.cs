using System;

public enum ZLayer
{
	BehindWall = -1500,
	AboveWall = -500,
	BoardTile = 0,
	BackgroundAttachment = 50,
	BelowBoardPiece = 75,
	BoardPiece = 100,
	BoardPieceEffect = 125,
	ForegroundAttachment = 150,
	BoardMask = 200,
	BoardEdge = 300,
	AboveTilesAndPieces = 500,
	AboveTilesAndPieces2 = 550,
	AboveTilesAndPieces3 = 560,
	EffectOverlay = 600
}
