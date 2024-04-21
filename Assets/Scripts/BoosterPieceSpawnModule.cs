using System;
using System.Collections.Generic;
using UnityEngine;

public class BoosterPieceSpawnModule : LogicModule
{
	public override void Begin(LevelSession session)
	{
		LevelAsset levelAsset = (LevelAsset)session.Level.LevelAsset;
		int amountOfOneTurnBoosters = levelAsset.amountOfOneTurnBoosters;
		if (levelAsset.enableOneTurnBoosterShield)
		{
			this.boosterPieceTypes.Add("BoosterShield");
		}
		if (levelAsset.enableOneTurnBoosterSuperaim)
		{
			this.boosterPieceTypes.Add("BoosterSuperAim");
		}
		if (levelAsset.enableOneTurnBoosterSuperqueue)
		{
			this.boosterPieceTypes.Add("BoosterSuperQueue");
		}
		if (amountOfOneTurnBoosters <= 0 || this.boosterPieceTypes.Count <= 0)
		{
			return;
		}
		GameBoard board = session.TurnLogic.Board;
		List<Tile> list = new List<Tile>();
		foreach (Tile item in board.GetOccupiedTiles())
		{
			if (item.Piece is NormalPiece && item.Piece.IsBasicPiece)
			{
				list.Add(item);
			}
		}
		list.Shuffle<Tile>();
		List<Tile> range = list.GetRange(0, Mathf.Min(list.Count, amountOfOneTurnBoosters));
		foreach (Tile tile in range)
		{
			MatchFlag matchFlag = tile.Piece.MatchFlag;
			bool activeSelf = tile.Piece.gameObject.activeSelf;
			board.DespawnPieceAt(tile);
			PieceId randomBoosterPiece = this.GetRandomBoosterPiece(matchFlag);
			if (randomBoosterPiece != PieceId.Empty)
			{
				Piece piece = board.SpawnPieceAt(tile.Index, randomBoosterPiece);
				piece.gameObject.SetActive(activeSelf);
			}
		}
	}

	private PieceId GetRandomBoosterPiece(MatchFlag matchColor)
	{
		InventoryItem random = this.boosterPieceTypes.GetRandom<InventoryItem>();
		string text = random;
		if (text != null)
		{
			if (text == "BoosterShield")
			{
				return PieceId.Create<ShieldBoosterPiece>(matchColor);
			}
			if (text == "BoosterSuperAim")
			{
				return PieceId.Create<SuperaimBoosterPiece>(matchColor);
			}
			if (text == "BoosterSuperQueue")
			{
				return PieceId.Create<SuperqueueBoosterPiece>(matchColor);
			}
		}
		return PieceId.Empty;
	}

	private readonly List<InventoryItem> boosterPieceTypes = new List<InventoryItem>();
}
