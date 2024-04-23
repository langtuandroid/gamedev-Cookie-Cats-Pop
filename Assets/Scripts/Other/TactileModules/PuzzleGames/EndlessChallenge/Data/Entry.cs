using System;

namespace TactileModules.PuzzleGames.EndlessChallenge.Data
{
	public class Entry
	{
		public Entry()
		{
			this.Score = new Score();
		}

		[JsonSerializable("userId", null)]
		public string UserId { get; set; }

		[JsonSerializable("device", null)]
		public string DeviceId { get; set; }

		[JsonSerializable("score", null)]
		public Score Score { get; set; }

		public bool IsOwnedByDeviceOrUser(string deviceId, string userId)
		{
			return (!string.IsNullOrEmpty(this.UserId) && this.UserId == userId) || (!string.IsNullOrEmpty(this.DeviceId) && this.DeviceId == deviceId);
		}
	}
}
