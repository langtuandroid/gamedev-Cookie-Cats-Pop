using System;
using Tactile;

public class TicketEarnedView : UIView
{
	protected override void ViewLoad(object[] parameters)
	{
		base.ViewLoad(parameters);
		if (parameters.Length > 0)
		{
			TournamentRank tournamentRank = (TournamentRank)parameters[0];
			this.ticket.SpriteName = "Ticket" + tournamentRank;
			TournamentConfig tournamentConfig = ConfigurationManager.Get<TournamentConfig>();
			int num;
			if (tournamentRank != TournamentRank.Gold)
			{
				if (tournamentRank != TournamentRank.Silver)
				{
					num = tournamentConfig.LevelNrRequiredForTournament;
				}
				else
				{
					num = tournamentConfig.LevelNrRequiredForSilverTournament;
				}
			}
			else
			{
				num = tournamentConfig.LevelNrRequiredForGoldTournament;
			}
			this.description.text = string.Format(L.Get("Unlock level {0} to compete in {1} tournaments and win fabulous prizes!"), num, tournamentRank);
		}
	}

	private void DismissClicked(UIEvent e)
	{
		base.Close(0);
	}

	public UILabel description;

	public UISprite ticket;
}
