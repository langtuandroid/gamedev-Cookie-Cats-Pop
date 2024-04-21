using System;
using System.Collections.Generic;
using TactileModules.Analytics.EventVerification.Packaging;

namespace TactileModules.Analytics.EventVerification.Storage.Serialization
{
	public class StoredPackages
	{
		public StoredPackages()
		{
			this.Packages = new List<EventCountPackage>();
		}

		[JsonSerializable("Packages", typeof(EventCountPackage))]
		public List<EventCountPackage> Packages { get; set; }
	}
}
