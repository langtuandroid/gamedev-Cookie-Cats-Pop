using System;
using System.Collections.Generic;
using TactileModules.Analytics.EventVerification.Packaging;

namespace TactileModules.Analytics.EventVerification.Uploading
{
	public interface IPackageUploader
	{
		event Action<EventCountPackage> PackageUploaded;

		void UpLoadPackages(List<EventCountPackage> packages);

		bool IsUploading();
	}
}
