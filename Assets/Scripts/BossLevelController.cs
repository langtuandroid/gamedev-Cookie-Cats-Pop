using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Fibers;
using TactileModules.Foundation;
using UnityEngine;

public class BossLevelController
{
	public BossLevelController(LevelSession session)
	{
		this.session = session;
	}

	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<Vector3> OnBossHit = delegate (Vector3 A_0)
    {
    };



    public BossCharacterController BossCharacterController { get; private set; }

	private BossLevelGameBoard GameBoard
	{
		get
		{
			return this.gameView.Board as BossLevelGameBoard;
		}
	}

	public void Begin()
	{
		this.gameView = ManagerRepository.Get<UIViewManager>().FindView<GameView>();
		this.gameView.cannon.InputEnabled += false;
		this.gameView.SetPanningPosToTargetPosition();
		this.checkpoints = new BossCheckpoints(this.GameBoard);
		this.BossCharacterController = new BossCharacterController(this.GameBoard, this.gameView.bossLevelAreas, this.session, this.checkpoints);
		this.bossCameraController = new BossCameraController(this.gameView.ViewCamera, this.gameView.TopOfShipRailingPosition);
		this.bossCannonController = new BossCannonController(this.gameView.cannonOperator, this.gameView.cannonBallQueue);
		this.bossKittenController = new BossKittenController(this.BossCharacterController, this.GameBoard, this.gameView.bossLevelAreas);
		this.BossCharacterController.OnHitBoss += this.HitBoss;
		AudioManager.Instance.SetMusic(SingletonAsset<SoundDatabase>.Instance.bossMusic, true);
	}

	public void End()
	{
		this.BossCharacterController.End();
	}

	public IEnumerator LevelIntroduction()
	{
		this.gameView.cannon.InputEnabled += true;
		this.gameView.gameHud.BoosterBar.Show();
		this.introFiber.Start(FiberHelper.RunSerial(new IEnumerator[]
		{
			this.IntroMoveBossToSuckPosition(),
			this.IntroSuckUpKittens(),
			this.IntroZoomAndMovement()
		}));
		while (!this.introFiber.IsTerminated)
		{
			yield return null;
		}
		yield return this.StageIntroduction();
		this.BossCharacterController.SetBossState(BossState.ACTIVE);
		yield break;
	}

	private IEnumerator IntroMoveBossToSuckPosition()
	{
		this.bossNoisesSource = SingletonAsset<SoundDatabase>.Instance.bossNoisesLoop.Play();
		yield return FiberHelper.RunParallel(new IEnumerator[]
		{
			this.BossCharacterController.GetPrimaryIntroAnimation(),
			this.bossKittenController.GetKittensSurprisedAnimation()
		});
		yield break;
	}

	private IEnumerator IntroSuckUpKittens()
	{
		SingletonAsset<SoundDatabase>.Instance.bossSuction.Play();
		IEnumerator playDoorCloseAnim = FiberHelper.RunDelayed(1.9f, delegate
		{
			SingletonAsset<SoundDatabase>.Instance.bossDoorClose.Play();
		});
		IEnumerator laughAnim = FiberHelper.RunDelayed(0.15f, delegate
		{
			SingletonAsset<SoundDatabase>.Instance.bossEvilLaugh.Play();
		});
		yield return FiberHelper.RunParallel(new IEnumerator[]
		{
			this.bossKittenController.GetSuckUpKittensAnimation(),
			this.BossCharacterController.Visuals.GetIntroSuckUpKittensAnimation(),
			playDoorCloseAnim,
			laughAnim
		});
		this.BossCharacterController.Visuals.EnableMovementAnimation(true);
		yield break;
	}

	private IEnumerator IntroZoomAndMovement()
	{
		yield return FiberHelper.RunParallel(new IEnumerator[]
		{
			this.BossCharacterController.GetIntroZoomAndMovementAnimation(),
			this.bossCannonController.GetIntroZoomAndMovementAnimation(),
			this.bossCameraController.GetIntroZoomAndMovementAnimation()
		});
		yield break;
	}

