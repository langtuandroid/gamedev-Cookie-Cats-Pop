using System;
using TactileModules.Ads.Analytics;
using TactileModules.CrossPromotion.Cloud.DataInterfaces;

namespace TactileModules.CrossPromotion.General.PromotedGameUtility
{
	public class PromotedGameLauncher : IPromotedGameLauncher
	{
		public PromotedGameLauncher(IPromotedGameUtility promotedGameUtility)
		{
			this.promotedGameUtility = promotedGameUtility;
		}

		public void SendToStoreOrLaunchGame(IPromotedGameUtilityData utilityData, AdGroupContext adGroupContext, IPromotedGameUtilityAdditionalData additionalData, Action onComplete)
		{
			if (this.promotedGameUtility.IsGameInstalled(utilityData))
			{
				this.promotedGameUtility.LaunchGame(utilityData);
				if (onComplete != null)
				{
					onComplete();
				}
			}
			else
			{
				this.promotedGameUtility.SendToStore(utilityData, adGroupContext, additionalData, onComplete);
			}
		}

		public bool IsGameInstalled(IPromotedGameUtilityPrimaryData utilityData)
		{
			return this.promotedGameUtility.IsGameInstalled(utilityData);
		}

		public void LogAdjustImpression(IPromotedGameUtilityData utilityData, AdGroupContext adGroupContext, IPromotedGameUtilityAdditionalData additionalData)
		{
			this.promotedGameUtility.LogAdjustImpression(utilityData, adGroupContext, additionalData);
		}

		private readonly IPromotedGameUtility promotedGameUtility;
	}
}
