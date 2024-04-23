using System;

public interface ITournamentRankSelectButton
{
	void Init(TournamentRank rank);

	TournamentRank Rank { get; }

	bool Selected { get; set; }
}
