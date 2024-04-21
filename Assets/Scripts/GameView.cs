using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using Tactile;
using TactileModules.FeatureManager;
using TactileModules.Foundation;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGame.HotStreak;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGame.PiggyBank;
using TactileModules.PuzzleGame.PiggyBank.Controllers;
using TactileModules.PuzzleGame.PiggyBank.UI;
using TactileModules.PuzzleGame.ScheduledBooster;
using TactileModules.PuzzleGame.ScheduledBooster.Data;
using TactileModules.PuzzleGame.ScheduledBooster.Model;
using UnityEngine;

public class GameView : UIView
{
    static Dictionary<string, int> _003C_003Ef__switch_0024map0;
	public bool IsBossAnimating { get; set; }

	public GameBoard Board { get; private set; }

	public Vector3 TopOfShipRailingPosition
	{
		get
		{
			return this.topOfShipRailing.position;
		}
	}

	private ConfigurationManager ConfigurationManager
	{
		get
		{
			return ManagerRepository.Get<ConfigurationManager>();
		}
	}

	private ScheduledBoosters ScheduledBoosters
	{
		get
		{
			return ManagerRepository.Get<ScheduledBoosterSystem>().ScheduledBoosters;
		}
	}

	protected override void ViewLoad(object[] parameters)
	{
		this.levelSession = (parameters[0] as LevelSession);
		this.levelSession.StateChanged += this.HandleSessionStateChanged;
		this.levelSession.InGameBoosterUsed += this.TriggerPiggyBankInGameBoosterUsed;
		AudioManager.Instance.SetMusic(SingletonAsset<SoundDatabase>.Instance.ingameMusic, true);
		this.piggyBankStateController = ManagerRepository.Get<PiggyBankSystem>().ControllerFactory.CreateStateController();
		this.HandlePiggyBank();
	}

