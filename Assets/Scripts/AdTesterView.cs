using System;
using TactileModules.Ads;
using TactileModules.Foundation;

public class AdTesterView : UIView
{
	private IInterstitialPresenter InterstitialPresenter
	{
		get
		{
			return ManagerRepository.Get<InterstitialPresenter>();
		}
	}

	private IRewardedVideoPresenter RewardedVideoPresenter
	{
		get
		{
			return ManagerRepository.Get<RewardedVideoPresenter>();
		}
	}

	private void ButtonCloseClicked(UIEvent e)
	{
		base.Close(0);
	}

	private void FatchInterstitialAndShowClicked(UIEvent e)
	{
		FiberCtrl.Pool.Run(this.InterstitialPresenter.FetchAndShowInterstitial(10), false);
	}

	private void RequestAndShowRewardedVideoClicked(UIEvent e)
	{
		RewardedVideoParameters data = new RewardedVideoParameters("Debug", string.Empty, 0);
		FiberCtrl.Pool.Run(this.RewardedVideoPresenter.FetchAndShowRewardedVideo(data, delegate(bool videoCompleted)
		{
			UIViewManager.Instance.ShowView<MessageBoxView>(new object[]
			{
				"Video completed",
				"Completed: " + videoCompleted,
				"OK"
			});
		}, 10), false);
	}

	public void ShowMediationDebugger(UIEvent e)
	{
		MaxSdkAndroid.ShowMediationDebugger();
	}
}
