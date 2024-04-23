using System;
using TactileModules.Ads.Analytics;
using TactileModules.CrossPromotion.Cloud.DataInterfaces;

namespace TactileModules.CrossPromotion.General.PromotedGameUtility
{
	public interface IPromotedGameUtility
	{
		bool IsGameInstalled(IPromotedGameUtilityPrimaryData utilityData);

		void LaunchGame(IPromotedGameUtilityPrimaryData utilityData);

		void SendToStore(IPromotedGameUtilityData utilityData, AdGroupContext adGroupContext, IPromotedGameUtilityAdditionalData additionalData, Action onComplete);

		void LogAdjustImpression(IPromotedGameUtilityData utilityData, AdGroupContext adGroupContext, IPromotedGameUtilityAdditionalData additionalData);
	}
}
