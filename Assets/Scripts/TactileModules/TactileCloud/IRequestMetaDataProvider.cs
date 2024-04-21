using System;
using System.Collections.Generic;

namespace TactileModules.TactileCloud
{
	public interface IRequestMetaDataProvider
	{
		string GetMetaDataKey();

		object GetMetaDataValue(string endPoint);

		List<string> GetEndPoints();
	}
}
