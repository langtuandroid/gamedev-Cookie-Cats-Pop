using System;
using System.Collections;
using TactileModules.Ads;
using TactileModules.Placements;

namespace TactileModules.PuzzleGame.Ads
{
	public class InterstitialPlacementRunnable : IPlacementRunnableNoBreak, IPlacementRunnable
	{
		public InterstitialPlacementRunnable(IInterstitialPresenter interstitialPresenter)
		{
			this.interstitialPresenter = interstitialPresenter;
		}

		public string ID
		{
			get
			{
				return "Interstitial";
			}
		}

		public IEnumerator Run(IPlacementViewMediator placementViewMediator)
		{
			yield return this.interstitialPresenter.ShowInterstitial();
			yield break;
		}

		private readonly IInterstitialPresenter interstitialPresenter;
	}
}
