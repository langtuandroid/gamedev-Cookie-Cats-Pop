using System;
using System.Collections.Generic;

namespace TactileModules.TactileCloud
{
	public class RequestMetaDataProviderRegistry : IRequestMetaDataProviderRegistry
	{
		public RequestMetaDataProviderRegistry()
		{
			this.providers = new Dictionary<string, List<IRequestMetaDataProvider>>();
		}

		public void RegisterProvider(IRequestMetaDataProvider requestMetaDataProvider)
		{
			List<string> endPoints = requestMetaDataProvider.GetEndPoints();
			foreach (string key in endPoints)
			{
				if (!this.providers.ContainsKey(key))
				{
					this.providers.Add(key, new List<IRequestMetaDataProvider>());
				}
				this.providers[key].Add(requestMetaDataProvider);
			}
		}

		public IEnumerable<IRequestMetaDataProvider> GetProviders(string endPoint)
		{
			if (!this.providers.ContainsKey(endPoint))
			{
				yield break;
			}
			foreach (IRequestMetaDataProvider provider in this.providers[endPoint])
			{
				yield return provider;
			}
			yield break;
		}

		private readonly Dictionary<string, List<IRequestMetaDataProvider>> providers;
	}
}
