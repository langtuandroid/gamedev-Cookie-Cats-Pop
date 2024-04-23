using System;
using System.Collections.Generic;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	public class StoryMapEventMetaData : FeatureMetaData
	{
		public StoryMapEventMetaData()
		{
			this.AssetBundleNames = new List<string>();
		}

		[JsonSerializable("AssetBundleNames", typeof(string))]
		public List<string> AssetBundleNames { get; set; }
	}
}
