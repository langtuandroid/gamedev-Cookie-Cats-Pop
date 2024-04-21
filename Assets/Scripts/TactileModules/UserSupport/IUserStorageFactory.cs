using System;
using TactileModules.TactilePrefs;

namespace TactileModules.UserSupport
{
	public interface IUserStorageFactory
	{
		ILocalStorageObject<UserDetails> GetStorage(string domain, string key);
	}
}
