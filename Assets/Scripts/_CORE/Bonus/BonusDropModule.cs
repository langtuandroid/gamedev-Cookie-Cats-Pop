using System;
using System.Collections.Generic;

public class BonusDropModule : LogicModule
{
	public override void Begin(LevelSession session)
	{
		if (Singleton<BonusDropManager>.Instance.CanSpawnItem && !session.Tutorial.HasSteps)
		{
			Singleton<BonusDropManager>.Instance.ResetMoves();
			GameBoard board = session.TurnLogic.Board;
			List<Tile> list = new List<Tile>();
			foreach (Tile item in board.GetOccupiedTiles())
			{
				if (item.Piece is NormalPiece && item.Piece.IsBasicPiece)
				{
					list.Add(item);
				}
			}
			if (list.Count > 0)
			{
				Tile random = list.GetRandom<Tile>();
				MatchFlag matchFlag = random.Piece.MatchFlag;
				bool activeSelf = random.Piece.gameObject.activeSelf;
				board.DespawnPieceAt(random);
				Piece piece = board.SpawnPieceAt(random.Index, PieceId.Create<BonusPiece>(matchFlag));
				piece.gameObject.SetActive(activeSelf);
			}
		}
	}

	public override void TurnCompleted(LevelSession session)
	{
		Singleton<BonusDropManager>.Instance.MarkMove();
	}
}
