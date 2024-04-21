using System;
using TactileModules.CrossPromotion.Cloud.Data;

namespace TactileModules.CrossPromotion.Cloud.DataInterfaces
{
	public interface ICrossPromotionAdAssetMetaData
	{
		CrossPromotionAdCreative GetPortrait();

		CrossPromotionAdCreative GetLandscape();
	}
}
