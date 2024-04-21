using System;
using System.Collections;
using Cloud;

namespace TactileModules.CrossPromotion.Cloud.CloudInterface
{
	public interface IAdServerCloudInterface
	{
		IEnumerator CrossPromotionAdConfig(Response result);

		IEnumerator CrossPromotionAdInterstitial(string[] installedGames, int userProgress, Response result);

		IEnumerator CrossPromotionAdRewardedVideo(string[] installedGames, int userProgress, Response result);
	}
}
