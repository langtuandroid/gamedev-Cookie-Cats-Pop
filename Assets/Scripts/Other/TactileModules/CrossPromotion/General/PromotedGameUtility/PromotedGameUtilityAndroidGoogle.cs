using System;
using TactileModules.Ads.Analytics;
using TactileModules.CrossPromotion.Cloud.DataInterfaces;

namespace TactileModules.CrossPromotion.General.PromotedGameUtility
{
	public class PromotedGameUtilityAndroidGoogle : PromotedGameUtilityAndroid
	{
		public PromotedGameUtilityAndroidGoogle(string currentGameId) : base(currentGameId)
		{
		}

		protected override string AdvertisementPlatform
		{
			get
			{
				return "gps_adid";
			}
		}

		protected override string AdvertisementId
		{
			get
			{
				return SystemInfoHelper.AdvertisingId;
			}
		}

		public override void SendToStore(IPromotedGameUtilityData utilityData, AdGroupContext adGroupContext, IPromotedGameUtilityAdditionalData additionalData, Action onComplete)
		{
			string storeUrl = "market://details?id=" + utilityData.PackageName;
			if (!string.IsNullOrEmpty(storeUrl))
			{
				base.LogAdjustStoreInternal(utilityData.TrackerToken, adGroupContext, additionalData, delegate
				{
					URLHelper.OpenURL(storeUrl);
					onComplete();
				});
			}
			else
			{
				onComplete();
			}
		}

		public override void LogAdjustImpression(IPromotedGameUtilityData utilityData, AdGroupContext adGroupContext, IPromotedGameUtilityAdditionalData additionalData)
		{
			base.LogAdjustImpressionInternal(utilityData.TrackerToken, adGroupContext, additionalData);
		}
	}
}
