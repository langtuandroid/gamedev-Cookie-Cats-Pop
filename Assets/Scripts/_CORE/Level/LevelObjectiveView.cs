using System;
using System.Collections;
using Fibers;
using JetBrains.Annotations;
using TactileModules.Ads;
using TactileModules.Foundation;
using UnityEngine;

public class LevelObjectiveView : UIView
{
	protected override void ViewLoad(object[] parameters)
	{
		this.session = (LevelSession)parameters[0];
		this.amountLabel.text = "x " + this.session.TotalGoalPieces.ToString();
		this.spinnerButtonWatchAd = this.freeBoostButtonAd.GetInstance<ButtonWithSpinner>();
		this.buyButton = this.freeBoostButtonWithCoins.GetInstance<ButtonWithTitle>();
		this.animFiber.Start(this.WaitForUserInputAndAnimateOffScreen(5f));
		this.isTimerPaused = false;
		this.goalLabel.text = L.Get("Save the Kittens!");
		if (this.session.Level.LevelAsset is EndlessLevel)
		{
			this.saveKittensGoalObjective.gameObject.SetActive(false);
			this.goalLabel.text = L.Get("See how far you can get!");
			this.goalLabel.GetElement().LocalPosition = Vector2.zero;
			this.goalBar.Size = new Vector2(this.goalBar.GetElementSize().x, this.goalLabel.GetElementSize().y * 2f);
		}
	}

	protected override void ViewWillAppear()
	{
		this.slideIn.gameObject.SetActive(true);
		ShopItem shopItem = ShopManager.Instance.GetShopItem("ShopItemFreebie");
		this.buyButton.Title = shopItem.CurrencyPrice + " [C]";
		this.spinnerButtonWatchAd.PrimaryTitle = L.Get("Watch Ad");
		this.currencyOverlay = base.ObtainOverlay<CurrencyOverlay>();
	}

	protected override void ViewWillDisappear()
	{
		if (this.currencyOverlay != null)
		{
			base.ReleaseOverlay<CurrencyOverlay>();
		}
	}

	protected override void ViewGotFocus()
	{
		this.isTimerPaused = false;
		this.HookCoinButton();
	}

	private void HookCoinButton()
	{
		if (this.currencyOverlay != null)
		{
			this.currencyOverlay.OnCoinButtonClicked = delegate()
			{
				UIViewManager.Instance.ShowView<ShopView>(new object[]
				{
					0
				});
				this.isTimerPaused = true;
			};
		}
	}

	protected override void ViewLostFocus()
	{
		if (this.currencyOverlay != null)
		{
			this.currencyOverlay.OnCoinButtonClicked = null;
		}
	}
	
	private void OnDisable()
	{
		this.animFiber.Terminate();
		this.requestingVideoFiber.Terminate();
	}



	[UsedImplicitly]
	private void OnBuyButtonClick(UIEvent e)
	{
		if (this.userInputBlocked)
		{
			return;
		}
		this.animFiber.Start(this.Buy());
	}

	private IEnumerator Buy()
	{
		this.userInputBlocked = true;
		bool waitingForUserAction = true;
		UserCareManager.Instance.StartCare("Coin");
		ShopManager.Instance.TrySpendCoins("ShopItemFreebie", this.buyButton, delegate(bool success)
		{
			this.gotFreeBee = success;
			this.wasBought = success;
			waitingForUserAction = false;
		});
		while (waitingForUserAction)
		{
			yield return null;
		}
		UserCareManager.Instance.StopCare("Coin");
		yield return this.WaitForUserInputAndAnimateOffScreen((float)((!this.gotFreeBee) ? 3 : 0));
		yield break;
	}
	

	[UsedImplicitly]
	private void DismissClicked(UIEvent e)
	{
		this.didDismiss = true;
	}

	private IEnumerator WaitForUserInputAndAnimateOffScreen(float timeout)
	{
		this.userInputBlocked = false;
		while (timeout > 0f && !this.didDismiss)
		{
			if (!this.isTimerPaused)
			{
				timeout -= Time.deltaTime;
			}
			yield return null;
		}
		this.userInputBlocked = true;
		if (!this.gotFreeBee)
		{
			base.Close(LevelObjectiveView.Result.NoFreeBeeObtained);
		}
		else
		{
			base.Close((!this.wasBought) ? LevelObjectiveView.Result.FreebieFromAd : LevelObjectiveView.Result.FreebieBought);
		}
		yield break;
	}

	[SerializeField]
	private Transform slideIn;

	[SerializeField]
	private UILabel amountLabel;

	[SerializeField]
	private GameObject freeBoostPivot;

	[SerializeField]
	private UIInstantiator freeBoostButtonAd;

	[SerializeField]
	private UIInstantiator freeBoostButtonWithCoins;

	[SerializeField]
	private UIElement goalBar;

	[SerializeField]
	private GameObject saveKittensGoalObjective;

	[SerializeField]
	private UILabel goalLabel;

	private readonly FiberRunner fiberRunner = new FiberRunner();

	private Fiber animFiber = new Fiber();

	private Fiber requestingVideoFiber = new Fiber();

	private bool didDismiss;

	private bool gotFreeBee;

	private bool userInputBlocked;

	private GameObject goal;

	private LevelSession session;

	private CurrencyOverlay currencyOverlay;

	private bool isTimerPaused;

	private bool wasBought;

	private ButtonWithSpinner spinnerButtonWatchAd;

	private ButtonWithTitle buyButton;

	private string rewardedVideoPlacement = "LevelObjective";

	public enum Result
	{
		FreebieBought,
		FreebieFromAd,
		NoFreeBeeObtained
	}
}
