using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Fibers;
using JetBrains.Annotations;
using Tactile;
using TactileModules.Ads;
using TactileModules.Foundation;
using UnityEngine;

public class ShopView : UIView
{
	private InAppPurchaseManager InAppPurchaseManager
	{
		get
		{
			return ManagerRepository.Get<InAppPurchaseManager>();
		}
	}

	private IRewardedVideoPresenter RewardedVideoPresenter
	{
		get
		{
			return ManagerRepository.Get<RewardedVideoPresenter>();
		}
	}

	protected override void ViewLoad(object[] parameters)
	{
		this.neededCoinsForPurchase = (int)parameters[0];
		this.spinnerButtonWatchAd = this.watchAdInstantiator.GetInstance<ButtonWithSpinner>();
		this.RequestVideo();
	}

	protected override void ViewWillAppear()
	{
		bool active = !UserSettingsManager.Get<InAppPurchaseManager.PersistableState>().IsPayingUser;
		this.anyPurchaseRemovesAdsLabel.gameObject.SetActive(active);
		this.currencyOverlay = base.ObtainOverlay<CurrencyOverlay>();
		this.spinnerButtonWatchAd.PrimaryTitle = L.Get("1 [C] Watch Ad!");
		this.spinnerButtonWatchAd.LoadingTitle = L.Get("Loading...");
		this.UpdateUI();
	}

	protected override void ViewWillDisappear()
	{
		this.hasBeenClosedOrIsClosing = true;
		this.animCoinsFiber.Terminate();
		this.requestingVideoFiber.Terminate();
		this.videoFiber.Terminate();
	}

	protected override void ViewDidDisappear()
	{
		base.ReleaseOverlay<CurrencyOverlay>();
	}

	protected override void ViewGotFocus()
	{
		this.currencyOverlay.OnCoinButtonClicked = delegate()
		{
			base.Close(0);
		};
	}

	protected override void ViewLostFocus()
	{
		this.currencyOverlay.OnCoinButtonClicked = null;
	}

	private void UpdateUI()
	{
		this.UpdateCoinItemsUI();
	}

	private void RequestVideo()
	{
		this.RewardedVideoPresenter.RequestVideo();
		this.requestingVideoFiber.Start(this.WhileIsRequestingVideo());
	}

	private IEnumerator WhileIsRequestingVideo()
	{
		this.spinnerButtonWatchAd.Disabled = this.RewardedVideoPresenter.IsRequestingVideo();
		this.spinnerButtonWatchAd.Spinning = this.RewardedVideoPresenter.IsRequestingVideo();
		this.spinnerButtonWatchAd.LoadingTitle = L.Get("Loading...");
		while (this.RewardedVideoPresenter.IsRequestingVideo())
		{
			yield return null;
		}
		if (this.spinnerButtonWatchAd == null)
		{
			yield break;
		}
		this.spinnerButtonWatchAd.Disabled = !this.RewardedVideoPresenter.CanShowRewardedVideo(this.rewardedVideoPlacement);
		this.spinnerButtonWatchAd.Spinning = this.RewardedVideoPresenter.IsRequestingVideo();
		this.spinnerButtonWatchAd.LoadingTitle = "No Video available";
		yield break;
	}

	private void UpdateCoinItemsUI()
	{
		if (this.hasBeenClosedOrIsClosing)
		{
			return;
		}
		this.InitializeStaticList();
	}

