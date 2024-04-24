using System;
using System.Collections;
using Fibers;
using TactileModules.RuntimeTools;
using TactileModules.SpecialOffers.Analytics;
using TactileModules.SpecialOffers.Model;
using TactileModules.SpecialOffers.Views;

namespace TactileModules.SpecialOffers.Controllers
{
	public class SpecialOfferIapViewController : SpecialOfferViewController
	{
		public SpecialOfferIapViewController(IViewPresenter viewPresenter, ISpecialOfferViewFactory specialOfferViewFactory, IAnalyticsReporter analyticsReporter, ISpecialOffer specialOffer)
		{
			this.viewPresenter = viewPresenter;
			this.specialOfferViewFactory = specialOfferViewFactory;
			this.specialOffer = specialOffer;
			this.analyticsReporter = analyticsReporter;
		}

		public override IEnumerator ShowView(EnumeratorResult<PurchaseData> purchaseDataResult)
		{
			this.view = this.specialOfferViewFactory.CreateIAPView(this.specialOffer);
			this.SetupView(this.view);
			this.view.OnAcceptButtonClicked += this.OnPurchaseClicked;
			this.view.OnDismissButtonClicked += this.OnCloseClicked;
			this.viewPresenter.ShowViewInstance<ISpecialOfferIapView>(this.view, new object[0]);
			while (!this.closeView)
			{
				yield return null;
			}
			purchaseDataResult.value = this.purchaseData;
			this.view.Close((!purchaseDataResult.value.purchaseSuccessful) ? 0 : 1);
			yield break;
		}

		private void OnPurchaseClicked()
		{
			this.analyticsReporter.LogSpecialOfferBuyStarted(this.specialOffer.FeatureInstanceId);
		}

		private void OnPurchaseCompleted(PurchaseData purchaseData)
		{
			this.purchaseData = purchaseData;
			if (this.purchaseData.purchaseSuccessful)
			{
				this.analyticsReporter.LogSpecialOfferBuyPurchased(this.specialOffer.FeatureInstanceId);
				this.closeView = true;
			}
			else
			{
				this.analyticsReporter.LogSpecialOfferBuyAborted(this.specialOffer.FeatureInstanceId);
			}
		}

		private void OnCloseClicked()
		{
			this.closeView = true;
		}

		private void SetupView(ISpecialOfferIapView view)
		{
			view.SetTexture(this.specialOffer.LoadTexture());
			view.SetPriceNow(this.specialOffer.GetPriceNow());
			view.SetPriceBefore(this.specialOffer.GetPriceBefore());
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

		private readonly IAnalyticsReporter analyticsReporter;

		private ISpecialOfferIapView view;

		private bool closeView;

		private PurchaseData purchaseData = new PurchaseData();
	}
}
