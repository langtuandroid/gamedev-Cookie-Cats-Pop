using System;
using System.Collections;
using TactileModules.Ads;
using TactileModules.CrossPromotion.General.Ads.AdModels;
using TactileModules.CrossPromotion.Interstitials.ViewControllers;

namespace TactileModules.CrossPromotion.Interstitials
{
	public class InterstitialProvider : IInterstitialProvider
	{
		public InterstitialProvider(ICrossPromotionAdRetriever crossPromotionAdRetriever, ICrossPromotionAdUpdater crossPromotionAdUpdater, IInterstitialControllerFactory controllerFactory)
		{
			this.crossPromotionAdRetriever = crossPromotionAdRetriever;
			this.crossPromotionAdUpdater = crossPromotionAdUpdater;
			this.controllerFactory = controllerFactory;
		}

		public bool IsInterstitialAvailable
		{
			get
			{
				return this.crossPromotionAdRetriever.GetPresentablePromotion() != null;
			}
		}

		public bool IsRequestingInterstitial
		{
			get
			{
				return this.crossPromotionAdRetriever.IsRequesting();
			}
		}

		public void RequestInterstitial()
		{
			this.crossPromotionAdUpdater.UpdateCrossPromotionAd();
		}

		public int Priority
		{
			get
			{
				return 1;
			}
		}

		public IEnumerator ShowInterstitial()
		{
			IInterstitialViewController viewController = this.controllerFactory.CreateViewController();
			viewController.ShowViewIfPossible("Popup");
			yield return viewController.WaitForClose();
			yield break;
		}

		private readonly IInterstitialControllerFactory controllerFactory;

		private readonly ICrossPromotionAdRetriever crossPromotionAdRetriever;

		private readonly ICrossPromotionAdUpdater crossPromotionAdUpdater;
	}
}
