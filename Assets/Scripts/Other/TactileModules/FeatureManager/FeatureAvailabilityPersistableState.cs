using System;
using System.Collections.Generic;

namespace TactileModules.FeatureManager
{
	public class FeatureAvailabilityPersistableState
	{
		public FeatureAvailabilityPersistableState()
		{
			this.Availability = new Dictionary<string, bool>();
		}

		[JsonSerializable("FeatureAvailability", typeof(bool))]
		public Dictionary<string, bool> Availability { get; set; }
	}
}
