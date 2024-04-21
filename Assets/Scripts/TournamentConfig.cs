using System;
using System.Collections.Generic;
using Tactile;

[ConfigProvider("TournamentConfig")]
public class TournamentConfig
{
	public TournamentConfig()
	{
		this.Ranks = new List<TournamentRankConfig>();
	}

	[JsonSerializable("LevelNrRequiredForTournament", null)]
	public int LevelNrRequiredForTournament { get; set; }

	[JsonSerializable("LevelNrRequiredForSilverTournament", null)]
	public int LevelNrRequiredForSilverTournament { get; set; }

	[JsonSerializable("LevelNrRequiredForGoldTournament", null)]
	public int LevelNrRequiredForGoldTournament { get; set; }

	[JsonSerializable("LifeRegenerationTime", null)]
	public int LifeRegenerationTime { get; set; }

	[JsonSerializable("LifeRegenerationMaxCount", null)]
	public int LifeRegenerationMaxCount { get; set; }

	[JsonSerializable("Ranks", typeof(TournamentRankConfig))]
	private List<TournamentRankConfig> Ranks { get; set; }

	public TournamentRankConfig GetRankConfig(TournamentRank rank)
	{
		string b = rank.ToString();
		foreach (TournamentRankConfig tournamentRankConfig in this.Ranks)
		{
			if (tournamentRankConfig.Identifier == b)
			{
				return tournamentRankConfig;
			}
		}
		return null;
	}
}
