using System;
using ConfigSchema;
using Tactile;

namespace TactileModules.Placements
{
	[ConfigProvider("Placements")]
	public sealed class PlacementsConfig
	{
		[JsonSerializable("PreAnimateAvatar", null)]
		[PlacementConfigBinding]
		private PlacementConfigData PreAnimateAvatar { get; set; }

		[JsonSerializable("PostAnimateAvatar", null)]
		[PlacementConfigBinding]
		private PlacementConfigData PostAnimateAvatar { get; set; }

		[JsonSerializable("SessionStart", null)]
		[PlacementConfigBinding]
		private PlacementConfigData SessionStart { get; set; }

		[Description("Invoked on entering the map")]
		[JsonSerializable("MapIdle", null)]
		[PlacementConfigBinding]
		private PlacementConfigData MapIdle { get; set; }

		[Description("Invoked when the level start view appears")]
		[JsonSerializable("LevelStartShown", null)]
		[PlacementConfigBinding]
		private PlacementConfigData LevelStartShown { get; set; }
	}
}
