using System;

namespace TactileModules.PuzzleGames.StarTournament
{
	public class OldStartTournamentInfo
	{
		public OldStartTournamentInfo()
		{
			this.Reset();
		}

		public OldStartTournamentInfo(string featureId, int needToShow, StarTournamentConfig.Reward reward)
		{
			this.FeatureId = featureId;
			this.NeedToShowEndedViewOldTournament = needToShow;
			this.RewardForOldTournament = reward;
		}

		[JsonSerializable("fid", null)]
		public string FeatureId { get; set; }

		[JsonSerializable("nev", null)]
		public int NeedToShowEndedViewOldTournament { get; set; }

		[JsonSerializable("cot", null)]
		public StarTournamentConfig.Reward RewardForOldTournament { get; set; }

		public void Reset()
		{
			this.FeatureId = string.Empty;
			this.NeedToShowEndedViewOldTournament = 0;
			this.RewardForOldTournament = new StarTournamentConfig.Reward();
		}
	}
}
