using System;
using System.Collections;
using TactileModules.Ads;
using TactileModules.Ads.Analytics;
using TactileModules.CrossPromotion.General.Ads.AdModels;
using TactileModules.CrossPromotion.RewardedVideos.ViewControllers;

namespace TactileModules.CrossPromotion.RewardedVideos
{
	public class RewardedVideoProvider : IRewardedVideoProvider
	{
		public RewardedVideoProvider(ICrossPromotionAdRetriever videoRetriever, ICrossPromotionAdUpdater videoUpdater, IRewardedVideoControllerFactory controllerFactory)
		{
			this.videoRetriever = videoRetriever;
			this.videoUpdater = videoUpdater;
			this.controllerFactory = controllerFactory;
		}

		public bool IsVideoAvailable
		{
			get
			{
				return this.videoRetriever.GetPresentablePromotion() != null;
			}
		}

		public bool IsPreparingVideo
		{
			get
			{
				return this.videoRetriever.IsRequesting();
			}
		}

		public void RequestVideo()
		{
			this.videoUpdater.UpdateCrossPromotionAd();
		}

		public int Priority
		{
			get
			{
				return 1;
			}
		}

		public string AdProviderDisplayName
		{
			get
			{
				return "CrossPromotion";
			}
		}

		public IEnumerator ShowVideo(AdGroupContext adGroupContext, Action videoStarted, Action<bool> videoCompleted)
		{
			IRewardedVideoViewController viewController = this.controllerFactory.CreateViewController();
			bool didShow = viewController.ShowViewIfPossible(adGroupContext);
			if (didShow)
			{
				videoStarted();
				yield return viewController.WaitForClose();
				if (viewController.WasVideoAborted())
				{
					if (viewController.ShouldRewatchVideo())
					{
						yield return this.ShowVideo(adGroupContext, videoStarted, videoCompleted);
					}
					else
					{
						videoCompleted(false);
					}
				}
				else
				{
					videoCompleted(true);
				}
			}
			yield break;
		}

		private readonly ICrossPromotionAdRetriever videoRetriever;

		private readonly IRewardedVideoControllerFactory controllerFactory;

		private readonly ICrossPromotionAdUpdater videoUpdater;
	}
}
