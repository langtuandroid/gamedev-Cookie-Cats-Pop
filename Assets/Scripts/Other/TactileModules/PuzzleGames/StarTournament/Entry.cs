using System;

namespace TactileModules.PuzzleGames.StarTournament
{
	public class Entry
	{
		[JsonSerializable("userId", null)]
		public string UserId { get; set; }

		[JsonSerializable("deviceId", null)]
		public string DeviceId { get; set; }

		[JsonSerializable("stars", null)]
		public int Stars { get; set; }

		[JsonSerializable("maxLevel", null)]
		public int MaxLevel { get; set; }

		public bool IsOwnedByDeviceOrUser(string deviceId, string userId)
		{
			return (!string.IsNullOrEmpty(this.UserId) && this.UserId == userId) || (!string.IsNullOrEmpty(this.DeviceId) && this.DeviceId == deviceId);
		}
	}
}
