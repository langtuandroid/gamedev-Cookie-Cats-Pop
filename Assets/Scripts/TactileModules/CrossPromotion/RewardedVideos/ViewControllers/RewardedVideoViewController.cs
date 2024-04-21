using System;
using System.Collections;
using Fibers;
using TactileModules.Ads.Analytics;
using TactileModules.Analytics.Interfaces;
using TactileModules.CrossPromotion.Analytics;
using TactileModules.CrossPromotion.General.Ads.AdModels;
using TactileModules.CrossPromotion.RewardedVideos.ViewFactories;
using TactileModules.CrossPromotion.RewardedVideos.Views;
using TactileModules.RuntimeTools;
using TactileModules.TactileLogger;

namespace TactileModules.CrossPromotion.RewardedVideos.ViewControllers
{
	public class RewardedVideoViewController : IRewardedVideoViewController
	{
		public RewardedVideoViewController(ICrossPromotionAdRetriever videoRetriever, ICrossPromotionAdUpdater videoUpdater, IRewardedVideoViewFactory viewFactory, IViewPresenter viewPresenter, ICrossPromotionAnalyticsEventFactory analyticsEventFactory, IAnalytics analytics, IFiber dialogFiber, ITactileDateTime dateTime)
		{
			this.viewFactory = viewFactory;
			this.viewPresenter = viewPresenter;
			this.videoRetriever = videoRetriever;
			this.videoUpdater = videoUpdater;
			this.analyticsEventFactory = analyticsEventFactory;
			this.analytics = analytics;
			this.dialogFiber = dialogFiber;
			this.dateTime = dateTime;
		}

		public bool ShowViewIfPossible(AdGroupContext adGroupContext)
		{
			ICrossPromotionAd crossPromotionAd = this.videoRetriever.GetPresentablePromotion();
			if (crossPromotionAd == null)
			{
				this.videoUpdater.UpdateCrossPromotionAd();
				return false;
			}
			this.videoRetriever.RequestNewPromotion();
			this.isShowing = true;
			this.videoAborted = false;
			this.rewatchVideo = false;
			this.videoView = this.viewFactory.CreateRewardedVideoView();
			this.videoView.Closed += delegate()
			{
				crossPromotionAd.ReportAsClosed(adGroupContext);
				this.isShowing = false;
				this.videoView.Close(0);
			};
			this.videoView.ClickedAd += delegate()
			{
				crossPromotionAd.ReportAsClicked(adGroupContext);
				IUIView spinnerView = this.viewFactory.ShowSpinnerView();
				crossPromotionAd.SendToStoreOrLaunchGame(adGroupContext, delegate
				{
					spinnerView.Close(0);
				});
			};
			this.videoView.OnVideoEnded += delegate()
			{
				this.dialogFiber.Start(this.VideoEnded(crossPromotionAd, adGroupContext));
			};
			this.videoView.OnVideoFreeze += delegate(int videoFrame)
			{
				this.LogVideoFreeze(crossPromotionAd, videoFrame);
			};
			this.viewPresenter.ShowViewInstance<IRewardedVideoView>(this.videoView, new object[0]);
			crossPromotionAd.ReportAsShown(adGroupContext);
			this.overlayView = this.viewFactory.CreateRewardedVideoOverlayView();
			this.viewPresenter.ShowViewInstance<ICrossPromotionVideoOverlayView>(this.overlayView, new object[0]);
			this.videoStartTime = this.dateTime.UtcNow;
			this.videoView.Initialize(crossPromotionAd, this.overlayView);
			return true;
		}

		private IEnumerator VideoEnded(ICrossPromotionAd crossPromotionAd, AdGroupContext adGroupContext)
		{
			yield return FiberHelper.WaitForFrames(5, (FiberHelper.WaitFlag)0);
			DateTime utcNow = this.dateTime.UtcNow;
			double elapsedTime = (utcNow - this.videoStartTime).TotalMilliseconds;
			int duration = crossPromotionAd.GetVideoCreative().Duration;
			Log.Info(Category.CrossPromotion, () => string.Format("VideoStart: {0}, VideoEnd: {1}, TimeElapsed: {2}, Duration: {3}", new object[]
			{
				this.videoStartTime.Ticks,
				utcNow.Ticks,
				elapsedTime,
				duration
			}), null);
			if (elapsedTime < (double)(duration - 5000))
			{
				yield return this.VideoWasAborted();
			}
			else
			{
				crossPromotionAd.ReportAsCompletedWatching(adGroupContext);
				IUIView spinnerView = this.viewFactory.ShowSpinnerView();
				crossPromotionAd.OpenEmbeddedStoreIfPossible(adGroupContext, delegate
				{
					spinnerView.Close(0);
				});
			}
			yield break;
		}

		private IEnumerator VideoWasAborted()
		{
			this.videoAborted = true;
			IUIView dialogView = this.viewFactory.ShowVideoAbortedDialogView();
			while (dialogView.ClosingResult == null)
			{
				yield return null;
			}
			int closingResult = (int)dialogView.ClosingResult;
			if (closingResult == 1)
			{
				this.rewatchVideo = true;
				this.isShowing = false;
				this.videoView.Close(0);
				this.overlayView.Close(0);
			}
			yield break;
		}

		private void LogVideoFreeze(ICrossPromotionAd crossPromotionAd, int videoFrame)
		{
			this.analytics.LogEvent(this.analyticsEventFactory.CreateCrossPromotionRewardedVideoFreezeEvent(crossPromotionAd, videoFrame), -1.0, null);
		}

		public IEnumerator WaitForClose()
		{
			while (this.isShowing)
			{
				yield return null;
			}
			yield break;
		}

		public bool IsShowing()
		{
			return this.isShowing;
		}

		public bool WasVideoAborted()
		{
			return this.videoAborted;
		}

		public bool ShouldRewatchVideo()
		{
			return this.rewatchVideo;
		}

		private const int DURATION_VARAINCE_TIME = 5000;

		private readonly IRewardedVideoViewFactory viewFactory;

		private readonly IViewPresenter viewPresenter;

		private readonly ICrossPromotionAdRetriever videoRetriever;

		private readonly ICrossPromotionAdUpdater videoUpdater;

		private readonly ICrossPromotionAnalyticsEventFactory analyticsEventFactory;

		private readonly IAnalytics analytics;

		private readonly IFiber dialogFiber;

		private readonly ITactileDateTime dateTime;

		private bool isShowing;

		private bool videoAborted;

		private bool rewatchVideo;

		private IRewardedVideoView videoView;

		private ICrossPromotionVideoOverlayView overlayView;

		private DateTime videoStartTime;
	}
}
