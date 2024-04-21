using System;
using TactileModules.TactileCloud;

namespace TactileModules.CrossPromotion.Cloud.ResponseParsing
{
	public interface ICloudResponseParser
	{
		T ParseDataFromResponse<T>(ICloudResponse response);
	}
}
