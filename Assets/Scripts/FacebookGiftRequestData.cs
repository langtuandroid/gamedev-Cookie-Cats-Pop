using System;

public class FacebookGiftRequestData : FacebookRequestData
{
	public FacebookGiftRequestData() : base("gift")
	{
	}

	public FacebookGiftRequestData(string cloudId, string facebookId) : base("gift", cloudId, facebookId)
	{
	}

	[JsonSerializable("st", null)]
	public string GiftType { get; set; }

	public const string REQUEST_TYPE = "gift";
}
