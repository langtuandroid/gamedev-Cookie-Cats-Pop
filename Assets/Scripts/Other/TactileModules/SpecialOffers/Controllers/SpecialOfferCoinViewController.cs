using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using TactileModules.RuntimeTools;
using TactileModules.SpecialOffers.Analytics;
using TactileModules.SpecialOffers.Model;
using TactileModules.SpecialOffers.Views;

namespace TactileModules.SpecialOffers.Controllers
{
	public class SpecialOfferCoinViewController : SpecialOfferViewController
	{
		public SpecialOfferCoinViewController(IViewPresenter viewPresenter, ISpecialOfferViewFactory specialOfferViewFactory, IAnalyticsReporter analyticsReporter, ISpecialOffer specialOffer, IShopManager shopManager)
		{
			this.viewPresenter = viewPresenter;
			this.specialOfferViewFactory = specialOfferViewFactory;
			this.specialOffer = specialOffer;
			this.shopManager = shopManager;
			this.analyticsReporter = analyticsReporter;
		}

		public override IEnumerator ShowView(EnumeratorResult<PurchaseData> purchaseDataResult)
		{
			this.view = this.specialOfferViewFactory.CreateCoinView(this.specialOffer);
			this.SetupView(this.view);
			this.view.OnAcceptButtonClicked += this.OnPurchaseClicked;
			this.view.OnDismissButtonClicked += this.OnCloseClicked;
			this.viewPresenter.ShowViewInstance<ISpecialOfferView>(this.view, new object[0]);
			while (!this.closeView)
			{
				yield return null;
			}
			purchaseDataResult.value = this.purchaseData;
			this.view.Close((!this.purchaseData.purchaseSuccessful) ? 0 : 1);
			yield break;
		}

		private void OnPurchaseClicked()
		{
			this.analyticsReporter.LogSpecialOfferBuyStarted(this.specialOffer.FeatureInstanceId);
			ShopItem shopItem = new ShopItem
			{
				Type = "SpecialOfferCoins",
				CurrencyPrice = this.specialOffer.GetCoinPrice(),
				Rewards = new List<ItemAmount>(),
				IAPIdentifier = string.Empty,
				CustomTag = string.Empty
			};
			this.shopManager.TrySpendCoins(shopItem, this.view, delegate(bool succes)
			{
				if (succes)
				{
					this.purchaseData.purchaseSuccessful = true;
					this.analyticsReporter.LogSpecialOfferBuyPurchased(this.specialOffer.FeatureInstanceId);
					this.closeView = true;
				}
				else
				{
					this.analyticsReporter.LogSpecialOfferBuyAborted(this.specialOffer.FeatureInstanceId);
				}
			});
		}

		private void OnCloseClicked()
		{
			this.closeView = true;
		}

		private void SetupView(ISpecialOfferView view)
		{
			view.SetTexture(this.specialOffer.LoadTexture());
			view.SetTimeLeft(this.specialOffer.GetTimeLeft());
			UISafeTimer uisafeTimer = new UISafeTimer(view.gameObject, new Action(this.UpdateTimerText), 0.1f);
			uisafeTimer.Run();
		}

		private void UpdateTimerText()
		{
			this.view.SetTimeLeft(this.specialOffer.GetTimeLeft());
		}

		private readonly IViewPresenter viewPresenter;

		private readonly ISpecialOfferViewFactory specialOfferViewFactory;

		private readonly ISpecialOffer specialOffer;

		private readonly IShopManager shopManager;

		private readonly IAnalyticsReporter analyticsReporter;

		private ISpecialOfferView view;

		private bool closeView;

		private PurchaseData purchaseData = new PurchaseData();
	}
}
