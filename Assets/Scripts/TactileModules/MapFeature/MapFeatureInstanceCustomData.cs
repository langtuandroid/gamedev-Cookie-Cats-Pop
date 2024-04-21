using System;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.MapFeature
{
	public class MapFeatureInstanceCustomData : FeatureInstanceCustomData
	{
		protected MapFeatureInstanceCustomData()
		{
			this.IsParticipating = false;
		}

		[JsonSerializable("ip", null)]
		public bool IsParticipating { get; set; }
	}
}
