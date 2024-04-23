using System;
using System.Collections.Generic;
using TactileModules.Ads;
using TactileModules.CrossPromotion.General.Ads.AdModels;
using TactileModules.CrossPromotion.Interstitials.ViewFactories;
using TactileModules.SideMapButtons;

namespace TactileModules.CrossPromotion.Interstitials.ViewControllers
{
	public class InterstitialControllerFactory : ISideMapButtonControllerProvider, IInterstitialControllerFactory
	{
		public InterstitialControllerFactory(IViewPresenter viewPresenter, ICrossPromotionInterstitialViewFactory viewFactory, ICrossPromotionAdRetriever interstitialAdRetriever, ICrossPromotionAdUpdater interstitialAdUpdater, IInterstitialPresenter interstitialPresenter)
		{
			this.viewPresenter = viewPresenter;
			this.viewFactory = viewFactory;
			this.interstitialAdRetriever = interstitialAdRetriever;
			this.interstitialAdUpdater = interstitialAdUpdater;
			this.interstitialPresenter = interstitialPresenter;
		}

		public IInterstitialViewController CreateViewController()
		{
			return new InterstitialViewController(this.interstitialAdRetriever, this.interstitialAdUpdater, this.viewPresenter, this.viewFactory);
		}

		public List<ISideMapButtonController> CreateButtonControllers()
		{
			IInterstitialViewController viewController = this.CreateViewController();
			return new List<ISideMapButtonController>
			{
				new InterstitialSideMapButtonController(this.interstitialAdRetriever, this.viewFactory, viewController, this.interstitialPresenter)
			};
		}

		private readonly ICrossPromotionInterstitialViewFactory viewFactory;

		private readonly ICrossPromotionAdRetriever interstitialAdRetriever;

		private readonly ICrossPromotionAdUpdater interstitialAdUpdater;

		private readonly IInterstitialPresenter interstitialPresenter;

		private readonly IViewPresenter viewPresenter;
	}
}
