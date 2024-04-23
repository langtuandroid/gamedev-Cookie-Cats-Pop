using System;
using System.Collections.Generic;
using ConfigSchema;

namespace TactileModules.FeatureManager.DataClasses
{
	[RequireAll]
	public sealed class FeatureNotificationSettings
	{
		[Description("Is notifications enabled for this feature?")]
		[JsonSerializable("NotificationsEnabled", null)]
		private bool NotificationsEnabled { get; set; }

		[Description("The amount of seconds before the duration of an event expires notifications should show")]
		[ArrayFormat(ArrayFormatAttribute.ArrayFormat.table)]
		[JsonSerializable("NotificationTimes", typeof(int))]
		private List<int> NotificationTimes { get; set; }

		public List<int> GetNotificationTimes()
		{
			return (!this.NotificationsEnabled) ? new List<int>() : this.NotificationTimes;
		}
	}
}
