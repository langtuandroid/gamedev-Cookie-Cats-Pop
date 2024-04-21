using System;
using System.Collections;
using Cloud;
using Fibers;
using TactileModules.CrossPromotion.Cloud.CloudInterface;
using TactileModules.CrossPromotion.Cloud.Data;
using TactileModules.CrossPromotion.Cloud.RequestState;
using TactileModules.CrossPromotion.Cloud.ResponseParsing;
using TactileModules.RuntimeTools;

namespace TactileModules.CrossPromotion.Cloud
{
	public abstract class CrossPromotionAdCloud : ICrossPromotionAdCloud
	{
		protected CrossPromotionAdCloud(IAdServerCloudInterface cloudInterface, ICloudResponseParser cloudResponseParser, IRequestStateHandler requestStateHandler, ITactileDateTime tactileDateTime)
		{
			this.cloudInterface = cloudInterface;
			this.cloudResponseParser = cloudResponseParser;
			this.requestStateHandler = requestStateHandler;
			this.tactileDateTime = tactileDateTime;
		}

		public IEnumerator GetCrossPromotionAd(string[] installedApps, int userProgress, EnumeratorResult<CrossPromotionAdMetaData> result)
		{
			Response response = new Response();
			yield return this.InvokeCloudInterface(installedApps, userProgress, response);
			CrossPromotionAdMetaData data = this.cloudResponseParser.ParseDataFromResponse<CrossPromotionAdMetaData>(response);
			result.value = data;
			if (response.Success)
			{
				this.requestStateHandler.SetLastSuccessfulRequestTimestamp(this.tactileDateTime.UtcNow);
			}
			yield break;
		}

		public abstract IEnumerator InvokeCloudInterface(string[] installedApps, int userProgress, Response response);

		private readonly IAdServerCloudInterface cloudInterface;

		private readonly ICloudResponseParser cloudResponseParser;

		private readonly IRequestStateHandler requestStateHandler;

		private readonly ITactileDateTime tactileDateTime;
	}
}
