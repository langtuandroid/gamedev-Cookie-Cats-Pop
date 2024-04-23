using System;

public class FacebookAskForKeyRequestData : FacebookRequestData
{
	public FacebookAskForKeyRequestData() : base("askforkey")
	{
	}

	public FacebookAskForKeyRequestData(string cloudId, string facebookId, string senderName) : base("askforkey", cloudId, facebookId)
	{
		base.FromName = senderName;
	}

	public const string REQUEST_TYPE = "askforkey";
}
