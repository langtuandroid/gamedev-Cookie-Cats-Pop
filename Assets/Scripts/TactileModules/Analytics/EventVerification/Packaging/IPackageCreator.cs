using System;
using System.Collections.Generic;

namespace TactileModules.Analytics.EventVerification.Packaging
{
	public interface IPackageCreator
	{
		List<EventCountPackage> Create(Dictionary<string, int> rawCounts);
	}
}
