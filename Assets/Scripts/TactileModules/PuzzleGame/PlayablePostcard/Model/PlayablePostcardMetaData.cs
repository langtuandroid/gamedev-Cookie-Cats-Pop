using System;
using ConfigSchema;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.PuzzleGame.PlayablePostcard.Model
{
	public class PlayablePostcardMetaData : FeatureMetaData
	{
		[Required]
		[Description("AssetBundleName")]
		[JsonSerializable("AssetBundleName", null)]
		public string AssetBundleName { get; set; }
	}
}
