using System;

public class FacebookAskForTournamentLifeRequestData : FacebookRequestData
{
	public FacebookAskForTournamentLifeRequestData() : base("askforTournamentlives")
	{
	}

	public FacebookAskForTournamentLifeRequestData(string cloudId, string facebookId, string senderName) : base("askforTournamentlives", cloudId, facebookId)
	{
		base.FromName = senderName;
	}

	public const string REQUEST_TYPE = "askforTournamentlives";
}