	private void InitializeStaticList()
	{
		this.staticContentRoot.gameObject.SetActive(true);
		if (!this.isStaticListInitialized)
		{
			int num = 0;
			foreach (ShopItem shopItem in ShopManager.Instance.ShopItemsWithIAPForItems("Coin"))
			{
				if (shopItem.Type.Contains("CoinPack"))
				{
					num++;
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.coinElementPrefab);
					gameObject.SetLayerRecursively(base.gameObject.layer);
					gameObject.SetActive(true);
					ShopViewItem component = gameObject.GetComponent<ShopViewItem>();
					component.Initialize(shopItem, false);
					component.OnClick += this.HandleCoinItemClicked;
					component.name = num.ToString();
					if (shopItem.CustomTag.Contains("popular"))
					{
						component.ShowMostPopularSticker();
					}
					component.transform.parent = this.staticGridLayout.transform;
					component.transform.localPosition = Vector3.zero;
				}
			}
			this.isStaticListInitialized = true;
		}
	}

	[UsedImplicitly]
	private void WatchAdClicked(UIEvent e)
	{
		this.videoFiber.Start(this.WatchAdCr());
	}

	private void HandleCoinItemClicked(ShopViewItem item)
	{
		FiberCtrl.Pool.Run(this.PurchaseShopItem(item), false);
	}

	private IEnumerator WatchAdCr()
	{
		RewardedVideoParameters videoParameters = new RewardedVideoParameters(this.rewardedVideoPlacement, "Coin", 1);
		yield return this.RewardedVideoPresenter.ShowRewardedVideo(videoParameters, delegate(bool didComplete)
		{
			if (didComplete)
			{
				this.currencyOverlay.coinButton.PauseRefreshingCoins(true);
				foreach (ItemAmount itemAmount in ConfigurationManager.Get<ShopConfig>().RewardForWatchingAd)
				{
					InventoryManager.Instance.Add(itemAmount.ItemId, itemAmount.Amount, "shopViewVideoAd");
					if (itemAmount.ItemId == "Coin")
					{
						this.AnimatePurchaseSuccess(itemAmount.Amount, this.spinnerButtonWatchAd.transform.position + Vector3.back * 50f);
					}
				}
				GameEventManager.Instance.Emit(31);
			}
			this.RequestVideo();
		});
		yield break;
	}

	private IEnumerator PurchaseShopItem(ShopViewItem item)
	{
		this.currencyOverlay.coinButton.PauseRefreshingCoins(true);
		ShopItem shopItem = item.GetShopItem();
		InAppProduct inAppProduct = this.InAppPurchaseManager.GetProductForIdentifier(shopItem.FullIAPIdentifier);
		bool success = false;
		if (inAppProduct != null)
		{
			yield return this.InAppPurchaseManager.DoInAppPurchase(inAppProduct, delegate(string purchaseSessionId, string transactionId, InAppPurchaseStatus resultStatus)
			{
				success = (resultStatus == InAppPurchaseStatus.Succeeded);
			});
		}
		if (success)
		{
			SingletonAsset<UISetup>.Instance.purchaseSuccessful.Play();
			UICamera.DisableInput();
			if (ShopView._003C_003Ef__mg_0024cache0 == null)
			{
				ShopView._003C_003Ef__mg_0024cache0 = new Fiber.OnExitHandler(UICamera.EnableInput);
			}
			yield return new Fiber.OnExit(ShopView._003C_003Ef__mg_0024cache0);
			yield return this.AnimateCoins(5, item.GetCoinSpawnPos());
			UIViewManager.UIViewStateGeneric<PurchaseAcknowledgementView> acknowledgementvs = UIViewManager.Instance.ShowView<PurchaseAcknowledgementView>(new object[0]);
			yield return acknowledgementvs.WaitForClose();
			this.UpdateCoinItemsUI();
			UICamera.EnableInput();
		}
		this.currencyOverlay.coinButton.PauseRefreshingCoins(false);
		yield break;
	}

	private void AnimatePurchaseSuccess(int numCoins, Vector3 spawnPos)
	{
		this.animCoinsFiber.Start(this.AnimateCoins(numCoins, spawnPos));
	}

	private void EvaluateCoinsReceived()
	{
		if (this.neededCoinsForPurchase > 0 && InventoryManager.Instance.Coins >= this.neededCoinsForPurchase)
		{
			base.Close(0);
		}
	}

	private IEnumerator AnimateCoins(int numCoins, Vector3 spawnPos)
	{
		yield return new Fiber.OnExit(delegate()
		{
			this.currencyOverlay.coinButton.PauseRefreshingCoins(false);
		});
		yield return FiberHelper.Wait(0.4f, (FiberHelper.WaitFlag)0);
		this.currencyOverlay.coinAnimator.GiveCoins(spawnPos, numCoins, 0.5f, null, null);
		yield return FiberHelper.Wait(0.6f, (FiberHelper.WaitFlag)0);
		this.EvaluateCoinsReceived();
		yield break;
	}

	private void ButtonCloseClicked(UIEvent e)
	{
		base.Close(0);
	}

	[SerializeField]
	private GameObject coinElementPrefab;

	[SerializeField]
	private UILabel anyPurchaseRemovesAdsLabel;

	[SerializeField]
	private UIInstantiator watchAdInstantiator;

	[SerializeField]
	private GameObject staticContentRoot;

	[SerializeField]
	private UIGridLayout staticGridLayout;

	private CurrencyOverlay currencyOverlay;

	private int neededCoinsForPurchase;

	private readonly Fiber animCoinsFiber = new Fiber();

	private readonly Fiber requestingVideoFiber = new Fiber();

	private readonly Fiber videoFiber = new Fiber();

	private ButtonWithSpinner spinnerButtonWatchAd;

	private bool isStaticListInitialized;

	private bool hasBeenClosedOrIsClosing;

	private string rewardedVideoPlacement = "Shop";

	[CompilerGenerated]
	private static Fiber.OnExitHandler _003C_003Ef__mg_0024cache0;
}
