using System;
using System.Collections;
using ConfigSchema;

namespace TactileModules.FeatureManager.DataClasses
{
	public class FeatureData
	{
		public FeatureData()
		{
			this.MetaData = new Hashtable();
		}

		[JsonSerializable("id", null)]
		[Description("The unique ID of this feature instance")]
		public string Id { get; set; }

		[JsonSerializable("type", null)]
		[Description("The type of this feature instance")]
		public string Type { get; set; }

		[JsonSerializable("startAt", null)]
		[Description("The start date in unix time stamp")]
		public long StartUnixTime { get; set; }

		[JsonSerializable("endAt", null)]
		[Description("The end date in unix time stamp")]
		private long EndUnixTime { get; set; }

		[JsonSerializable("maxDuration", null)]
		[Description("The maximum active duration in seconds")]
		public int MaxDuration { get; set; }

		[JsonSerializable("minDuration", null)]
		[Description("The minimum active duration in seconds")]
		public int MinDuration { get; set; }

		[JsonSerializable("meta", null)]
		[Description("Meta data for the feature")]
		public Hashtable MetaData { get; set; }

		[IgnoreProperty]
		[JsonSerializable("metaVersion", null)]
		public int MetaVersion { get; set; }

		public long CorrectedEndUnixTime
		{
			get
			{
				return (this.EndUnixTime > 0L) ? this.EndUnixTime : long.MaxValue;
			}
		}

		public void SetEndUnixTime(int timeStamp)
		{
			this.EndUnixTime = (long)timeStamp;
		}
	}
}
