using System;
using System.Collections;
using TactileModules.Ads.Analytics;
using TactileModules.CrossPromotion.General.Ads.AdModels;
using TactileModules.CrossPromotion.Interstitials.ViewFactories;
using TactileModules.CrossPromotion.Interstitials.Views;

namespace TactileModules.CrossPromotion.Interstitials.ViewControllers
{
	public class InterstitialViewController : IInterstitialViewController
	{
		public InterstitialViewController(ICrossPromotionAdRetriever crossPromotionAdRetriever, ICrossPromotionAdUpdater crossPromotionAdUpdater, IViewPresenter viewPresenter, ICrossPromotionInterstitialViewFactory viewFactory)
		{
			this.crossPromotionAdRetriever = crossPromotionAdRetriever;
			this.crossPromotionAdUpdater = crossPromotionAdUpdater;
			this.viewPresenter = viewPresenter;
			this.viewFactory = viewFactory;
		}

		public bool ShowViewIfPossible(AdGroupContext adGroupContext)
		{
			ICrossPromotionAd crossPromotionAd = this.crossPromotionAdRetriever.GetPresentablePromotion();
			if (crossPromotionAd == null)
			{
				this.crossPromotionAdUpdater.UpdateCrossPromotionAd();
				return false;
			}
			this.crossPromotionAdRetriever.RequestNewPromotion();
			this.isShowing = true;
			ICrossPromotionInterstitialView view = this.viewFactory.CreateInterstitialView();
			view.Initialize(crossPromotionAd);
			view.Closed += delegate()
			{
				crossPromotionAd.ReportAsClosed(adGroupContext);
				this.isShowing = false;
				view.Close(0);
			};
			view.ClickedAd += delegate()
			{
				crossPromotionAd.ReportAsClicked(adGroupContext);
				IUIView spinnerView = this.viewFactory.ShowSpinnerView();
				crossPromotionAd.SendToStoreOrLaunchGame(adGroupContext, delegate
				{
					spinnerView.Close(0);
				});
			};
			this.viewPresenter.ShowViewInstance<ICrossPromotionInterstitialView>(view, new object[0]);
			crossPromotionAd.ReportAsShown(adGroupContext);
			return true;
		}

		public IEnumerator WaitForClose()
		{
			while (this.isShowing)
			{
				yield return null;
			}
			yield break;
		}

		private readonly ICrossPromotionAdRetriever crossPromotionAdRetriever;

		private readonly ICrossPromotionAdUpdater crossPromotionAdUpdater;

		private readonly IViewPresenter viewPresenter;

		private readonly ICrossPromotionInterstitialViewFactory viewFactory;

		private bool isShowing;
	}
}
