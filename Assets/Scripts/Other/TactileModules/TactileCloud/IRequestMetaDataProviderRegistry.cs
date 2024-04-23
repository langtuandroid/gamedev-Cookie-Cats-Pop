using System;
using System.Collections.Generic;

namespace TactileModules.TactileCloud
{
	public interface IRequestMetaDataProviderRegistry
	{
		void RegisterProvider(IRequestMetaDataProvider requestMetaDataProvider);

		IEnumerable<IRequestMetaDataProvider> GetProviders(string endPoint);
	}
}
