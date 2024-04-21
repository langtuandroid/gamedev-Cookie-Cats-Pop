using System;
using JetBrains.Annotations;

namespace TactileModules.UserSupport.Model
{
	public class Backup
	{
		[JsonSerializable("state", null)]
		public string State { get; [UsedImplicitly] set; }

		[JsonSerializable("data", null)]
		public string Data { get; [UsedImplicitly] set; }

		[JsonSerializable("lastModified", null)]
		public string LastModifiedAsString { get; [UsedImplicitly] set; }

		public DateTime LastModified
		{
			get
			{
				if (string.IsNullOrEmpty(this.LastModifiedAsString))
				{
					return DateTime.MinValue;
				}
				return DateTime.Parse(this.LastModifiedAsString);
			}
		}

		public string GetPrettyFormattedDate()
		{
			if (this.LastModified == DateTime.MinValue)
			{
				return string.Empty;
			}
			return this.LastModified.ToString("dddd, dd MMMM yyyy");
		}

		public const string PENDING = "pending";

		public const string DISMISSED = "dismissed";

		public const string APPLIED = "applied";
	}
}