	protected override void ViewWillAppear()
	{
		if (this.levelSession.Level.LevelAsset is EndlessLevel)
		{
			this.Board = new EndlessChallengeGameBoard(this.levelSession);
		}
		else if (this.levelSession.Level.LevelAsset is BossLevel)
		{
			this.Board = new BossLevelGameBoard(this.levelSession);
		}
		else
		{
			this.Board = new GameBoard(this.levelSession);
		}
		PowerResolvementHelper.Layer = base.gameObject.layer;
		this.Board.InitializePool(base.transform);
		this.levelSession.Initialize(this.Board, this.cannon, this.boardPortraitBounds.transform);
		this.Board.FitToEnclosingElement(this.boardPortraitBounds);
		this.gameHud.Initialize(this.levelSession);
        this.savedKittens.Initialize(this.levelSession);
        this.powerArea.Initialize(this.levelSession);
        this.tracerDots.Initialize(this.Board.Root);
        PieceId rainbowPieceId = PieceId.Create<RainbowPiece>(string.Empty);
        this.tracerDots.ColorRetriever = delegate (float distance)
        {
            PieceId nextPieceToShoot = this.levelSession.GetNextPieceToShoot();
            if (nextPieceToShoot == rainbowPieceId)
            {
                return Color.HSVToRGB(Mathf.Repeat((distance - Time.time * 0.0005f) * 0.005f, 1f), 0.75f, 1f);
            }
            return SingletonAsset<LevelVisuals>.Instance.GetAimColorFromMatchColor(nextPieceToShoot.MatchFlag);
        };
        this.cannonOperator.Initialize(this.levelSession);
        this.cannonBallQueue.Initialize(this.levelSession);
        this.boardCamera.Initialize(this.levelSession.TurnLogic);
        this.Board.allowedToPanFunction = (() => !this.boardCamera.IsAnimating);
        this.gameHud.BoosterBar.BoosterActivated += this.BoosterActivated;
        this.BuySpecialBooster.BoosterActivated += this.BoosterActivated;
        this.levelSession.TurnLogic.ResolveCompleted += this.HandleResolveCompleted;
        this.levelSession.TurnLogic.PieceCleared += this.HandlePieceCleared;
        this.levelSession.TurnLogic.PieceDiscarded += this.HandlePieceDiscarded;
        this.levelSession.Tutorial.ChangedStep += this.HandleTutorialChangedStep;
        this.SetPanningPosToLevelTop();
        this.gameHud.BoosterBar.Hide();
        this.shield.Initialize(this.levelSession);
        this.BuySpecialBooster.Initialize(this.levelSession);
        SingletonAsset<SoundDatabase>.Instance.poppedByPower.ResetSequential();
        SingletonAsset<SoundDatabase>.Instance.bubblePop.ResetSequential();
        UserCareManager.Instance.StartBoosterCare();
        AchievementsLevelSessionListener achievementsLevelSessionListener = new AchievementsLevelSessionListener();
        achievementsLevelSessionListener.Initialize(this.levelSession);
        this.skyBackground = this.backgroundInstantiator.GetInstance<SkyBackground>();
        this.skyBackground.Initialize(this.topOfShipRailing);
        Floaters.FloorWorldPositionFunction = (() => this.panningRoot.TransformPoint(new Vector3(0f, -360f, 0f)));
        this.effectsPanningRoot = new GameObject("Effects").transform;
        this.effectsPanningRoot.SetParent(this.Board.Root, true);
        EffectPool instance = EffectPool.Instance;
        instance.EffectSpawned = (Action<string, SpawnedEffect>)Delegate.Combine(instance.EffectSpawned, new Action<string, SpawnedEffect>(this.EffectPool_EffectSpawned));
        PowerResolvementHelper.panningRoot = this.effectsPanningRoot;
        if (BoosterManagerBase<BoosterManager>.Instance.ShouldUseSuperAimMonocular)
        {
            this.levelSession.Cannon.SuperAimActivated += delegate ()
            {
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.superAimMonocularPrefab);
                gameObject.SetLayerRecursively(this.gameObject.layer);
                gameObject.transform.parent = this.transform;
                gameObject.transform.localPosition = Vector3.zero;
                this.superAimMonocularInitializer = gameObject.GetComponent<SuperAimMonocular>();
                this.superAimMonocularInitializer.Initialize(this.levelSession, this.Board.Root, this.boardPortraitBounds, this.gameObject.layer);
            };
        }
        this.levelSession.Cannon.SuperAimDeactivated += delegate ()
        {
            if (this.superAimMonocularInitializer != null)
            {
                this.superAimMonocularInitializer.EndAndDestroySuperAim();
            }
        };
        if (this.levelSession.Level.LevelAsset is EndlessLevel)
        {
            this.savedKittens.Disable();
        }
        if (this.levelSession.Level.LevelAsset is BossLevel)
        {
            this.levelSession.BossLevelController.Begin();
        }
    }

	protected override void ViewDidAppear()
	{
		bool moveCameraInstantly = this.levelSession.Level.LevelAsset is EndlessLevel || this.levelSession.Level.LevelAsset is BossLevel;
		this.fiberRunner.Run(this.StartupSequence(moveCameraInstantly), false);
	}

	protected override void ViewDidDisappear()
	{
		EffectPool.Instance.TerminateAllRunningEffects();
		AudioManager.Instance.SetMusic(null, true);
		this.shield.Destroy();
		this.levelSession.Destroy();
		this.cannonBallQueue.Destroy();
		UserCareManager.Instance.StopBoosterCare();
		UserCareManager.Instance.ClearSavedItems();
		Floaters.FloorWorldPositionFunction = null;
	}

	private void SetPanningPosToLevelTop()
	{
		GameBoard board = this.levelSession.TurnLogic.Board;
		float d = board.GetBoardRowNrInWorldSpace(board.GetLowestRowIndex()) + base.GetElementSize().y * 0.5f;
		this.panningRoot.transform.localPosition = Vector3.down * d;
	}

	private void HandleSessionStateChanged(LevelSession session)
	{
		LevelSessionState sessionState = this.levelSession.SessionState;
		if (sessionState == LevelSessionState.ReadyForAftermath)
		{
			this.fiberRunner.Run(this.LevelCompletedFlow(), false);
		}
	}

	private void HandleTutorialChangedStep(ITutorialStep step)
	{
		TutorialStep tutorialStep = step as TutorialStep;
		this.isTutorialActive = (tutorialStep != null);
		if (this.isTutorialActive)
		{
			if (tutorialStep.dismissType != TutorialStep.DismissType.UseBooster)
			{
				this.gameHud.BoosterBar.DisableAllButtons();
			}
			else
			{
				this.gameHud.BoosterBar.DisableAllButtons(tutorialStep.useBoosterType);
			}
		}
		else
		{
			this.gameHud.BoosterBar.EnableAllButtons();
		}
	}

	private void HandlePiggyBank()
	{
		if (this.piggyBankStateController == null)
		{
			return;
		}
		if (!this.piggyBankStateController.IsActive())
		{
			return;
		}
		this.piggyBankGameSessionController = ManagerRepository.Get<PiggyBankSystem>().ControllerFactory.CreateGameSessionController();
		PiggyBankProvider piggyBankProvider = (PiggyBankProvider)this.piggyBankGameSessionController.Provider;
		piggyBankProvider.RegisterLevelSession(this.levelSession);
	}

	private void TriggerPiggyBankInGameBoosterUsed()
	{
		if (this.IsPiggyBankActive())
		{
			this.piggyBankCollectorBooster.GetInstance<PiggyBankCollector>().Activate();
		}
	}

	private void TriggerPiggyBankEnd()
	{
		if (this.IsPiggyBankActive())
		{
			this.piggyBankCollectorEndGame.GetInstance<PiggyBankCollector>().Activate();
		}
	}

	private bool IsPiggyBankActive()
	{
		return this.piggyBankStateController != null && this.piggyBankStateController.IsActive();
	}

	private IEnumerator PanLevelToBottom()
	{
		yield return new Fiber.OnExit(delegate()
		{
			this.panningRoot.transform.localPosition = Vector3.zero;
			this.isPanning = false;
			UICamera.EnableInput();
		});
		this.isPanning = true;
		UICamera.DisableInput();
		float distance = Mathf.Abs(this.panningRoot.transform.localPosition.y);
		float unitsPerSec = SingletonAsset<LevelVisuals>.Instance.boardPanningInUnitsPerSecond;
		yield return FiberAnimation.MoveLocalTransform(this.panningRoot, this.panningRoot.transform.localPosition, Vector3.zero, AnimationCurve.EaseInOut(0f, 0f, 1f, 1f), distance / unitsPerSec);
		yield break;
	}

	private IEnumerator StartupSequence(bool moveCameraInstantly)
	{
		if (!moveCameraInstantly)
		{
			this.gameHud.BoosterBar.Hide();
			yield return this.PanLevelToBottom();
			this.gameHud.BoosterBar.Show();
		}
		else
		{
			this.panningRoot.transform.localPosition = Vector3.zero;
			this.gameHud.BoosterBar.Show();
		}
		Time.timeScale = 1f;
		UICamera.DisableInput();
		if (this.levelSession.Level.LevelAsset is BossLevel)
		{
			yield return this.levelSession.BossLevelController.LevelIntroduction();
		}
		yield return this.ResolvePregameBoosters();
		UICamera.EnableInput();
		yield return this.ResolveTutorial();
		UICamera.DisableInput();
		yield return this.ResolveHotStreakBoosters();
		yield return this.ResolveScheduledBoosters();
		UICamera.EnableInput();
		if (!this.levelSession.Tutorial.HasSteps)
		{
			UIViewManager.UIViewStateGeneric<LevelObjectiveView> vs = UIViewManager.Instance.ShowView<LevelObjectiveView>(new object[]
			{
				this.levelSession
			});
			yield return vs.WaitForClose();
			bool didReceiveFreeGift = (LevelObjectiveView.Result)vs.ClosingResult != LevelObjectiveView.Result.NoFreeBeeObtained;
			if (didReceiveFreeGift)
			{
				bool wasBought = (LevelObjectiveView.Result)vs.ClosingResult == LevelObjectiveView.Result.FreebieBought;
				yield return FreebieLogic.ResolveFreeGift(wasBought, this.levelSession, this);
			}
		}
		yield break;
	}

	private IEnumerator ResolvePregameBoosters()
	{
		foreach (SelectedBooster b in this.levelSession.PregameBoosters)
		{
			if (!b.isFree)
			{
				InventoryManager.Instance.Consume(b.id, 1, "LevelStarted");
			}
			yield return BoosterLogic.ResolveBooster(b.id, this.levelSession, this);
		}
		yield break;
	}

	private IEnumerator ResolveTutorial()
	{
		if (!this.levelSession.Tutorial.HasSteps)
		{
			yield break;
		}
		this.levelSession.Tutorial.Begin();
		this.isTutorialActive = true;
		while (this.isTutorialActive)
		{
			yield return null;
		}
		yield break;
	}

	private IEnumerator ResolveHotStreakBoosters()
	{
		if (!(this.levelSession.Level.LevelCollection is MainLevelDatabase))
		{
			yield break;
		}
		HotStreakManager hotStreakManager = FeatureManager.GetFeatureHandler<HotStreakManager>();
		List<ItemAmount> boosters = hotStreakManager.CurrentTierBonus;
		if (boosters.Count > 0)
		{
			EffectPool.Instance.SpawnEffect("HotStreakEffect", Vector3.zero, base.gameObject.layer, new object[0]);
			yield return FiberHelper.Wait(1.3f, (FiberHelper.WaitFlag)0);
			foreach (ItemAmount booster in boosters)
			{
				yield return BoosterLogic.ResolveBooster(booster.ItemId, this.levelSession, this);
			}
		}
		yield break;
	}

	private IEnumerator ResolveScheduledBoosters()
	{
		if (this.ScheduledBoosters.HasBoosterForLocation(ScheduledBoosterLocation.PreGame, this.levelSession.Level))
		{
			IScheduledBooster booster = this.ScheduledBoosters.GetBoosterForLocation(ScheduledBoosterLocation.PreGame);
			bool success = this.ScheduledBoosters.UseScheduledBoosterIfPossible(booster, this.levelSession.SessionId.id, this.levelSession.Level.GetContextWithFeatures());
			if (success)
			{
				yield return this.UseScheduledBooster(booster);
			}
		}
		yield break;
	}

	private IEnumerator UseScheduledBooster(IScheduledBooster booster)
	{
		List<BoosterLogic> listOfBoosterLogic = booster.Definition.listOfBoosterLogic;
		foreach (BoosterLogic boosterLogic in listOfBoosterLogic)
		{
			yield return BoosterLogic.ResolveBooster(boosterLogic, this.levelSession, this);
		}
		yield break;
	}

	private void HandlePieceCleared(CPPiece piece, int pointsToGive, HitMark hit)
	{
		if (pointsToGive > 0 && hit.cause != HitCause.Power && !this.pointEffectsDisabled)
		{
			SpawnedEffect spawnedEffect = EffectPool.Instance.SpawnEffect("DriftingPoints", Vector3.zero, piece.gameObject.layer, new object[]
			{
				pointsToGive.ToString(),
				piece.MatchFlag
			});
			spawnedEffect.transform.position = piece.transform.position + Vector3.back * 6f;
			spawnedEffect.transform.parent = this.levelSession.NonMovingEffectRoot;
		}
		if (piece is DeathPiece)
		{
			SingletonAsset<SoundDatabase>.Instance.deathBubblePop.PlaySequential();
		}
		else if (hit.cause == HitCause.Power)
		{
			SingletonAsset<SoundDatabase>.Instance.poppedByPower.PlaySequential();
		}
		else
		{
			SingletonAsset<SoundDatabase>.Instance.bubblePop.PlaySequential();
		}
		if (piece is GoalPiece)
		{
			SingletonAsset<SoundDatabase>.Instance.kittenReleased.Play();
			if (this.levelSession.GetGoalPiecesRemaining() <= 0)
			{
				SingletonAsset<SoundDatabase>.Instance.victoriousMotif.Play();
				AudioManager.Instance.SetMusic(null, true);
			}
		}
	}

	private void HandlePieceDiscarded(CPPiece piece)
	{
		EffectPool.Instance.SpawnEffect("BubblePopSmoke", piece.transform.position, piece.gameObject.layer, new object[0]);
	}

	private void HandleResolveCompleted(TurnLogic turnLogic)
	{
		SingletonAsset<SoundDatabase>.Instance.poppedByPower.ResetSequential();
		SingletonAsset<SoundDatabase>.Instance.bubblePop.ResetSequential();
		TurnLogic.PointsSummary pointsThisTurn = turnLogic.pointsThisTurn;
		if (pointsThisTurn.totalPoints > 0 && turnLogic.StreakMultiplier > 1 && !this.pointEffectsDisabled)
		{
			SingletonAsset<SoundDatabase>.Instance.receivePoints.Play();
			Vector2 v = pointsThisTurn.areaOfGivenPoints.Center + Vector2.down * (pointsThisTurn.areaOfGivenPoints.Size.y * 0.5f + 50f);
			string text = string.Format("{0}x{1}", pointsThisTurn.totalPoints, turnLogic.StreakMultiplier - 1);
			string awesomeText = SingletonAsset<LevelVisuals>.Instance.GetAwesomeText(turnLogic.StreakMultiplier);
			EffectPool.Instance.SpawnEffect("SummaryPoints", v, base.gameObject.layer, new object[]
			{
				text,
				string.Empty,
				awesomeText
			});
		}
	}

	protected override void ScreenSizeChanged()
	{
		this.levelSession.TurnLogic.Board.FitToEnclosingElement(this.boardPortraitBounds);
	}

	private void BoosterActivated(InventoryItem boosterId)
	{
		this.fiberRunner.Run(BoosterLogic.ResolveBooster(boosterId, this.levelSession, this), false);
	}

	private IEnumerator LevelCompletedFlow()
	{
		UICamera.DisableInput();
		yield return new Fiber.OnExit(delegate()
		{
			UICamera.EnableInput();
		});
		this.cannonBallQueue.HideAllBallsExceptMain();
		yield return FiberHelper.RunParallel(new IEnumerator[]
		{
			AfterMath.DetachRemainingPieces(this.levelSession),
			FiberHelper.RunSerial(new IEnumerator[]
			{
				EffectPool.Instance.WaitForNoEffectWithNamePlaying("SavedKittenEffect"),
				AfterMath.Run(this.levelSession, this.cannonBallQueue)
			})
		});
		yield return this.savedKittens.WaitForAllLanded();
		UIViewManager.UIViewStateGeneric<LevelCompleteSlideInView> vs = UIViewManager.Instance.ShowView<LevelCompleteSlideInView>(new object[0]);
		this.TriggerPiggyBankEnd();
		yield return vs.WaitForClose();
		while (EffectPool.Instance.AnyEffectsPlaying)
		{
			yield return null;
		}
		this.levelSession.SetState(LevelSessionState.Completed);
		yield break;
	}

	public IEnumerator OutOfMovesFlow()
	{
		while (this.boardCamera.IsAnimating || this.levelSession.TurnLogic.FloatersAreAnimating)
		{
			yield return null;
		}
		while (EffectPool.Instance.AnyEffectsPlaying)
		{
			yield return null;
		}
		bool showScheduledBooster = this.ScheduledBoosters.HasBoosterForLocation(ScheduledBoosterLocation.PostGame, this.levelSession.Level);
		UIViewManager.UIViewState vs;
		if (this.ShouldShowSpecialContinue() && !showScheduledBooster)
		{
			vs = UIViewManager.Instance.ShowView<SpecialOutOfMovesView>(new object[]
			{
				this.levelSession,
				this.GetUnusedBoostersForSpecialContinueOffer()
			});
		}
		else
		{
			vs = UIViewManager.Instance.ShowView<OutOfMovesView>(new object[]
			{
				this.levelSession,
				showScheduledBooster
			});
		}
		yield return vs.WaitForClose();
		if (showScheduledBooster)
		{
			EnumeratorResult<bool> didUseLABPostGameBooster = new EnumeratorResult<bool>();
			yield return this.UseLABPostGameBoosterIfPossible(didUseLABPostGameBooster, null);
			if (didUseLABPostGameBooster)
			{
				yield break;
			}
		}
		if (vs.ClosingResult is List<InventoryItem>)
		{
			List<InventoryItem> additionalItems = (List<InventoryItem>)vs.ClosingResult;
			if (additionalItems != null)
			{
				this.levelSession.Stats.IncrementSpecialContinueUsed();
				yield return BoosterLogic.ResolveBooster("BoosterExtraMoves", this.levelSession, this);
				yield return new WaitForSeconds(1f);
				foreach (InventoryItem additionalItem in additionalItems)
				{
					yield return BoosterLogic.ResolveBooster(additionalItem, this.levelSession, this);
				}
				yield return BoosterLogic.ResolveBooster(SingletonAsset<LevelVisuals>.Instance.cookieJarFillUpBoosterLogic, this.levelSession, this);
				yield break;
			}
		}
		if ((int)vs.ClosingResult == 1)
		{
			this.levelSession.Stats.IncrementContinueUsed();
			yield return BoosterLogic.ResolveBooster("BoosterExtraMoves", this.levelSession, this);
			yield break;
		}
		this.levelSession.SetState(LevelSessionState.Failed);
		yield break;
	}

	private IEnumerator UseLABPostGameBoosterIfPossible(EnumeratorResult<bool> didUseLABPostGameBooster, Action didUseLABPostGameBoosterCallback = null)
	{
		didUseLABPostGameBooster.value = false;
		IScheduledBooster booster = this.ScheduledBoosters.GetBoosterForLocation(ScheduledBoosterLocation.PostGame);
		if (booster.IsActive)
		{
			if (didUseLABPostGameBoosterCallback != null)
			{
				didUseLABPostGameBoosterCallback();
			}
			bool success = this.ScheduledBoosters.UseScheduledBoosterIfPossible(booster, this.levelSession.SessionId.id, this.levelSession.Level.GetContextWithFeatures());
			if (success)
			{
				yield return this.UseScheduledBooster(booster);
			}
			didUseLABPostGameBooster.value = true;
		}
		yield break;
	}

	private bool ShouldShowSpecialContinue()
	{
		if (this.levelSession.GetContinuousFailsForThisLevel() < this.ConfigurationManager.GetConfig<SpecialContinueOfferConfig>().NeedToFailXTimes)
		{
			return false;
		}
		SpecialContinueOfferConfig config = this.ConfigurationManager.GetConfig<SpecialContinueOfferConfig>();
		if (config.Probability == 0)
		{
			return false;
		}
		if (this.levelSession.Level.IsCompleted)
		{
			return false;
		}
		if (this.UnusedBoostersForCurrentLevel().Count == 0)
		{
			return false;
		}
		if (!TactilePlayerPrefs.GetBool("SpecialContinueOfferDidShow", false))
		{
			TactilePlayerPrefs.SetBool("SpecialContinueOfferDidShow", true);
			return true;
		}
		return UnityEngine.Random.value < Mathf.Clamp((float)config.Probability * 0.01f, 0f, 1f);
	}

	private List<InventoryItem> UnusedBoostersForCurrentLevel()
	{
		List<InventoryItem> list = new List<InventoryItem>();
		if (!this.levelSession.BallQueue.HasTripleQueue && BoosterManagerBase<BoosterManager>.Instance.IsBoosterUnlocked("BoosterSuperQueue"))
		{
			list.Add("BoosterSuperQueue");
		}
		if (!this.levelSession.Cannon.HasSuperAim && BoosterManagerBase<BoosterManager>.Instance.IsBoosterUnlocked("BoosterSuperAim"))
		{
			list.Add("BoosterSuperAim");
		}
		bool flag = LevelUtils.PieceWithTypeExist<MinusPiece>(this.levelSession.Level.LevelAsset as LevelAsset, null);
		bool flag2 = LevelUtils.PieceWithTypeExist<DeathPiece>(this.levelSession.Level.LevelAsset as LevelAsset, null);
		if ((flag || flag2) && !this.levelSession.ShieldActive && BoosterManagerBase<BoosterManager>.Instance.IsBoosterUnlocked("BoosterShield"))
		{
			list.Add("BoosterShield");
		}
		return list;
	}

	private InventoryItem GetRandomUnusedBoosterForSpecialContinueOffer()
	{
		List<InventoryItem> list = this.UnusedBoostersForCurrentLevel();
		if (list.Count > 0)
		{
			return list[UnityEngine.Random.Range(0, list.Count)];
		}
		return null;
	}

	private List<InventoryItem> GetUnusedBoostersForSpecialContinueOffer()
	{
		return this.UnusedBoostersForCurrentLevel();
	}

	public IEnumerator DeathTriggeredFlow()
	{
		while (this.boardCamera.IsAnimating || this.levelSession.TurnLogic.FloatersAreAnimating)
		{
			yield return null;
		}
		while (EffectPool.Instance.AnyEffectsPlaying)
		{
			yield return null;
		}
		bool showScheduledBooster = this.ScheduledBoosters.HasBoosterForLocation(ScheduledBoosterLocation.PostGame, this.levelSession.Level);
		UIViewManager.UIViewStateGeneric<DeathTriggeredView> vs = UIViewManager.Instance.ShowView<DeathTriggeredView>(new object[]
		{
			this.levelSession,
			showScheduledBooster
		});
		yield return vs.WaitForClose();
		if (showScheduledBooster)
		{
			EnumeratorResult<bool> didUseLABPostGameBooster = new EnumeratorResult<bool>();
			yield return this.UseLABPostGameBoosterIfPossible(didUseLABPostGameBooster, delegate
			{
				this.levelSession.ResetDeathTriggeredAfterPurchaseAndResume();
			});
			if (didUseLABPostGameBooster)
			{
				this.levelSession.Stats.IncrementContinueAfterDeathUsed();
				yield return BoosterLogic.ResolveBooster("BoosterShield", this.levelSession, this);
				yield break;
			}
		}
		int ballsLeft = this.levelSession.BallQueue.BallsLeft;
		if ((int)vs.ClosingResult == 1)
		{
			this.levelSession.ResetDeathTriggeredAfterPurchaseAndResume();
			if (ballsLeft == 0)
			{
				yield return BoosterLogic.ResolveBooster("BoosterExtraMoves", this.levelSession, this);
			}
			else
			{
				this.levelSession.SetState(LevelSessionState.Playing);
			}
			this.levelSession.Stats.IncrementContinueAfterDeathUsed();
			yield break;
		}
		if ((int)vs.ClosingResult == 2)
		{
			yield return BoosterLogic.ResolveBooster("BoosterShield", this.levelSession, this);
			this.levelSession.ResetDeathTriggeredAfterPurchaseAndResume();
			if (ballsLeft == 0)
			{
				yield return BoosterLogic.ResolveBooster("BoosterExtraMoves", this.levelSession, this);
			}
			else
			{
				this.levelSession.SetState(LevelSessionState.Playing);
			}
			this.levelSession.Stats.IncrementConinueAfterDeathWithShieldUsed();
			yield break;
		}
		this.levelSession.SetState(LevelSessionState.Failed);
		yield break;
	}

	private void PauseButtonClicked(UIEvent b)
	{
		this.TryAndPause();
	}

	private void TryAndPause()
	{
		if (this.levelSession.IsTurnResolving)
		{
			return;
		}
		if (this.levelSession.IsDeathTriggered)
		{
			return;
		}
		if (this.levelSession.SessionState != LevelSessionState.Playing)
		{
			return;
		}
		this.fiberRunner.Run(this.PauseViewFlow(), false);
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0) && this.isPanning)
		{
			Time.timeScale *= 4f;
		}
		if (UIViewManager.Instance.IsEscapeKeyDownAndAvailable(base.gameObject.layer))
		{
			this.TryAndPause();
		}
	}

	private void OnDestroy()
	{
		this.fiberRunner.Terminate();
		EffectPool instance = EffectPool.Instance;
		instance.EffectSpawned = (Action<string, SpawnedEffect>)Delegate.Remove(instance.EffectSpawned, new Action<string, SpawnedEffect>(this.EffectPool_EffectSpawned));
	}

	private IEnumerator PauseViewFlow()
	{
		UIViewManager.UIViewStateGeneric<PauseMenuView> vs = UIViewManager.Instance.ShowView<PauseMenuView>(new object[]
		{
			this.levelSession
		});
		yield return vs.WaitForClose();
		PauseMenuView.Result result = (PauseMenuView.Result)vs.ClosingResult;
		if (result == PauseMenuView.Result.GoToMap)
		{
			this.levelSession.SetState(LevelSessionState.Abandoned);
		}
		yield break;
	}

	public void CheatComplete(bool success, int pointsToAdd = 0)
	{
	}

	public override Vector2 CalculateViewSizeForScreen(Vector2 screenSize)
	{
		Vector2 originalSize = base.OriginalSize;
		float num = base.OriginalSize.x / base.OriginalSize.y;
		float num2 = screenSize.Aspect();
		bool flag = this.levelSession.Level.LevelAsset is EndlessLevel;
		if (flag)
		{
			float max = 850f;
			float num3 = 180f;
			originalSize = new Vector2(Mathf.Clamp(originalSize.x + num2 * num3, 0f, max), originalSize.y);
		}
		Vector2 result = UIUtility.CorrectSizeToAspect(originalSize, num2, AspectCorrection.Fill);
		bool active = flag || num2 > num + 0.01f;
		for (int i = 0; i < this.sideBars.Length; i++)
		{
			this.sideBars[i].SetActive(active);
		}
		this.HandleSafeArea();
		return result;
	}

	private void HandleSafeArea()
	{
	}

	public new Camera ViewCamera
	{
		get
		{
			return base.ViewCamera;
		}
	}

	public void StartStartupSequence()
	{
		this.fiberRunner.Run(this.StartupSequence(true), false);
	}

	public void SetPanningPosToTargetPosition()
	{
		this.panningRoot.transform.localPosition = Vector3.zero;
	}

	public void UnhookLevelSessionListener()
	{
		this.levelSession.StateChanged -= this.HandleSessionStateChanged;
	}

	public void DisablePointsEffects()
	{
		this.pointEffectsDisabled = true;
	}

	private void EffectPool_EffectSpawned(string effectName, SpawnedEffect effect)
	{
		if (effect == null)
		{
			return;
		}
		if (effectName != null)
		{
			if (GameView._003C_003Ef__switch_0024map0 == null)
			{
				GameView._003C_003Ef__switch_0024map0 = new Dictionary<string, int>(9)
				{
					{
						"FinalPowerHitEffect",
						0
					},
					{
						"NinjaStarHitEffect",
						0
					},
					{
						"NinjaStarSingleHitEffect",
						0
					},
					{
						"SummaryPoints",
						0
					},
					{
						"NoteHitEffect",
						0
					},
					{
						"IceExplode",
						0
					},
					{
						"FrogHitEffect",
						0
					},
					{
						"BurnEffect",
						0
					},
					{
						"BubblePopSmoke",
						0
					}
				};
			}
			int num;
			if (GameView._003C_003Ef__switch_0024map0.TryGetValue(effectName, out num))
			{
				if (num == 0)
				{
					effect.transform.parent = this.effectsPanningRoot;
				}
			}
		}
	}

	[SerializeField]
	public GameHud gameHud;

	[SerializeField]
	public PowerArea powerArea;

	[SerializeField]
	public CannonOperator cannonOperator;

	[SerializeField]
	public SavedKittens savedKittens;

	[SerializeField]
	public GameCannon cannon;

	[SerializeField]
	public CannonBallQueue cannonBallQueue;

	[SerializeField]
	public UIElement boardPortraitBounds;

	[SerializeField]
	public BossLevelAreas bossLevelAreas;

	[SerializeField]
	private CannonTracerDots tracerDots;

	[SerializeField]
	private Transform panningRoot;

	[SerializeField]
	private BoardCamera boardCamera;

	[SerializeField]
	private ShieldVisuals shield;

	[SerializeField]
	private BuySpecialBooster BuySpecialBooster;

	[SerializeField]
	private GameObject[] sideBars;

	[SerializeField]
	private UIElement topBar;

	[SerializeField]
	private UIInstantiator backgroundInstantiator;

	[SerializeField]
	private Transform topOfShipRailing;

	[SerializeField]
	private GameObject superAimMonocularPrefab;

	[SerializeField]
	private UIInstantiator piggyBankCollectorBooster;

	[SerializeField]
	private UIInstantiator piggyBankCollectorEndGame;

	private readonly FiberRunner fiberRunner = new FiberRunner();

	private LevelSession levelSession;

	private bool isPanning;

	private SkyBackground skyBackground;

	private bool pointEffectsDisabled;

	private Transform effectsPanningRoot;

	private bool isTutorialActive;

	private SuperAimMonocular superAimMonocularInitializer;

	private PiggyBankStateController piggyBankStateController;

	private PiggyBankGameSessionController piggyBankGameSessionController;
}
