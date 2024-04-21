using System;
using System.Collections.Generic;
using ConfigSchema;

namespace TactileModules.Placements
{
	public class PlacementConfigData
	{
		public PlacementConfigData()
		{
			this.SkippableRunnables = new List<PlacementConfigData.PlacementRunnableData>();
			this.UnskippableRunnables = new List<PlacementConfigData.PlacementRunnableData>();
		}

		[JsonSerializable("ViewLimit", null)]
		public int ViewLimit { get; set; }

		[ArrayFormat(ArrayFormatAttribute.ArrayFormat.table)]
		[JsonSerializable("Unskippable", typeof(PlacementConfigData.PlacementRunnableData))]
		public List<PlacementConfigData.PlacementRunnableData> UnskippableRunnables { get; set; }

		[ArrayFormat(ArrayFormatAttribute.ArrayFormat.table)]
		[JsonSerializable("Skippable", typeof(PlacementConfigData.PlacementRunnableData))]
		public List<PlacementConfigData.PlacementRunnableData> SkippableRunnables { get; set; }

		public class PlacementRunnableData
		{
			[JsonSerializable("ID", null)]
			public string ID { get; set; }

			[JsonSerializable("Enabled", null)]
			public bool Enabled { get; set; }
		}
	}
}
