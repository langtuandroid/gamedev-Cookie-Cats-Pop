using System;
using TactileModules.CrossPromotion.RewardedVideos.Views;
using TactileModules.CrossPromotion.TemplateAssets.GeneratedScript;
using TactileModules.NinjaUi.SharedViewControllers;

namespace TactileModules.CrossPromotion.RewardedVideos.ViewFactories
{
	public class RewardedVideoViewFactory : IRewardedVideoViewFactory
	{
		public RewardedVideoViewFactory(IViewFactory viewFactory, ISpinnerViewController spinnerViewController, IBasicDialogViewController dialogViewController)
		{
			this.viewFactory = viewFactory;
			this.spinnerViewController = spinnerViewController;
			this.dialogViewController = dialogViewController;
		}

		public IRewardedVideoView CreateRewardedVideoView()
		{
			return this.viewFactory.CreateCrossPromotionVideoView();
		}

		public ICrossPromotionVideoOverlayView CreateRewardedVideoOverlayView()
		{
			return this.viewFactory.CreateCrossPromotionVideoOverlayView();
		}

		public IUIView ShowSpinnerView()
		{
			return this.spinnerViewController.ShowSpinnerView(L.Get("Please wait"));
		}

		public IUIView ShowVideoAbortedDialogView()
		{
			return this.dialogViewController.ShowBasicDialogView(L.Get("Video aborted"), L.Get("The video was aborted, no reward will be given"), L.Get("Ok"), L.Get("Watch again"));
		}

		private readonly IViewFactory viewFactory;

		private readonly ISpinnerViewController spinnerViewController;

		private readonly IBasicDialogViewController dialogViewController;
	}
}
