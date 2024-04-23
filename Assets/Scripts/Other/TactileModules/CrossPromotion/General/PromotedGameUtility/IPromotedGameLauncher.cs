using System;
using TactileModules.Ads.Analytics;
using TactileModules.CrossPromotion.Cloud.DataInterfaces;

namespace TactileModules.CrossPromotion.General.PromotedGameUtility
{
	public interface IPromotedGameLauncher
	{
		void SendToStoreOrLaunchGame(IPromotedGameUtilityData utilityData, AdGroupContext adGroupContext, IPromotedGameUtilityAdditionalData additionalData, Action onComplete);

		bool IsGameInstalled(IPromotedGameUtilityPrimaryData utilityData);

		void LogAdjustImpression(IPromotedGameUtilityData utilityData, AdGroupContext adGroupContext, IPromotedGameUtilityAdditionalData additionalData);
	}
}
