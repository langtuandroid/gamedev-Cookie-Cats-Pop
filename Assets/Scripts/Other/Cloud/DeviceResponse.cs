using System;
using System.Collections;

namespace Cloud
{
	public class DeviceResponse : Response
	{
		public CloudDevice Device
		{
			get
			{
				Hashtable hashtable = base.data["device"] as Hashtable;
				return (hashtable == null) ? null : JsonSerializer.HashtableToObject<CloudDevice>(hashtable);
			}
		}
	}
}
