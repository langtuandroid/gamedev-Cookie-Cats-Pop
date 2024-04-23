using System;
using System.Collections;

namespace TactileModules.TactileCloud
{
	public interface ICloudResponseEvents
	{
		event Action<Hashtable, string> ResponseMetaDataReceived;
	}
}
