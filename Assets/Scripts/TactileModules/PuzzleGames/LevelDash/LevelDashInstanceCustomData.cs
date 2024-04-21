using System;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.PuzzleGames.LevelDash
{
	[ObsoleteJsonName("irjr")]
	public class LevelDashInstanceCustomData : FeatureInstanceCustomData
	{
		public LevelDashInstanceCustomData()
		{
			this.JoinedInstanceId = string.Empty;
			this.StartLevel = -1;
			this.ReceivedRewardStatus = RewardStatus.Default;
			this.HasReceivedFinalStatus = false;
		}

		[JsonSerializable("jid", null)]
		public string JoinedInstanceId { get; set; }

		[JsonSerializable("sl", null)]
		public int StartLevel { get; set; }

		[JsonSerializable("rr", null)]
		public RewardStatus ReceivedRewardStatus { get; set; }

		[JsonSerializable("hrfs", null)]
		public bool HasReceivedFinalStatus { get; set; }

		public bool IsReceivedValidResultResponse()
		{
			return this.ReceivedRewardStatus == RewardStatus.Presented || this.ReceivedRewardStatus == RewardStatus.PreviouslyPresented || this.ReceivedRewardStatus == RewardStatus.NoValidEntry;
		}

		private const RewardStatus DEFAULT_REWARD_STATUS = RewardStatus.Default;
	}
}
