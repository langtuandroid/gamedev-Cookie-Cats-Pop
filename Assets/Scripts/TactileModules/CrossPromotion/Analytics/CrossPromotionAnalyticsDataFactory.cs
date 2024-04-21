using System;
using TactileModules.Ads.Analytics;
using TactileModules.CrossPromotion.Analytics.Data;
using TactileModules.CrossPromotion.Cloud.Data;
using TactileModules.CrossPromotion.Cloud.DataInterfaces;
using TactileModules.CrossPromotion.General;
using TactileModules.CrossPromotion.General.Ads.AdModels;
using TactileModules.RuntimeTools.Orientation;

namespace TactileModules.CrossPromotion.Analytics
{
	public class CrossPromotionAnalyticsDataFactory : ICrossPromotionAnalyticsDataFactory
	{
		public CrossPromotionAnalyticsDataFactory(IScreenOrientationGetter screenOrientationGetter, IAdCreativeSelector adCreativeSelector)
		{
			this.screenOrientationGetter = screenOrientationGetter;
			this.adCreativeSelector = adCreativeSelector;
		}

		public CrossPromotionAnalyticsData CreateData(AdType type, CrossPromotionAdMetaData data, AdGroupContext adGroupContext)
		{
			return new CrossPromotionAnalyticsData
			{
				RequestId = data.RequestId,
				CampaignId = data.CampaignId,
				CrossPromotionGame = CrossPromotionAnalyticsDataFactory.GetGameName(data.PackageName),
				CrossPromotionType = type.ToString(),
				Location = adGroupContext.ToString(),
				ImageId = this.GetCreativeId(data.AssetImage),
				ImageName = this.GetCreativeName(data.AssetImage),
				ImageResolution = this.GetCreativeResolution(data.AssetImage),
				VideoId = this.GetCreativeId(data.AssetVideo),
				VideoName = this.GetCreativeName(data.AssetVideo),
				VideoResolution = this.GetCreativeResolution(data.AssetVideo),
				Orientation = this.screenOrientationGetter.GetOrientation().ToString()
			};
		}

		private string GetCreativeId(ICrossPromotionAdAssetMetaData adAssetMetaData)
		{
			if (adAssetMetaData == null)
			{
				return null;
			}
			CrossPromotionAdCreative creativeForOrientation = this.adCreativeSelector.GetCreativeForOrientation(adAssetMetaData);
			if (creativeForOrientation != null)
			{
				return creativeForOrientation.Id;
			}
			return null;
		}

		private string GetCreativeName(ICrossPromotionAdAssetMetaData adAssetMetaData)
		{
			if (adAssetMetaData == null)
			{
				return null;
			}
			CrossPromotionAdCreative creativeForOrientation = this.adCreativeSelector.GetCreativeForOrientation(adAssetMetaData);
			if (creativeForOrientation != null)
			{
				return creativeForOrientation.Name;
			}
			return null;
		}

		private string GetCreativeResolution(ICrossPromotionAdAssetMetaData adAssetMetaData)
		{
			if (adAssetMetaData == null)
			{
				return null;
			}
			CrossPromotionAdCreative creativeForOrientation = this.adCreativeSelector.GetCreativeForOrientation(adAssetMetaData);
			if (creativeForOrientation != null)
			{
				return creativeForOrientation.AssetResolution.Width + "x" + creativeForOrientation.AssetResolution.Height;
			}
			return null;
		}

		private static string GetGameName(string packageName)
		{
			int num = packageName.LastIndexOf('.');
			return packageName.Substring(num + 1);
		}

		private readonly IScreenOrientationGetter screenOrientationGetter;

		private readonly IAdCreativeSelector adCreativeSelector;
	}
}
