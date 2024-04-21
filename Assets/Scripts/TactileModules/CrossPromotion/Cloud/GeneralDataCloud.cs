using System;
using System.Collections;
using Cloud;
using Fibers;
using TactileModules.CrossPromotion.Cloud.CloudInterface;
using TactileModules.CrossPromotion.Cloud.Data;
using TactileModules.CrossPromotion.Cloud.ResponseParsing;

namespace TactileModules.CrossPromotion.Cloud
{
	public class GeneralDataCloud : ICrossPromotionGeneralDataCloud
	{
		public GeneralDataCloud(IAdServerCloudInterface cloudInterface, ICloudResponseParser cloudResponseParser)
		{
			this.cloudInterface = cloudInterface;
			this.cloudResponseParser = cloudResponseParser;
		}

		public IEnumerator GetCrossPromotionGeneralData(EnumeratorResult<CrossPromotionGeneralData> result)
		{
			Response response = new Response();
			yield return this.cloudInterface.CrossPromotionAdConfig(response);
			CrossPromotionGeneralData data = this.cloudResponseParser.ParseDataFromResponse<CrossPromotionGeneralData>(response);
			result.value = data;
			yield break;
		}

		private readonly IAdServerCloudInterface cloudInterface;

		private readonly ICloudResponseParser cloudResponseParser;
	}
}
