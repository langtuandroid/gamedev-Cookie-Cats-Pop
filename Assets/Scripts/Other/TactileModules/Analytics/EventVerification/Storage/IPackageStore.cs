using System;
using System.Collections.Generic;
using TactileModules.Analytics.EventVerification.Packaging;

namespace TactileModules.Analytics.EventVerification.Storage
{
	public interface IPackageStore
	{
		void Add(List<EventCountPackage> packagedEvents);

		void Remove(EventCountPackage package);

		List<EventCountPackage> GetPackages();

		bool IsEmpty();

		void Load();
	}
}
