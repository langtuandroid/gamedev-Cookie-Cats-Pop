using System;
using System.Collections.Generic;

namespace TactileModules.TactileCloud.AssetBundles
{
	public class PersistableState
	{
		public PersistableState()
		{
			this.AvailableAssetBundles = new Dictionary<string, AssetBundleInfo>();
		}

		[JsonSerializable("AvailableAssetBundles", typeof(AssetBundleInfo))]
		public Dictionary<string, AssetBundleInfo> AvailableAssetBundles { get; set; }

		[JsonSerializable("AvailableAssetBundlesVersion", null)]
		public int AvailableAssetBundlesVersion { get; set; }

		[JsonSerializable("LastSeenBundleVersion", null)]
		public int LastSeenBundleVersion { get; set; }
	}
}
