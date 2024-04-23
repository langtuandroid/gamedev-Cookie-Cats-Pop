using System;

namespace TactileModules.FeatureManager.DataClasses
{
	public class FeatureInstanceCustomData
	{
		public FeatureInstanceCustomData()
		{
			this.SerializedVersion = 1;
		}

		[JsonSerializable("sv", null)]
		protected int SerializedVersion { get; set; }
	}
}
