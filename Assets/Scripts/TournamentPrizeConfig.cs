using System;
using System.Collections.Generic;

public class TournamentPrizeConfig
{
	private TournamentPrizeConfig()
	{
		this.Rewards = new List<ItemAmount>();
	}

	[JsonSerializable("RankTo", null)]
	public int RankTo { get; set; }

	[JsonSerializable("RankFrom", null)]
	public int RankFrom { get; set; }

	[JsonSerializable("Reward", typeof(ItemAmount))]
	public List<ItemAmount> Rewards { get; set; }
}
