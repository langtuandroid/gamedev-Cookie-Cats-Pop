using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Fibers;
using UnityEngine;

public class BossCharacterController
{
	public BossCharacterController(BossLevelGameBoard gameBoard, BossLevelAreas bossLevelAreas, LevelSession session, BossCheckpoints checkpoints)
	{
		this.gameBoard = gameBoard;
		this.bossLevelAreas = bossLevelAreas;
		this.session = session;
		this.checkpoints = checkpoints;
		this.session.TurnLogic.ShotMovementStarted += this.ShotStarted;
		this.session.TurnLogic.ShotMovementEnded += this.ShotEnded;
		this.session.TurnLogic.ShotFired += this.ShotFired;
		this.session.TurnLogic.TurnCompleted += this.TurnCompleted;
		this.session.Tutorial.ChangedStep += this.HandleTutorialChangedStep;
		this.SpawnBoss();
	}

	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action OnHitBoss = delegate ()
    {
    };



    public Transform BossTransform { get; private set; }

	public BossVisuals Visuals { get; private set; }

	public bool IsBossHitByPower { get; private set; }

	private BossState BossState { get; set; }

	public void End()
	{
		this.session.TurnLogic.ShotMovementStarted -= this.ShotStarted;
		this.session.TurnLogic.ShotMovementEnded -= this.ShotEnded;
		this.session.TurnLogic.ShotFired -= this.ShotFired;
		this.session.TurnLogic.TurnCompleted -= this.TurnCompleted;
		this.session.Tutorial.ChangedStep -= this.HandleTutorialChangedStep;
		this.mainFiberRunner.Terminate();
	}

	private void SpawnBoss()
	{
		this.Visuals = UnityEngine.Object.Instantiate<BossVisuals>(BossLevelDatabase.Database.bossVisuals, this.gameBoard.Root);
		this.Visuals.gameObject.SetLayerRecursively(this.gameBoard.Root.gameObject.layer);
		this.BossTransform = this.Visuals.transform;
		this.originalBossScale = this.BossTransform.localScale;
		this.BossTransform.position = this.bossLevelAreas.IntroStartPos;
		this.BossTransform.localScale = Vector3.one * 24f;
		this.SetBossState(BossState.INACTIVE);
		this.mainFiberRunner.Run(this.BossLogic(), false);
	}

	public void SetBossState(BossState state)
	{
		this.BossState = state;
	}

	private void HitByPower(float timeDelay)
	{
		if (this.IsBossHitByPower)
		{
			return;
		}
		this.IsBossHitByPower = true;
		this.bossHitByPowerTime = Time.time + timeDelay;
	}

	public IEnumerator GetPrimaryIntroAnimation()
	{
		return FiberAnimation.Animate(3f, AnimationCurve.EaseInOut(0f, 0f, 1f, 1f), delegate(float t)
		{
			Vector3 introStartPos = this.bossLevelAreas.IntroStartPos;
			Vector3 introSuckKittensPos = this.bossLevelAreas.IntroSuckKittensPos;
			this.BossTransform.position = Vector3.Lerp(introStartPos, introSuckKittensPos, t);
		}, false);
	}

	public IEnumerator GetIntroZoomAndMovementAnimation()
	{
		IEnumerator enumerator = FiberAnimation.Animate(1.5f, AnimationCurve.EaseInOut(0f, 0f, 1f, 1f), delegate(float t)
		{
			Vector3 introSuckKittensPos = this.bossLevelAreas.IntroSuckKittensPos;
			Vector3 introCompletePos = this.bossLevelAreas.IntroCompletePos;
			this.BossTransform.position = Vector3.Lerp(introSuckKittensPos, introCompletePos, t);
		}, false);
		Vector3 startScale = this.BossTransform.localScale;
		IEnumerator enumerator2 = FiberAnimation.Animate(2.5f, AnimationCurve.EaseInOut(0f, 0f, 1f, 1f), delegate(float t)
		{
			Vector3 localScale = Vector3.LerpUnclamped(startScale, this.originalBossScale, t);
			this.BossTransform.localScale = localScale;
		}, false);
		return FiberHelper.RunParallel(new IEnumerator[]
		{
			enumerator,
			enumerator2
		});
	}

	public void StartStage()
	{
		this.movementFiber.Start(this.BossMovement());
		this.Visuals.EnableBubbleParticles(false);
	}

	private void ShotStarted(CPPiece piece, Action<bool> hitAction)
	{
		this.destroyFlyingPieceActions.Add(piece, hitAction);
	}

	private void ShotEnded(CPPiece piece)
	{
		this.destroyFlyingPieceActions.Remove(piece);
	}

	private void ShotFired(TurnLogic.Shot[] shots, ResolveState resolveState)
	{
		this.resolveState = resolveState;
		this.resolveState.AllMarkedByPowerTilesApplied += this.AllMarkedByPowerTilesApplied;
	}

	private void TurnCompleted()
	{
		this.resolveState.AllMarkedByPowerTilesApplied -= this.AllMarkedByPowerTilesApplied;
		this.resolveState = null;
		if (this.BossState == BossState.RESOLVING_POWER_HITS)
		{
			this.SetBossState(BossState.ACTIVE);
		}
	}

	private void HandleTutorialChangedStep(ITutorialStep step)
	{
		TutorialStep tutorialStep = step as TutorialStep;
		if (tutorialStep != null && tutorialStep.highlightBoss && tutorialStep.dismissType != TutorialStep.DismissType.BossNewStage)
		{
			this.SetBossState(BossState.INACTIVE);
		}
	}

