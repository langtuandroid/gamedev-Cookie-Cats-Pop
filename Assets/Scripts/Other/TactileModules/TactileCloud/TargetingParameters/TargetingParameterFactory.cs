using System;
using System.Collections;
using System.Collections.Generic;
using TactileModules.InstallTimeTracking;

namespace TactileModules.TactileCloud.TargetingParameters
{
	public class TargetingParameterFactory : ITargetingParameterFactory
	{
		public TargetingParameterFactory(ICloudClientState cloudClientState, IInstallTime installTime, params ITargetingParametersProvider[] providers)
		{
			this.cloudClientState = cloudClientState;
			this.installTime = installTime;
			this.CacheProviders(providers);
		}

		private void CacheProviders(ITargetingParametersProvider[] providers)
		{
			CloudTargetingParametersProvider item = new CloudTargetingParametersProvider(this.cloudClientState, this.installTime);
			this.allProviders.Add(item);
			this.allProviders.AddRange(providers);
		}

		public Hashtable GetTargetingParameters()
		{
			Hashtable hashtable = new Hashtable();
			foreach (ITargetingParametersProvider targetingParametersProvider in this.allProviders)
			{
				IDictionaryEnumerator enumerator2 = targetingParametersProvider.GetAdditionalTargetingParameters().GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						object obj = enumerator2.Current;
						DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
						if (hashtable.ContainsKey(dictionaryEntry.Key))
						{
							throw new ArgumentException("Key duplication when adding: " + dictionaryEntry.Key);
						}
						hashtable.Add(dictionaryEntry.Key, dictionaryEntry.Value);
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator2 as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
			}
			return hashtable;
		}

		private readonly ICloudClientState cloudClientState;

		private readonly IInstallTime installTime;

		private readonly List<ITargetingParametersProvider> allProviders = new List<ITargetingParametersProvider>();
	}
}
