using System;

namespace TactileModules.Analytics.EventVerification.Packaging
{
	public class PackageMetaData : IPackageMetaData
	{
		public PackageMetaData(string userId)
		{
			this.userId = userId;
		}

		public string GetPlatform()
		{
			return SystemInfoHelper.DeviceType.ToUpper();
		}

		public string GetUserId()
		{
			return this.userId;
		}

		private string userId;
	}
}
