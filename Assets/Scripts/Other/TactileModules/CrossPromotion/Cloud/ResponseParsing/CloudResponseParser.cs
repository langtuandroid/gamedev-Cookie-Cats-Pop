using System;
using System.Diagnostics;
using Cloud;
using TactileModules.Analytics.Interfaces;
using TactileModules.TactileCloud;
using TactileModules.TactileLogger;

namespace TactileModules.CrossPromotion.Cloud.ResponseParsing
{
	public class CloudResponseParser : ICloudResponseParser
	{
		public CloudResponseParser(IAnalytics analytics)
		{
			this.analytics = analytics;
		}

		public T ParseDataFromResponse<T>(ICloudResponse response)
		{
			T result = default(T);
			try
			{
				bool success = response.Success;
				if (success)
				{
					if (!this.IsResponseEmpty(response))
					{
						result = JsonSerializer.HashtableToObject<T>(response.data);
						Log.Info(Category.CrossPromotion, () => "CrossPromotionCloudResponseParser: Received cross promotion ads: \n" + response.data.toPrettyJson(), null);
					}
				}
				else
				{
					Log.Info(Category.CrossPromotion, () => "CrossPromotionCloudResponseParser: response has some issues : \n" + response.ErrorInfo, null);
					if (response.ReturnCode != ReturnCode.ClientConnectionError)
					{
						this.LogResponseError(null, response);
					}
				}
			}
			catch (Exception ex)
			{
				Exception e2 = ex;
				Exception e = e2;
				Log.Info(Category.CrossPromotion, () => "CrossPromotionCloudResponseParser: response has some issues : \n" + e, null);
				this.LogResponseError(e, response);
			}
			return result;
		}

		private bool IsResponseEmpty(ICloudResponse response)
		{
			return response.data.Count <= 1;
		}

		private void LogResponseError(Exception e, ICloudResponse response)
		{
			this.analytics.LogEvent(new ClientErrorEvent("CrossPromotionRequestError", new StackTrace(false).ToString(), e, response.data.toJson(), null, null, null, null, null), -1.0, null);
		}

		private readonly IAnalytics analytics;
	}
}
