using System;

namespace TactileModules.Analytics.EventVerification.Packaging
{
	public class EventCount
	{
		public string VersionName { get; set; }

		public int VersionCode { get; set; }

		public string EventDate { get; set; }

		[JsonSerializable("eventSchemaHash", null)]
		public string EventSchemaHash { get; set; }

		[JsonSerializable("eventName", null)]
		public string EventName { get; set; }

		[JsonSerializable("eventCount", null)]
		public int Count { get; set; }
	}
}