	private void AllMarkedByPowerTilesApplied(List<TileHitMark> markedByPowerTiles)
	{
		if (markedByPowerTiles.Count == 0)
		{
			return;
		}
		this.SetBossState(BossState.RESOLVING_POWER_HITS);
		foreach (TileHitMark tileHitMark in markedByPowerTiles)
		{
			float num = Vector2.Distance(tileHitMark.tile.LocalPosition, this.BossTransform.localPosition);
			if (num < BossLevelDatabase.Database.flyingPieceRange)
			{
				this.HitByPower(tileHitMark.time);
				break;
			}
		}
	}

	private IEnumerator BossLogic()
	{
		for (;;)
		{
			BossState bossState = this.BossState;
			if (bossState != BossState.INACTIVE)
			{
				if (bossState != BossState.RESOLVING_POWER_HITS)
				{
					if (bossState == BossState.ACTIVE)
					{
						this.movementFiber.Step();
						this.CheckBoardPieceCollision();
						this.CheckFlyingPieceCollision();
					}
				}
				else if (this.IsBossHitByPower && Time.time > this.bossHitByPowerTime)
				{
					this.IsBossHitByPower = false;
					this.HitBoss();
				}
			}
			yield return null;
		}
		yield break;
	}

	private void CheckBoardPieceCollision()
	{
		bool flag = false;
		List<CPPiece> piecesInRange = this.session.TurnLogic.Board.GetPiecesInRange(this.BossTransform.localPosition, BossLevelDatabase.Database.bossDestroyPieceRange);
		foreach (CPPiece cppiece in piecesInRange)
		{
			float num = Vector2.Distance(cppiece.GetTile().LocalPosition, this.BossTransform.localPosition);
			if (num < BossLevelDatabase.Database.bossDestroyPieceRange)
			{
				flag = true;
				this.mainFiberRunner.Run(this.PopPathBubble(cppiece, 0f), false);
			}
		}
		if (!flag)
		{
			return;
		}
		List<int> unattachedClouds = Clusters.FindUnattachedClouds(this.session.TurnLogic.Board);
		List<int> list = Clusters.FindUnattachedTilesSemiOptimized(this.session.TurnLogic.Board, unattachedClouds);
		foreach (int index in list)
		{
			Tile tile = this.session.TurnLogic.Board.GetTile(index);
			if (!(tile.Piece == null))
			{
				float num2 = Vector3.Distance(tile.LocalPosition, this.BossTransform.localPosition);
				float animationDelay = num2 * BossLevelDatabase.Database.pathBubblePopDelayMultiplier;
				this.mainFiberRunner.Run(this.PopPathBubble((CPPiece)tile.Piece, animationDelay), false);
			}
		}
	}

	private void CheckFlyingPieceCollision()
	{
		foreach (KeyValuePair<Piece, Action<bool>> keyValuePair in this.destroyFlyingPieceActions)
		{
			Piece key = keyValuePair.Key;
			Action<bool> value = keyValuePair.Value;
			float num = Vector2.Distance(key.transform.localPosition, this.BossTransform.localPosition);
			if (num < BossLevelDatabase.Database.flyingPieceRange)
			{
				value(true);
				this.HitBoss();
				break;
			}
		}
	}

	private void HitBoss()
	{
		this.movementFiber.Terminate();
		this.OnHitBoss();
	}

	private IEnumerator PopPathBubble(CPPiece piece, float animationDelay)
	{
		bool hasResolveState = this.resolveState != null;
		bool isPieceMarkedForRemoval = hasResolveState && this.resolveState.IsPieceMarkedForRemoval(piece);
		if (isPieceMarkedForRemoval)
		{
			yield break;
		}
		List<Piece> pieces = new List<Piece>(piece.GetTile().Pieces);
		foreach (Piece piece2 in pieces)
		{
			if (!(piece2 is SquidPiece))
			{
				piece2.TileIndex = -1;
			}
		}
		yield return FiberHelper.Wait(animationDelay, (FiberHelper.WaitFlag)0);
		yield return piece.AnimatePop();
		foreach (Piece piece3 in pieces)
		{
			if (piece3 is SquidPiece)
			{
				this.session.TurnLogic.BossPoppedSquid(piece3 as SquidPiece);
			}
			else
			{
				this.session.TurnLogic.Board.DespawnPiece(piece3);
			}
		}
		yield break;
	}

	private IEnumerator BossMovement()
	{
		for (;;)
		{
			yield return this.MoveBossToNextCheckpoint(BossLevelDatabase.Database.bossStageMoveSpeed);
		}
		yield break;
	}

	public IEnumerator MoveBossToNextCheckpoint(float speed)
	{
		Vector3 bossPos = this.BossTransform.localPosition;
		Vector3 checkpointPosition = this.checkpoints.GetNextCheckpointPosition();
		checkpointPosition.z = bossPos.z;
		float distance = Vector2.Distance(bossPos, checkpointPosition);
		float duration = distance / speed;
		yield return FiberAnimation.MoveLocalTransform(this.BossTransform, bossPos, checkpointPosition, null, duration);
		yield break;
	}

	private readonly BossLevelGameBoard gameBoard;

	private readonly BossLevelAreas bossLevelAreas;

	private readonly LevelSession session;

	private readonly BossCheckpoints checkpoints;

	private readonly Fiber movementFiber = new Fiber(FiberBucket.Manual);

	private readonly FiberRunner mainFiberRunner = new FiberRunner();

	private readonly Dictionary<Piece, Action<bool>> destroyFlyingPieceActions = new Dictionary<Piece, Action<bool>>();

	private Vector3 originalBossScale;

	private float bossHitByPowerTime;

	private ResolveState resolveState;
}
