using System;

public class FacebookLifeRequestData : FacebookRequestData
{
	public FacebookLifeRequestData() : base("life")
	{
	}

	public FacebookLifeRequestData(string cloudId, string facebookId, string senderName) : base("life", cloudId, facebookId)
	{
		base.FromName = senderName;
	}

	public const string REQUEST_TYPE = "life";
}
