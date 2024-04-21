using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class PuzzleLevel : LevelAssetBase
{
	public abstract IEnumerable<ITutorialStep> TutorialSteps { get; }

	[HideInInspector]
	[Hashable(null)]
	public List<PuzzleLevel.TileInfo> tiles = new List<PuzzleLevel.TileInfo>();

	[HideInInspector]
	[Hashable(null)]
	public int moves;

	[HideInInspector]
	[Hashable(null)]
	public PuzzleLevel.HardLevelParameters hardLevelParameters;

	[HideInInspector]
	[Hashable(null)]
	public int[] starThresholds;

	[HideInInspector]
	[Hashable(null)]
	public List<PuzzleLevel.SpawnInfo> spawnInfos;

	[Serializable]
	public class HardLevelParameters
	{
		[Hashable(null)]
		public int moves;

		[Hashable(null)]
		public int overrideCharge;

		[Hashable(null)]
		public int[] starThresholds = new int[]
		{
			500,
			1000,
			2000
		};

		[Hashable(null)]
		public List<MatchFlag> enabledPowerupColors = new List<MatchFlag>();
	}

	[Serializable]
	public class TileInfo
	{
		public virtual void CopyFrom(PuzzleLevel.TileInfo source)
		{
			this.piece = source.piece;
			this.attachments = new List<PuzzleLevel.TileElementInfo>();
			foreach (PuzzleLevel.TileElementInfo item in source.attachments)
			{
				this.attachments.Add(item);
			}
		}

		[Hashable(null)]
		public PuzzleLevel.TileElementInfo piece;

		[Hashable(null)]
		public List<PuzzleLevel.TileElementInfo> attachments = new List<PuzzleLevel.TileElementInfo>();
	}

	[Serializable]
	public struct TileElementInfo
	{
		[Hashable(null)]
		public PieceId id;

		[Hashable(null)]
		public int amount;

		[Hashable(null)]
		public int direction;
	}

	[Serializable]
	public class SpawnInfo
	{
		[Hashable(null)]
		public PieceId id;

		[Hashable(null)]
		public float chance = 1f;

		[Hashable(null)]
		public int maxObjects;
	}
}
