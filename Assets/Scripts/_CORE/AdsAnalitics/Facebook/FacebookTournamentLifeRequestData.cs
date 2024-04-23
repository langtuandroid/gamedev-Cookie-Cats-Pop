using System;

public class FacebookTournamentLifeRequestData : FacebookRequestData
{
	public FacebookTournamentLifeRequestData() : base("tournamentLife")
	{
	}

	public FacebookTournamentLifeRequestData(string cloudId, string facebookId, string senderName) : base("tournamentLife", cloudId, facebookId)
	{
		base.FromName = senderName;
	}

	public const string REQUEST_TYPE = "tournamentLife";
}
