using System;
using System.Collections.Generic;
using UnityEngine;

public class BossCheckpoints
{
	public BossCheckpoints(BossLevelGameBoard gameBoard)
	{
		this.gameBoard = gameBoard;
	}

	public void ProcessStageCheckpoints()
	{
		this.checkpointIdx = 0;
		this.checkpointDirection = 1;
		this.checkpointPositions.Clear();
		List<BossCheckpointPiece> checkpointPieces = this.GetCheckpointPieces();
		this.CacheCheckpointPositions(checkpointPieces);
		this.DespawnCheckpointPieces(checkpointPieces);
	}

	public Vector3 GetNextCheckpointPosition()
	{
		bool flag = this.checkpointIdx == 0;
		bool flag2 = this.checkpointIdx == this.checkpointPositions.Count - 1;
		if (this.gameBoard.CurrentStage.bossPathType == BossPathType.BACK_AND_FORTH)
		{
			if (flag)
			{
				this.checkpointDirection = 1;
			}
			else if (flag2)
			{
				this.checkpointDirection = -1;
			}
		}
		this.checkpointIdx = (this.checkpointIdx + this.checkpointDirection) % this.checkpointPositions.Count;
		return this.checkpointPositions[this.checkpointIdx];
	}

	private void CacheCheckpointPositions(List<BossCheckpointPiece> checkpointPieces)
	{
		foreach (BossCheckpointPiece bossCheckpointPiece in checkpointPieces)
		{
			this.checkpointPositions.Add(bossCheckpointPiece.transform.localPosition);
		}
	}

	private List<BossCheckpointPiece> GetCheckpointPieces()
	{
		List<BossCheckpointPiece> list = new List<BossCheckpointPiece>();
		List<Piece> allPiecesOnBoard = this.gameBoard.GetAllPiecesOnBoard();
		foreach (Piece piece in allPiecesOnBoard)
		{
			BossCheckpointPiece bossCheckpointPiece = piece as BossCheckpointPiece;
			if (bossCheckpointPiece != null)
			{
				list.Add(bossCheckpointPiece);
			}
		}
		list.Sort();
		return list;
	}

	private void DespawnCheckpointPieces(List<BossCheckpointPiece> checkpointPieces)
	{
		foreach (BossCheckpointPiece bossCheckpointPiece in checkpointPieces)
		{
			Tile tile = this.gameBoard.GetTile(bossCheckpointPiece.TileIndex);
			this.gameBoard.DespawnPieceAt(tile);
		}
	}

	private readonly BossLevelGameBoard gameBoard;

	private readonly List<Vector3> checkpointPositions = new List<Vector3>();

	private int checkpointIdx;

	private int checkpointDirection;
}
