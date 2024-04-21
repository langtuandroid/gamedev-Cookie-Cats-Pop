using System;
using System.Collections.Generic;

namespace TactileModules.Analytics.EventVerification.Packaging
{
	public class EventCountPackage
	{
		public EventCountPackage()
		{
			this.PackageId = Guid.NewGuid().ToString();
			this.EventCounts = new List<EventCount>();
		}

		[JsonSerializable("userId", null)]
		public string UserId { get; set; }

		[JsonSerializable("packageId", null)]
		public string PackageId { get; private set; }

		[JsonSerializable("eventDate", null)]
		public string EventDate { get; set; }

		[JsonSerializable("versionName", null)]
		public string VersionName { get; set; }

		[JsonSerializable("versionCode", null)]
		public int VersionCode { get; set; }

		[JsonSerializable("platform", null)]
		public string Platform { get; set; }

		[JsonSerializable("eventCounts", typeof(EventCount))]
		public List<EventCount> EventCounts { get; set; }
	}
}