	private IEnumerator StageIntroduction()
	{
		this.GameBoard.SpawnNextStage();
		this.session.TurnLogic.Initialize();
		this.checkpoints.ProcessStageCheckpoints();
		yield return this.BossCharacterController.MoveBossToNextCheckpoint(BossLevelDatabase.Database.bossStageIntroSpeed);
		yield return this.SpawnStagePieces();
		this.BossCharacterController.StartStage();
		yield return this.CheckForDelayedOutOfMoves();
		GameEventManager.Instance.Emit(62, null, 1);
		yield break;
	}

	private IEnumerator CheckForDelayedOutOfMoves()
	{
		int remainingStages = this.session.GetGoalPiecesRemaining();
		if (remainingStages <= 0 || this.session.BallQueue.BallsLeft > 0)
		{
			yield break;
		}
		this.session.SetState(LevelSessionState.NoMoreMoves);
		UICamera.EnableInput();
		while (this.session.SessionState != LevelSessionState.Playing)
		{
			yield return null;
		}
		UICamera.DisableInput();
		yield break;
	}

	private IEnumerator SpawnStagePieces()
	{
		SingletonAsset<SoundDatabase>.Instance.bossBlowingWind.Play();
		List<IEnumerator> spawnAnimations = new List<IEnumerator>();
		spawnAnimations.Add(this.BossCharacterController.Visuals.PlayBubblesAnimation());
		List<Piece> pieces = this.GameBoard.GetSortedBoardPieces(this.BossCharacterController.BossTransform.position);
		float delay = BossLevelDatabase.Database.spawnBubblesDelay;
		IEnumerator spawnBubbleParticlesAnim = FiberHelper.RunDelayed(delay, delegate
		{
			this.BossCharacterController.Visuals.EnableBubbleParticles(true);
		});
		spawnAnimations.Add(spawnBubbleParticlesAnim);
		float pieceSpawnInterval = BossLevelDatabase.Database.spawnBubblesDuration / (float)pieces.Count;
		foreach (Piece piece in pieces)
		{
			IEnumerator item = this.MovePieceToPosition(piece, delay);
			spawnAnimations.Add(item);
			delay += pieceSpawnInterval;
		}
		yield return FiberHelper.RunParallel(spawnAnimations);
		SingletonAsset<SoundDatabase>.Instance.bossBlowingSingleBubble.ResetSequential();
		yield break;
	}

	private void HitBoss()
	{
		if (!this.hitBossFiber.IsTerminated)
		{
		}
		this.BossCharacterController.SetBossState(BossState.INACTIVE);
		this.GameBoard.CurrentStageCompleted();
		int bossDestructionLevel = this.GameBoard.GetBossDestructionLevel();
		this.BossCharacterController.Visuals.SetDestructionLevel(bossDestructionLevel);
		this.hitBossFiber.Start(this.HitBossRoutine());
	}

	private IEnumerator HitBossRoutine()
	{
		UICamera.DisableInput();
		if (BossLevelController._003C_003Ef__mg_0024cache0 == null)
		{
			BossLevelController._003C_003Ef__mg_0024cache0 = new Fiber.OnExitHandler(UICamera.EnableInput);
		}
		yield return new Fiber.OnExit(BossLevelController._003C_003Ef__mg_0024cache0);
		yield return this.StageCompletedAnimation();
		while (!this.session.TurnLogic.IsFibersTerminated)
		{
			yield return null;
		}
		this.session.TurnLogic.Destroy();
		yield return this.StageCompleted();
		yield break;
	}

