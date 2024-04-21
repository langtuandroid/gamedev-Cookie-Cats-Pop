using System;
using System.Collections;
using Fibers;
using TactileModules.CrossPromotion.Cloud.Data;

namespace TactileModules.CrossPromotion.Cloud
{
	public interface ICrossPromotionAdCloud
	{
		IEnumerator GetCrossPromotionAd(string[] installedApps, int userProgress, EnumeratorResult<CrossPromotionAdMetaData> result);
	}
}
