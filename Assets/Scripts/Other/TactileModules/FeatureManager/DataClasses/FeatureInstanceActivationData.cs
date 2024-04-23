using System;
using ConfigSchema;

namespace TactileModules.FeatureManager.DataClasses
{
	public sealed class FeatureInstanceActivationData
	{
		[Obsolete("use non empty constructor instead", true)]
		public FeatureInstanceActivationData()
		{
		}

		public FeatureInstanceActivationData(FeatureData featureData, int activationServerTimeStamp)
		{
			this.ActivatedFeatureData = featureData;
			this.ActivationServerTimeStamp = activationServerTimeStamp;
		}

		[JsonSerializable("afd", null)]
		[Description("The FeatureData for this feature ID.")]
		public FeatureData ActivatedFeatureData { get; set; }

		[JsonSerializable("ast", null)]
		[Description("The server time stamp for when this feature was activated")]
		public int ActivationServerTimeStamp { get; set; }
	}
}
