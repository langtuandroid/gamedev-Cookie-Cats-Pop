using System;
using System.Collections;
using System.Collections.Generic;
using TactileModules.FeatureManager;
using TactileModules.Foundation;
using TactileModules.PuzzleGames.EndlessChallenge;
using TactileModules.PuzzleGames.EndlessChallenge.Data;
using UnityEngine;

public class EndlessChallengeGameBoard : GameBoard
{
	public EndlessChallengeGameBoard(ILevelSession session) : base(session)
	{
		GameView gameView = ManagerRepository.Get<UIViewManager>().FindView<GameView>();
		this.pillarSize = gameView.GetElement().Size.x - gameView.gameHud.GetElement().Size.x;
		this.aspectRatioScale = (float)Screen.width / (float)Screen.height + 0.2f;
	}

	protected override bool ShouldSpawnNormalLevelTiles
	{
		get
		{
			return false;
		}
	}

	private EndlessChallengeHandler EndlessChallengeHandler
	{
		get
		{
			return FeatureManager.GetFeatureHandler<EndlessChallengeHandler>();
		}
	}

	public override void BuildLevel()
	{
		base.BuildLevel();
		this.SpawnEndlessBlocks();
	}

	public bool ShouldSpawnEndlessBlocks()
	{
		return base.Root.localPosition.y < 2000f;
	}

	public void SpawnEndlessBlocks()
	{
		List<PuzzleLevel.TileInfo> newTilesToAdd = this.GetNewTilesToAdd(this.session.Level.LevelAsset as EndlessLevel);
		this.MoveExistingPiecesDown(newTilesToAdd.Count);
		this.SpawnNewPiecesAboveExistingPieces(newTilesToAdd);
		this.UpdateCheckpointPositions();
	}

	private List<PuzzleLevel.TileInfo> GetNewTilesToAdd(EndlessLevel endlessLevel)
	{
		List<PuzzleLevel.TileInfo> list = new List<PuzzleLevel.TileInfo>();
		for (int i = 0; i < 1; i++)
		{
			List<EndlessLevelBlock> endlessLevelBlocksHard = endlessLevel.endlessLevelBlocksHard;
			endlessLevelBlocksHard.Shuffle<EndlessLevelBlock>();
			list.AddRange(endlessLevelBlocksHard[0].tiles);
			List<EndlessLevelBlock> endlessLevelBlocksMedium = endlessLevel.endlessLevelBlocksMedium;
			endlessLevelBlocksMedium.Shuffle<EndlessLevelBlock>();
			list.AddRange(endlessLevelBlocksMedium[0].tiles);
			List<EndlessLevelBlock> endlessLevelBlocksEasy = endlessLevel.endlessLevelBlocksEasy;
			endlessLevelBlocksEasy.Shuffle<EndlessLevelBlock>();
			for (int j = 0; j < 2; j++)
			{
				list.AddRange(endlessLevelBlocksEasy[j].tiles);
			}
		}
		return list;
	}

	private void MoveExistingPiecesDown(int tileIndexIncrement)
	{
		List<Piece> allPiecesOnBoard = base.GetAllPiecesOnBoard();
		foreach (Piece piece in allPiecesOnBoard)
		{
			piece.TileIndex += tileIndexIncrement;
			piece.AlignToTile();
		}
		base.Root.localPosition = base.CalculateScrollPosition();
	}

	private void SpawnNewPiecesAboveExistingPieces(List<PuzzleLevel.TileInfo> newTilesToAdd)
	{
		base.SetupPiecesFromLevel(newTilesToAdd);
	}

	private void UpdateCheckpointPositions()
	{
		foreach (EndlessCheckpoint endlessCheckpoint in this.endlessCheckPoints)
		{
			endlessCheckpoint.AlignGameObjectToRow();
		}
		this.TrySpawnEndlessCheckpoints();
	}

