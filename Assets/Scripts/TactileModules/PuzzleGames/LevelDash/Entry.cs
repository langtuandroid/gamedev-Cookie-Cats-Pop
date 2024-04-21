using System;

namespace TactileModules.PuzzleGames.LevelDash
{
	public class Entry
	{
		[JsonSerializable("userId", null)]
		public string UserId { get; set; }

		[JsonSerializable("deviceId", null)]
		public string DeviceId { get; set; }

		[JsonSerializable("maxLevel", null)]
		public int MaxLevel { get; set; }

		[JsonSerializable("tournamentId", null)]
		public int TournamentId { get; set; }

		public bool IsOwnedByDeviceOrUser(string deviceId, string userId)
		{
			return (!string.IsNullOrEmpty(this.UserId) && this.UserId == userId) || (!string.IsNullOrEmpty(this.DeviceId) && this.DeviceId == deviceId);
		}
	}
}
