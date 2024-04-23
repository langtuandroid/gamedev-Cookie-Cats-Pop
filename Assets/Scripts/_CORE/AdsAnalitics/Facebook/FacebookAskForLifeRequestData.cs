using System;

public class FacebookAskForLifeRequestData : FacebookRequestData
{
	public FacebookAskForLifeRequestData() : base("askforlives")
	{
	}

	public FacebookAskForLifeRequestData(string cloudId, string facebookId, string senderName) : base("askforlives", cloudId, facebookId)
	{
		base.FromName = senderName;
	}

	public const string REQUEST_TYPE = "askforlives";
}
