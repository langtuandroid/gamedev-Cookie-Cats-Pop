using System;

namespace TactileModules.Analytics.EventVerification.Packaging
{
	public interface IPackageMetaData
	{
		string GetPlatform();

		string GetUserId();
	}
}