	private void TrySpawnEndlessCheckpoints()
	{
		List<EndlessChallengeCheckpointData> checkPointsData = this.GetCheckPointsData();
		if (checkPointsData == null || checkPointsData.Count == 0)
		{
			return;
		}
		for (int i = base.GetNumberOfRowsClearedByPlayer() + 1; i <= base.TotalRows; i++)
		{
			if (i == this.GetNumberOfRowsPositionIntervalWasLooped() + this.NextCheckpointToSpawn().PositionAtRow)
			{
				EndlessCheckpoint item = this.SpawnEndlessCheckpoint(i, this.NextCheckpointToSpawn());
				this.endlessCheckPoints.Add(item);
				this.positionIntervalCounter++;
			}
		}
	}

	private List<EndlessChallengeCheckpointData> GetCheckPointsData()
	{
		List<EndlessChallengeCheckpointData> result = null;
		if (this.session.Level.RootDatabase is EndlessChallengeLevelDatabase)
		{
			result = this.EndlessChallengeHandler.GetCheckPointsData();
		}
		return result;
	}

	private EndlessChallengeCheckpointData NextCheckpointToSpawn()
	{
		List<EndlessChallengeCheckpointData> checkPointsData = this.GetCheckPointsData();
		int index = this.positionIntervalCounter % checkPointsData.Count;
		return checkPointsData[index];
	}

	private int GetNumberOfRowsPositionIntervalWasLooped()
	{
		List<EndlessChallengeCheckpointData> checkPointsData = this.GetCheckPointsData();
		int positionAtRow = checkPointsData[checkPointsData.Count - 1].PositionAtRow;
		int num = Mathf.FloorToInt((float)this.positionIntervalCounter / (float)checkPointsData.Count);
		return positionAtRow * num;
	}

	private EndlessCheckpoint SpawnEndlessCheckpoint(int row, EndlessChallengeCheckpointData endlessChallengeCheckpointData)
	{
		EndlessCheckpoint endlessCheckpoint = UnityEngine.Object.Instantiate<EndlessCheckpoint>(SingletonAsset<EndlessChallengeSetup>.Instance.endlessCheckpoint, base.Root, true);
		endlessCheckpoint.Row = row;
		endlessCheckpoint.SetReward(endlessChallengeCheckpointData);
		endlessCheckpoint.GameBoard = this;
		endlessCheckpoint.HandleAspectRatioSizes(this.pillarSize, this.aspectRatioScale);
		endlessCheckpoint.transform.localScale = Vector3.one;
		endlessCheckpoint.transform.localPosition = new Vector3(endlessCheckpoint.transform.localPosition.x, endlessCheckpoint.transform.localPosition.y, 100f);
		endlessCheckpoint.gameObject.GetElement().Size = new Vector2(base.Size.x, 16f);
		endlessCheckpoint.gameObject.SetLayerRecursively(base.Root.gameObject.layer);
		endlessCheckpoint.AlignGameObjectToRow();
		return endlessCheckpoint;
	}

	public IEnumerator CollectAllCheckPoints(LevelSession levelSession)
	{
		List<EndlessCheckpoint> checkpointsToRemove = new List<EndlessCheckpoint>();
		foreach (EndlessCheckpoint checkPoint in this.endlessCheckPoints)
		{
			if (checkPoint.Row <= base.GetNumberOfRowsClearedByPlayer() && !checkPoint.HasCollectedReward)
			{
				yield return checkPoint.AnimateCollect(levelSession);
				checkpointsToRemove.Add(checkPoint);
				levelSession.EndlessChallengeStats.ReachedNewCheckPoint();
			}
		}
		foreach (EndlessCheckpoint endlessCheckpoint in checkpointsToRemove)
		{
			this.endlessCheckPoints.Remove(endlessCheckpoint);
			UnityEngine.Object.Destroy(endlessCheckpoint.gameObject);
		}
		yield break;
	}

	private const float ASPECT_RATIO_WIDTH_SCALE_ADDITION = 0.2f;

	private const int ENDLESS_CHALLENGE_SPAWN_THRESHOLD = 2000;

	private const int NUMBER_OF_ENDLESS_GROUPS_TO_SPAWN = 1;

	private readonly List<EndlessCheckpoint> endlessCheckPoints = new List<EndlessCheckpoint>();

	private readonly float pillarSize;

	private readonly float aspectRatioScale;

	private int positionIntervalCounter;
}
