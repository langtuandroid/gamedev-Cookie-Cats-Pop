using System;
using TactileModules.CrossPromotion.Cloud.Data;
using TactileModules.CrossPromotion.Cloud.DataInterfaces;
using TactileModules.RuntimeTools.Orientation;
using UnityEngine;

namespace TactileModules.CrossPromotion.General.Ads.AdModels
{
	public class AdCreativeSelector : IAdCreativeSelector
	{
		public AdCreativeSelector(IScreenOrientationGetter screenOrientationGetter)
		{
			this.screenOrientationGetter = screenOrientationGetter;
		}

		public CrossPromotionAdCreative GetCreativeForOrientation(ICrossPromotionAdAssetMetaData assetMetaData)
		{
			CrossPromotionAdCreative crossPromotionAdCreative;
			if (this.screenOrientationGetter.GetOrientation() == ScreenOrientation.LandscapeLeft)
			{
				crossPromotionAdCreative = assetMetaData.GetLandscape();
				if (crossPromotionAdCreative == null)
				{
					crossPromotionAdCreative = assetMetaData.GetPortrait();
				}
			}
			else
			{
				crossPromotionAdCreative = assetMetaData.GetPortrait();
				if (crossPromotionAdCreative == null)
				{
					crossPromotionAdCreative = assetMetaData.GetLandscape();
				}
			}
			return crossPromotionAdCreative;
		}

		private readonly IScreenOrientationGetter screenOrientationGetter;
	}
}
