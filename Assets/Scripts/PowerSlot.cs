using System;
using System.Collections;
using System.Diagnostics;
using Fibers;
using JetBrains.Annotations;
using UnityEngine;

public class PowerSlot : MonoBehaviour
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<PowerSlot.CookieMovementPositions> CustomMovementPositions;

	public MatchFlag MatchFlag
	{
		get
		{
			return this.matchFlag;
		}
	}

	public PowerCat CatSpine
	{
		get
		{
			return this.catSpine;
		}
	}

	public GamePowers.Power Power { get; private set; }

	public void Initialize(GamePowers.Power power, LevelSession levelSession)
	{
		if (this.shimmer != null)
		{
			this.shimmer.gameObject.SetActive(false);
		}
		this.Power = power;
		this.levelSession = levelSession;
		this.catSpine.Initialize();
		this.AddLevelEventListeners();
		this.PowerEnded();
	}

	private void OnDestroy()
	{
		this.fiberRunner.Terminate();
		this.RemoveLevelEventListeners();
	}

	private void Update()
	{
		this.fiberRunner.Step();
	}

	private void AddLevelEventListeners()
	{
		this.levelSession.NextMovePrepared += this.HandleNextMovePrepared;
		GamePowers.Power power = this.Power;
		power.ValueChanged = (Action<MatchFlag, Vector3>)Delegate.Combine(power.ValueChanged, new Action<MatchFlag, Vector3>(this.ValueChanged));
		this.levelSession.StateChanged += this.LevelSessionStateChanged;
	}

	private void RemoveLevelEventListeners()
	{
		if (this.levelSession != null)
		{
			this.levelSession.NextMovePrepared -= this.HandleNextMovePrepared;
			this.levelSession.StateChanged -= this.LevelSessionStateChanged;
		}
		if (this.Power != null)
		{
			GamePowers.Power power = this.Power;
			power.ValueChanged = (Action<MatchFlag, Vector3>)Delegate.Remove(power.ValueChanged, new Action<MatchFlag, Vector3>(this.ValueChanged));
		}
	}

	private void ChangeState(PowerSlot.State nextState)
	{
		if (this.currentState == nextState)
		{
			return;
		}
		switch (nextState)
		{
		case PowerSlot.State.FILLING:
			this.SetActiveButtonActive(false, nextState);
			this.DeActivateShimmer();
			this.UpdateProgress(this.Power.Progress);
			this.SetFillBarActive(true);
			this.ChangeCatSpineStateIfNotTheSame(PowerCat.State.Idle);
			break;
		case PowerSlot.State.DRAW_ATTENTION:
			this.SetActiveButtonActive(false, nextState);
			this.ActivateShimmer();
			this.SetFillBarActive(true);
			this.ChangeCatSpineStateIfNotTheSame(PowerCat.State.Idle);
			break;
		case PowerSlot.State.DRAW_ATTENTION_CHARGED:
			this.SetActiveButtonActive(true, nextState);
			this.ActivateShimmer();
			this.SetFillBarActive(false);
			this.ChangeCatSpineStateIfNotTheSame(PowerCat.State.Charged);
			break;
		case PowerSlot.State.CHARGED:
			this.SetActiveButtonActive(true, nextState);
			this.ActivateShimmer();
			this.SetFillBarActive(false);
			this.ChangeCatSpineStateIfNotTheSame(PowerCat.State.Charged);
			break;
		case PowerSlot.State.ACTIVE:
			this.SetActiveButtonActive(false, nextState);
			this.DeActivateShimmer();
			this.SetFillBarActive(false);
			this.ChangeCatSpineStateIfNotTheSame(PowerCat.State.Happy);
			break;
		case PowerSlot.State.AFTERMATH:
			this.SetActiveButtonActive(false, nextState);
			this.DeActivateShimmer();
			this.SetFillBarActive(false);
			this.ChangeCatSpineStateIfNotTheSame(PowerCat.State.Happy);
			break;
		}
		this.currentState = nextState;
	}

	private void ChangeCatSpineStateIfNotTheSame(PowerCat.State newState)
	{
		if (this.catSpine.CurrentState == newState)
		{
			return;
		}
		this.catSpine.SetState(newState);
	}

	private void ValueChanged(MatchFlag color, Vector3 worldPosition)
	{
		if (this.currentState == PowerSlot.State.FILLING || this.currentState == PowerSlot.State.DRAW_ATTENTION)
		{
			this.fiberRunner.Run(this.AnimateCollectedCookie(color, worldPosition, this.Power.Progress), false);
		}
	}

	private void UpdateProgress(float progress)
	{
		this.fill.FillAmount = progress;
		this.catSpine.SetBowlFill(progress);
	}

	private void HandleNextMovePrepared()
	{
		SingletonAsset<SoundDatabase>.Instance.cookieToPowercat.ResetSequential();
		this.gotCookiesThisTurn = false;
	}

	private void LevelSessionStateChanged(LevelSession session)
	{
		if (session.SessionState == LevelSessionState.ReadyForAftermath)
		{
			this.ChangeState(PowerSlot.State.AFTERMATH);
		}
	}

	private IEnumerator AnimateCollectedCookie(MatchFlag color, Vector3 worldPosition, float progress)
	{
		GameObject collectedCookie = UnityEngine.Object.Instantiate<GameObject>(SingletonAsset<LevelVisuals>.Instance.collectPrefab, base.transform, true);
		collectedCookie.GetComponent<UISprite>().SpriteName = SingletonAsset<LevelVisuals>.Instance.GetCollectSpriteForColor(color);
		collectedCookie.gameObject.SetLayerRecursively(base.gameObject.layer);
		PowerSlot.CookieMovementPositions movementPositions = new PowerSlot.CookieMovementPositions
		{
			startWorldPosition = worldPosition,
			startPos = worldPosition,
			endPos = this.cookieCollectTarget.position
		};
		movementPositions.startPos.z = movementPositions.endPos.z;
		PowerSlot.CustomMovementPositions(movementPositions);
		yield return new Fiber.OnExit(delegate()
		{
			UnityEngine.Object.Destroy(collectedCookie.gameObject);
		});
		yield return FiberHelper.RunParallel(new IEnumerator[]
		{
			FiberAnimation.ScaleTransform(collectedCookie.transform, Vector3.zero, Vector3.one, SingletonAsset<LevelVisuals>.Instance.collectScaleCurve, 0f),
			FiberAnimation.MoveTransform(collectedCookie.transform, movementPositions.startPos, movementPositions.endPos, SingletonAsset<LevelVisuals>.Instance.collectCurve, 0f)
		});
		if (this.currentState != PowerSlot.State.FILLING && this.currentState != PowerSlot.State.DRAW_ATTENTION)
		{
			yield break;
		}
		SingletonAsset<SoundDatabase>.Instance.cookieToPowercat.PlaySequential();
		bool isCharged = progress >= 1f && this.levelSession.GetGoalPiecesRemaining() > 0;
		this.UpdateProgress(progress);
		if (isCharged)
		{
			this.ChangeState((this.currentState != PowerSlot.State.DRAW_ATTENTION) ? PowerSlot.State.CHARGED : PowerSlot.State.DRAW_ATTENTION_CHARGED);
			TutorialStep step = this.levelSession.Tutorial.CurrentStep as TutorialStep;
			if (step == null || step.dismissType == TutorialStep.DismissType.UseBooster)
			{
				EffectPool.Instance.SpawnEffect("CatPowerReadyEffect", Vector3.zero, base.gameObject.layer, new object[]
				{
					SingletonAsset<LevelVisuals>.Instance.powercatReady,
					this.matchFlag,
					this.shimmer.transform
				});
				this.ActivateShimmer();
				yield return FiberHelper.Wait(1.5f, (FiberHelper.WaitFlag)0);
			}
			SingletonAsset<SoundDatabase>.Instance.GetPowerSounds(this.matchFlag).ready.Play();
		}
		else
		{
			if (!this.gotCookiesThisTurn)
			{
				SingletonAsset<SoundDatabase>.Instance.GetPowerSounds(this.matchFlag).thanksForCookie.Play();
				this.gotCookiesThisTurn = true;
			}
			this.ChangeCatSpineStateIfNotTheSame(PowerCat.State.Happy);
		}
		yield break;
	}

	private IEnumerator AnimateShimmer()
	{
		if (this.shimmer == null)
		{
			yield break;
		}
		this.shimmer.gameObject.SetActive(true);
		yield return FiberAnimation.ScaleTransform(this.shimmer.transform, Vector3.zero, Vector3.one, SingletonAsset<LevelVisuals>.Instance.powerCatShimmerScale, 0f);
		yield break;
	}

	private void ActivateShimmer()
	{
		if (this.shimmer != null && !this.shimmer.gameObject.activeSelf)
		{
			this.fiberRunner.Run(this.AnimateShimmer(), false);
		}
	}

	private void DeActivateShimmer()
	{
		if (this.shimmer != null && this.shimmer.gameObject.activeSelf)
		{
			this.fiberRunner.Terminate();
			this.shimmer.gameObject.SetActive(false);
		}
	}

	private void SetActiveButtonActive(bool pIsActive, PowerSlot.State nextState)
	{
		string key = string.Empty;
		if (nextState == PowerSlot.State.CHARGED)
		{
			key = L.Get("Use");
		}
		else if (nextState == PowerSlot.State.DRAW_ATTENTION_CHARGED)
		{
			key = L.Get("Combine");
		}
		if (this.activateButton != null)
		{
			this.activateButton.SetActive(pIsActive);
			if (pIsActive)
			{
				this.activateButton.gameObject.GetComponentInChildren<UILabel>().text = L.Get(key);
			}
		}
	}

	private void SetFillBarActive(bool pIsActive)
	{
		if (this.fillMeter != null)
		{
			this.fillMeter.SetActive(pIsActive);
		}
	}

	[UsedImplicitly]
	private void ChargerClicked(UIEvent e)
	{
		if (!this.Power.IsChargingEnabled || this.levelSession.SessionState != LevelSessionState.Playing || this.Power.IsActivated || this.levelSession.IsTurnResolving || this.levelSession.IsDeathTriggered)
		{
			return;
		}
		if (this.Power.IsCharged)
		{
			this.levelSession.Stats.GetPowercomboStats().SetColorByMatchflag(this.matchFlag, 100);
			this.fiberRunner.Terminate();
			this.Power.Activate();
			this.shimmer.gameObject.SetActive(false);
			SingletonAsset<SoundDatabase>.Instance.GetPowerSounds(this.matchFlag).activated.Play();
			SingletonAsset<SoundDatabase>.Instance.powerActivation.Play();
			this.ChangeState(PowerSlot.State.ACTIVE);
		}
		else
		{
			FiberCtrl.Pool.Run(this.BuyRefill(), false);
		}
	}

	private IEnumerator BuyRefill()
	{
		SingletonAsset<UISetup>.Instance.soundButtonNormal.Play();
		ShopItem shopItem = ShopManager.Instance.GetShopItem(this.refillShopItem);
		bool isFreeBoosterAvailable = false;
		TutorialStep tutorialStep = this.levelSession.Tutorial.CurrentStep as TutorialStep;
		if (tutorialStep != null)
		{
			isFreeBoosterAvailable = (tutorialStep.dismissType == TutorialStep.DismissType.UsePower || tutorialStep.dismissType == TutorialStep.DismissType.WaitForFreePowerClaimed);
			if (!isFreeBoosterAvailable)
			{
				yield break;
			}
		}
		UIViewManager.UIViewStateGeneric<BuyCatPowerView> vs = UIViewManager.Instance.ShowView<BuyCatPowerView>(new object[]
		{
			shopItem,
			isFreeBoosterAvailable
		});
		yield return vs.WaitForClose();
		if ((BuyCatPowerView.Result)vs.ClosingResult == BuyCatPowerView.Result.SuccessfullyBought)
		{
			yield return this.PurchasedRefill();
		}
		yield break;
	}

	private IEnumerator PurchasedRefill()
	{
		int fillValue = (int)(this.Power.Progress * 100f);
		this.levelSession.Stats.GetPowercomboStats().SetColorByMatchflag(this.matchFlag, fillValue);
		this.levelSession.PurchasedPower = this.Power;
		GameView gameView = UIViewManager.Instance.FindView<GameView>();
		yield return BoosterLogic.ResolveBooster(SingletonAsset<LevelVisuals>.Instance.catPowerPurchaseBoosterLogic, this.levelSession, gameView);
		this.fiberRunner.Terminate();
		this.ChangeState(PowerSlot.State.ACTIVE);
		SingletonAsset<SoundDatabase>.Instance.GetPowerSounds(this.matchFlag).activated.Play();
		SingletonAsset<SoundDatabase>.Instance.powerActivation.Play();
		yield break;
	}

	public void PowerStarted()
	{
		if (this.Power.IsCharged && !this.Power.IsActivated)
		{
			this.ChangeState(PowerSlot.State.DRAW_ATTENTION_CHARGED);
		}
		else if (!this.Power.IsCharged && this.Power.IsActivated)
		{
			this.ChangeState(PowerSlot.State.ACTIVE);
		}
		else
		{
			this.ChangeState(PowerSlot.State.DRAW_ATTENTION);
		}
	}

	public void PowerEnded()
	{
		this.ChangeState((!this.Power.IsCharged) ? PowerSlot.State.FILLING : PowerSlot.State.CHARGED);
	}

	// Note: this type is marked as 'beforefieldinit'.
	static PowerSlot()
	{
		PowerSlot.CustomMovementPositions = delegate(PowerSlot.CookieMovementPositions A_0)
		{
		};
	}

	[SerializeField]
	private GameObject fillMeter;

	[SerializeField]
	private UIFillModifier fill;

	[SerializeField]
	private GameObject activateButton;

	[SerializeField]
	private MatchFlag matchFlag;

	[SerializeField]
	private PowerCat catSpine;

	[SerializeField]
	private ShopItemIdentifier refillShopItem;

	[SerializeField]
	private Transform cookieCollectTarget;

	[SerializeField]
	private UISprite shimmer;

	private readonly FiberRunner fiberRunner = new FiberRunner(FiberBucket.Manual);

	private PowerSlot.State currentState;

	private bool gotCookiesThisTurn;

	private LevelSession levelSession;

	private enum State
	{
		DEFAULT,
		FILLING,
		DRAW_ATTENTION,
		DRAW_ATTENTION_CHARGED,
		CHARGED,
		ACTIVE,
		AFTERMATH
	}

	public class CookieMovementPositions
	{
		public Vector3 startWorldPosition;

		public Vector3 startPos;

		public Vector3 endPos;
	}
}
