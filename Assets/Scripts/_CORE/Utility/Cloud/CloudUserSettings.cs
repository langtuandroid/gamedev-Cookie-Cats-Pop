using System;
using System.Collections;

public class CloudUserSettings
{
	public CloudUserSettings()
	{
		this.Private = new Hashtable();
		this.Public = new Hashtable();
	}

	[JsonSerializable("_id", null)]
	public string CloudId { get; set; }

	[JsonSerializable("userId", null)]
	public string UserId { get; set; }

	[JsonSerializable("version", null)]
	public int Version { get; set; }

	[JsonSerializable("privateSettings", null)]
	public Hashtable Private { get; set; }

	[JsonSerializable("publicSettings", null)]
	public Hashtable Public { get; set; }
}
