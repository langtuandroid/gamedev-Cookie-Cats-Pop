using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelAsset : Level
{
	public virtual string LevelType
	{
		get
		{
			return base.GetType().ToString();
		}
	}

	public int NumTilesX
	{
		get
		{
			return 12;
		}
	}

	public bool IsTutorial
	{
		get
		{
			return this.tutorialSteps != null && this.tutorialSteps.Count > 0;
		}
	}

	public override IEnumerable<ITutorialStep> TutorialSteps
	{
		get
		{
			return this.tutorialSteps.ConvertAll<ITutorialStep>((TutorialStep s) => s);
		}
	}

	public const string RANDOM_COLOR = "?";

	public static readonly string[] RANDOM_GROUPS = new string[]
	{
		"?0",
		"?1",
		"?2",
		"?3",
		"?4"
	};

	[HideInInspector]
	public List<TutorialStep> tutorialSteps;

	[HideInInspector]
	public List<PieceId> firstPieces;

	[HideInInspector]
	public bool spawnExistingColorsOnly;

	[Hashable(0)]
	[HideInInspector]
	public int amountOfOneTurnBoosters;

	[Hashable(false)]
	[HideInInspector]
	public bool enableOneTurnBoosterShield;

	[Hashable(false)]
	[HideInInspector]
	public bool enableOneTurnBoosterSuperaim;

	[Hashable(false)]
	[HideInInspector]
	public bool enableOneTurnBoosterSuperqueue;

	[HideInInspector]
	public List<MatchFlag> enabledPowerupColors;

	[HideInInspector]
	public int overrideChargeAmount;

	public class MetaData : PuzzleLevelMetaData
	{
	}
}
