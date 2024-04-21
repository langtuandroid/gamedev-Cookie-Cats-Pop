using System;

namespace TactileModules.FeatureManager.DataClasses
{
	public class FeatureTypeCustomData
	{
		public FeatureTypeCustomData()
		{
			this.SerializedVersion = 1;
		}

		[JsonSerializable("sv", null)]
		protected int SerializedVersion { get; set; }
	}
}