	private IEnumerator StageCompletedAnimation()
	{
		if (this.GameBoard.IsLastStage())
		{
			this.bossNoisesSource.Stop();
			SingletonAsset<SoundDatabase>.Instance.bossDestroyed.Play();
		}
		else
		{
			SingletonAsset<SoundDatabase>.Instance.bossHit.Play();
		}
		IEnumerator bossHitAnim = (!this.GameBoard.IsLastStage()) ? this.BossCharacterController.Visuals.PlayHitAnimation() : this.BossCharacterController.Visuals.PlayDieAnimation();
		IEnumerator delayedSpawnKittenAnim = FiberHelper.RunDelayed(BossLevelDatabase.Database.bossHitSpawnKittenDelay, delegate
		{
			this.OnBossHit(this.BossCharacterController.Visuals.KittenSpawnPos);
			SingletonAsset<SoundDatabase>.Instance.bossKittenYipee.Play();
		});
		IEnumerator delayedBubblesAnim = FiberHelper.RunDelayed(BossLevelDatabase.Database.openDoorParticlesDelay, delegate
		{
			this.BossCharacterController.Visuals.PlayOpenDoorParticles();
		});
		IEnumerator detachPiecesAnim = FiberHelper.RunSerial(new IEnumerator[]
		{
			FiberHelper.Wait(1.5f, (FiberHelper.WaitFlag)0),
			AfterMath.DetachRemainingPieces(this.session)
		});
		yield return FiberHelper.RunParallel(new IEnumerator[]
		{
			bossHitAnim,
			delayedSpawnKittenAnim,
			delayedBubblesAnim,
			detachPiecesAnim
		});
		yield break;
	}

	private IEnumerator StageCompleted()
	{
		if (this.GameBoard.IsLastStage())
		{
			yield return this.LevelCompleted();
		}
		else
		{
			yield return FiberHelper.Wait(BossLevelDatabase.Database.delayBetweenStages, (FiberHelper.WaitFlag)0);
			yield return this.StageIntroduction();
			this.BossCharacterController.SetBossState(BossState.ACTIVE);
		}
		yield break;
	}

	private IEnumerator LevelCompleted()
	{
		SingletonAsset<SoundDatabase>.Instance.victoriousMotif.Play();
		AudioManager.Instance.SetMusic(null, true);
		this.session.SetState(LevelSessionState.ReadyForAftermath);
		yield break;
	}

	private IEnumerator MovePieceToPosition(Piece piece, float delay)
	{
		yield return FiberHelper.Wait(delay, (FiberHelper.WaitFlag)0);
		SingletonAsset<SoundDatabase>.Instance.bossBlowingSingleBubble.PlaySequential();
		piece.gameObject.SetActive(true);
		piece.GetComponent<ZSorter>().enabled = false;
		Vector3 fromPos = this.BossCharacterController.Visuals.SpawnBubblesPos;
		Vector3 toPos = piece.transform.position;
		Vector3 fromScale = Vector3.one * BossLevelDatabase.Database.spawnedBubblesStartScale;
		Vector3 toScale = piece.transform.localScale;
		float distance = Vector2.Distance(fromPos, toPos);
		float duration = distance / BossLevelDatabase.Database.spawnedBubbleMoveToPositionSpeed;
		piece.transform.position = Vector3.MoveTowards(fromPos, toPos, BossLevelDatabase.Database.spawnedBubblesStartMoveDistance);
		IEnumerator moveAnim = FiberAnimation.MoveTransform(piece.transform, piece.transform.position, toPos, null, duration);
		IEnumerator scaleAnim = FiberAnimation.ScaleTransform(piece.transform, fromScale, toScale, null, duration);
		yield return FiberHelper.RunParallel(new IEnumerator[]
		{
			moveAnim,
			scaleAnim
		});
		piece.GetComponent<ZSorter>().enabled = true;
		yield break;
	}

	private readonly Fiber introFiber = new Fiber();

	private readonly Fiber hitBossFiber = new Fiber();

	private readonly LevelSession session;

	private GameView gameView;

	private BossCheckpoints checkpoints;

	private BossCameraController bossCameraController;

	private BossCannonController bossCannonController;

	private BossKittenController bossKittenController;

	private AudioSource bossNoisesSource;

	[CompilerGenerated]
	private static Fiber.OnExitHandler _003C_003Ef__mg_0024cache0;
}
