using System;
using System.Collections.Generic;

namespace TactileModules.Analytics.EventVerification.Storage.Serialization
{
	public class StoredEvents
	{
		public StoredEvents()
		{
			this.Events = new Dictionary<string, int>();
		}

		[JsonSerializable("Events", typeof(int))]
		public Dictionary<string, int> Events { get; set; }
	}
}
