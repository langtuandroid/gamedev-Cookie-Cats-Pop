using System;
using System.Collections;
using Tactile;
using TactileModules.Ads;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.TactileCloud.TargetingParameters;

namespace TactileModules.PuzzleGames.Common.TargetingParameters
{
	public class PuzzleTargetingParametersProvider : ITargetingParametersProvider
	{
		public PuzzleTargetingParametersProvider(IMainProgression mainProgression, IUserSettings userSettings, IInterstitialPresenterDataProvider interstitialPresenterDataProvider, IAnalyticsAdjustAttribution analyticsAdjustAttribution)
		{
			this.mainProgression = mainProgression;
			this.interstitialPresenterDataProvider = interstitialPresenterDataProvider;
			this.analyticsAdjustAttribution = analyticsAdjustAttribution;
			this.userSettings = userSettings;
		}

		protected virtual bool IsPayingUser()
		{
			return this.userSettings.GetSettings<InAppPurchaseManager.PersistableState>().IsPayingUser;
		}

		public Hashtable GetAdditionalTargetingParameters()
		{
			Hashtable hashtable = new Hashtable
			{
				{
					"levelId",
					this.mainProgression.GetFarthestUnlockedLevelHumanNumber()
				},
				{
					"isPayingUser",
					this.IsPayingUser()
				},
				{
					"totalInterstitialsShown",
					this.interstitialPresenterDataProvider.TotalInterstitialsShown
				}
			};
			
			return hashtable;
		}

		private readonly IMainProgression mainProgression;

		private readonly IInterstitialPresenterDataProvider interstitialPresenterDataProvider;

		private readonly IAnalyticsAdjustAttribution analyticsAdjustAttribution;

		private readonly IUserSettings userSettings;
	}
}
