using System;
using Fibers;
using TactileModules.Analytics.Interfaces;
using TactileModules.CrossPromotion.Analytics;
using TactileModules.CrossPromotion.General.Ads.AdModels;
using TactileModules.CrossPromotion.RewardedVideos.ViewFactories;
using TactileModules.RuntimeTools;

namespace TactileModules.CrossPromotion.RewardedVideos.ViewControllers
{
	public class RewardedVideoControllerFactory : IRewardedVideoControllerFactory
	{
		public RewardedVideoControllerFactory(ICrossPromotionAdRetriever videoRetriever, ICrossPromotionAdUpdater videoUpdater, IRewardedVideoViewFactory viewFactory, IViewPresenter viewPresenter, ICrossPromotionAnalyticsEventFactory analyticsEventFactory, IAnalytics analytics, IFiber dialogFiber, ITactileDateTime tactileDateTime)
		{
			this.videoRetriever = videoRetriever;
			this.videoUpdater = videoUpdater;
			this.viewFactory = viewFactory;
			this.viewPresenter = viewPresenter;
			this.analyticsEventFactory = analyticsEventFactory;
			this.analytics = analytics;
			this.dialogFiber = dialogFiber;
			this.tactileDateTime = tactileDateTime;
		}

		public IRewardedVideoViewController CreateViewController()
		{
			return new RewardedVideoViewController(this.videoRetriever, this.videoUpdater, this.viewFactory, this.viewPresenter, this.analyticsEventFactory, this.analytics, this.dialogFiber, this.tactileDateTime);
		}

		private readonly ICrossPromotionAdRetriever videoRetriever;

		private readonly ICrossPromotionAdUpdater videoUpdater;

		private readonly IRewardedVideoViewFactory viewFactory;

		private readonly IViewPresenter viewPresenter;

		private readonly ICrossPromotionAnalyticsEventFactory analyticsEventFactory;

		private readonly IAnalytics analytics;

		private readonly IFiber dialogFiber;

		private readonly ITactileDateTime tactileDateTime;
	}
}
