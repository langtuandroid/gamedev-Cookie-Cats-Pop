using System;
using System.Collections.Generic;
using UnityEngine;

public class BossLevelGameBoard : GameBoard
{
	public BossLevelGameBoard(ILevelSession session) : base(session)
	{
		this.bossStageIdx = -1;
		this.upcomingStageIdx = 0;
	}

	public BossLevelStage CurrentStage { get; private set; }

	private BossLevel BossLevel
	{
		get
		{
			return (BossLevel)this.session.Level.LevelAsset;
		}
	}

	public int BossStagesLeft
	{
		get
		{
			return this.BossLevel.bossLevelStages.Count - this.upcomingStageIdx;
		}
	}

	public bool IsChangingStages
	{
		get
		{
			return this.upcomingStageIdx != this.bossStageIdx;
		}
	}

	protected override bool ShouldAddTopLine
	{
		get
		{
			return false;
		}
	}

	protected override bool ShouldSpawnNormalLevelTiles
	{
		get
		{
			return false;
		}
	}

	public void SpawnNextStage()
	{
		this.bossStageIdx = this.upcomingStageIdx;
		this.CurrentStage = this.BossLevel.bossLevelStages[this.bossStageIdx];
		base.SetupPiecesFromLevel(this.CurrentStage.tiles);
		List<Piece> allPiecesOnBoard = base.GetAllPiecesOnBoard();
		foreach (Piece piece in allPiecesOnBoard)
		{
			piece.gameObject.SetActive(false);
		}
	}

	private bool IsPreparingLevel()
	{
		return this.bossStageIdx == -1;
	}

	public bool IsLastStage()
	{
		return this.bossStageIdx == this.BossLevel.bossLevelStages.Count - 1;
	}

	public void CurrentStageCompleted()
	{
		this.upcomingStageIdx = this.bossStageIdx + 1;
	}

	private List<MatchFlag> GetFirstStageColors()
	{
		List<MatchFlag> list = new List<MatchFlag>();
		BossLevelStage bossLevelStage = this.BossLevel.bossLevelStages[0];
		foreach (PuzzleLevel.TileInfo tileInfo in bossLevelStage.tiles)
		{
			PieceInfo piece = SingletonAsset<PieceDatabase>.Instance.GetPiece(tileInfo.piece.id);
			if (piece != null && !(piece.gamePrefab == null))
			{
				MatchFlag matchFlag = piece.gamePrefab.MatchFlag;
				if (matchFlag != string.Empty && !list.Contains(matchFlag))
				{
					list.Add(matchFlag);
				}
			}
		}
		return list;
	}

	public int GetBossDestructionLevel()
	{
		float f = (float)(4 * this.upcomingStageIdx) / (float)this.BossLevel.bossLevelStages.Count;
		return Mathf.Clamp(Mathf.RoundToInt(f), 0, 3);
	}

	public List<Piece> GetSortedBoardPieces(Vector2 bossPos)
	{
		List<Piece> allPiecesOnBoard = base.GetAllPiecesOnBoard();
		allPiecesOnBoard.Sort(delegate(Piece p1, Piece p2)
		{
			float num = Vector2.Distance(bossPos, p1.transform.position);
			float num2 = Vector2.Distance(bossPos, p2.transform.position);
			if (num < num2)
			{
				return 1;
			}
			if (num > num2)
			{
				return -1;
			}
			return 0;
		});
		return allPiecesOnBoard;
	}

	public override List<MatchFlag> GetColorsAmongPieces(bool withinBoundsOnly)
	{
		List<MatchFlag> list = new List<MatchFlag>();
		if (this.IsPreparingLevel())
		{
			foreach (MatchFlag item in this.GetFirstStageColors())
			{
				list.Add(item);
			}
		}
		else
		{
			list = base.GetColorsAmongPieces(withinBoundsOnly);
		}
		return list;
	}

	public override bool TileIndexIsAtTop(int tileIndex)
	{
		return false;
	}

	protected override void CalculateTrimmedTop()
	{
	}

	private int bossStageIdx;

	private int upcomingStageIdx;
}
