using System;
using UnityEngine;

[SingletonAssetPath("Assets/[Database]/Resources/BossLevelDatabase.asset")]
public class BossLevelDatabase : SingletonAsset<BossLevelDatabase>
{
	public static BossLevelDatabase Database
	{
		get
		{
			return SingletonAsset<BossLevelDatabase>.Instance;
		}
	}

	[Header("Level")]
	public int maxRowsCount;

	public float delayBetweenStages;

	[Header("Boss Setup")]
	public BossVisuals bossVisuals;

	public float bossStageIntroSpeed;

	public float bossStageMoveSpeed;

	[Header("Boss Range/Size")]
	public float flyingPieceRange;

	public float bossDestroyPieceRange;

	[Header("Path Bubbles Popping")]
	public float pathBubblePopDelayMultiplier;

	[Header("Level Introduction")]
	public float levelIntroCameraSize;

	public float railingYOffset;

	public float allKittensSuckUpDuration;

	public float singleKittenSuckUpFullDuration;

	public float singleKittenSuckUpMoveDuration;

	public float singleKittenSuckUpFadeOutDelay;

	public float singleKittenSuckedUpScale;

	public float introKittenSurpriseDelay;

	public float introKittenSurpriseDelaySoundDelay;

	[Header("Stage Introduction")]
	public float spawnedBubblesStartScale;

	public float spawnedBubblesStartMoveDistance;

	public float spawnedBubbleMoveToPositionSpeed;

	public float spawnBubblesDuration;

	public float spawnBubblesDelay;

	[Header("Boss hit")]
	public float openDoorParticlesDelay;

	public float bossHitSpawnKittenDelay;

	[Header("Editor/Gizmo")]
	public bool useSolidColliderDisc;
}
