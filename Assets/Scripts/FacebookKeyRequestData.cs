using System;

public class FacebookKeyRequestData : FacebookRequestData
{
	public FacebookKeyRequestData() : base("key")
	{
	}

	public FacebookKeyRequestData(string cloudId, string facebookId, string senderName) : base("key", cloudId, facebookId)
	{
		base.FromName = senderName;
	}

	public const string REQUEST_TYPE = "key";
}
