using System;
using System.Collections;

namespace TactileModules.UserSupport.DataProviders
{
	public interface IClientMessageMetaDataProvider
	{
		void AddUserSettings(out Hashtable privateUserSettings, out Hashtable publicUserSettings);

		void AddCustomData(out Hashtable customData);
	}
}
