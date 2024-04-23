using System;

public class FacebookInviteRequestData : FacebookRequestData
{
	public FacebookInviteRequestData() : base("invite")
	{
	}

	public FacebookInviteRequestData(string cloudId, string facebookId) : base("invite", cloudId, facebookId)
	{
	}

	public const string REQUEST_TYPE = "invite";
}
