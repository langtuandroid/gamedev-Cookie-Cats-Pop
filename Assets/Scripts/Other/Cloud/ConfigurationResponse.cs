using System;
using System.Collections;

namespace Cloud
{
	public class ConfigurationResponse : Response
	{
		public Hashtable Configuration
		{
			get
			{
				return base.data["configuration"] as Hashtable;
			}
		}
	}
}
