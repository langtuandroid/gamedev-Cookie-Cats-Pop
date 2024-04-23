using System;
using System.Collections.Generic;

public class TournamentRankConfig
{
	[JsonSerializable("Identifier", null)]
	public string Identifier { get; set; }

	[JsonSerializable("Prizes", typeof(TournamentPrizeConfig))]
	public List<TournamentPrizeConfig> Prizes { get; set; }
}
