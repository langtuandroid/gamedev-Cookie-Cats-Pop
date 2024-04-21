using System;

namespace TactileModules.TactileCloud.AssetBundles
{
	public class AssetBundleInfo
	{
		[JsonSerializable("URL", null)]
		public string URL { get; set; }

		[JsonSerializable("Filename", null)]
		public string Filename { get; set; }

		[JsonSerializable("UpdatedAt", null)]
		public string UpdatedAt { get; set; }

		[JsonSerializable("ExternalId", null)]
		public string ExternalId { get; set; }
	}
}
