using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorphModule : LogicModule
{
	public override void TurnCompleted(LevelSession session)
	{
		TurnLogic turnLogic = session.TurnLogic;
		List<Tile> list = new List<Tile>(turnLogic.Board.FindAllWithPieceClass<MorphPiece>());
		if (list.Count > 0)
		{
			SingletonAsset<SoundDatabase>.Instance.morphBubbleChange.Play();
		}
		List<IEnumerator> list2 = new List<IEnumerator>();
		foreach (Tile tile in list)
		{
			if (!this.cyclesForPieces.ContainsKey(tile.Index))
			{
				this.cyclesForPieces[tile.Index] = new List<MatchFlag>(session.GetSpawnColors(null));
				this.cyclesForPieces[tile.Index].Shuffle<MatchFlag>();
			}
			List<MatchFlag> list3 = this.cyclesForPieces[tile.Index];
			MatchFlag matchFlag = tile.Piece.MatchFlag;
			MatchFlag newMatchFlag = list3[(list3.IndexOf(matchFlag) + 1) % list3.Count];
			(tile.Piece as MorphPiece).UpdateMatchFlag(newMatchFlag);
			list2.Add(this.Animate(tile.Piece));
		}
		FiberCtrl.Pool.Run(FiberHelper.RunParallel(list2), false);
	}

	private IEnumerator Animate(Piece p)
	{
		yield return FiberAnimation.ScaleTransform(p.transform, Vector3.one * 1.5f, Vector3.one, null, 0.2f);
		yield break;
	}

	private Dictionary<int, List<MatchFlag>> cyclesForPieces = new Dictionary<int, List<MatchFlag>>();
}
