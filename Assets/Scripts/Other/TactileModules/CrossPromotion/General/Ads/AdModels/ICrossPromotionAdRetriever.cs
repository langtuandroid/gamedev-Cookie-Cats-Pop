using System;

namespace TactileModules.CrossPromotion.General.Ads.AdModels
{
	public interface ICrossPromotionAdRetriever
	{
		ICrossPromotionAd GetPromotion();

		ICrossPromotionAd GetPresentablePromotion();

		bool IsRequesting();

		void RequestNewPromotion();
	}
}
