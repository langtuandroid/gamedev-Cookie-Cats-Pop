using System;

public sealed class CloudDevice
{
	public CloudDevice()
	{
		this.PushEnabled = true;
	}

	[JsonSerializable("_id", null)]
	public string CloudId { get; private set; }

	[JsonSerializable("pushEnabled", null)]
	public bool PushEnabled { get; set; }

	[JsonSerializable("countryCode", null)]
	public string CountryCode { get; set; }
}
