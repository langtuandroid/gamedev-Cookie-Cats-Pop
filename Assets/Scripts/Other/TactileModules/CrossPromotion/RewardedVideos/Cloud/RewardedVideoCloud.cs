using System;
using System.Collections;
using Cloud;
using TactileModules.CrossPromotion.Cloud;
using TactileModules.CrossPromotion.Cloud.CloudInterface;
using TactileModules.CrossPromotion.Cloud.RequestState;
using TactileModules.CrossPromotion.Cloud.ResponseParsing;
using TactileModules.RuntimeTools;

namespace TactileModules.CrossPromotion.RewardedVideos.Cloud
{
	public class RewardedVideoCloud : CrossPromotionAdCloud
	{
		public RewardedVideoCloud(IAdServerCloudInterface cloudInterface, ICloudResponseParser cloudResponseParser, IRequestStateHandler requestStateHandler, ITactileDateTime tactileDateTime) : base(cloudInterface, cloudResponseParser, requestStateHandler, tactileDateTime)
		{
			this.cloudInterface = cloudInterface;
		}

		public override IEnumerator InvokeCloudInterface(string[] installedApps, int userProgress, Response response)
		{
			yield return this.cloudInterface.CrossPromotionAdRewardedVideo(installedApps, userProgress, response);
			yield break;
		}

		private readonly IAdServerCloudInterface cloudInterface;
	}
}
