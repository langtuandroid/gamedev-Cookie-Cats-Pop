using System;
using System.Collections.Generic;
using TactileModules.TactilePrefs;

namespace TactileModules.UserSupport
{
	public class UserStorageFactory : IUserStorageFactory
	{
		public ILocalStorageObject<UserDetails> GetStorage(string domain, string key)
		{
			string key2 = domain + key;
			if (!this.cache.ContainsKey(key2))
			{
				LocalStorageJSONObject<UserDetails> value = new LocalStorageJSONObject<UserDetails>(new PlayerPrefsSignedString(domain, key));
				this.cache[key2] = value;
			}
			return this.cache[key2];
		}

		private Dictionary<string, LocalStorageJSONObject<UserDetails>> cache = new Dictionary<string, LocalStorageJSONObject<UserDetails>>();
	}
}
