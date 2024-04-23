using System;
using TactileModules.CrossPromotion.Cloud.Data;
using TactileModules.CrossPromotion.Cloud.DataInterfaces;

namespace TactileModules.CrossPromotion.General.Ads.AdModels
{
	public interface IAdCreativeSelector
	{
		CrossPromotionAdCreative GetCreativeForOrientation(ICrossPromotionAdAssetMetaData assetMetaData);
	}
}
