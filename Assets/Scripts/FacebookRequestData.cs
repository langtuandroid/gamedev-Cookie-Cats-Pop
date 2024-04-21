using System;
using System.Collections;

public abstract class FacebookRequestData
{
	public FacebookRequestData(string type)
	{
		this.Version = 2;
		this.RequestType = type;
	}

	public FacebookRequestData(string type, string cloudId, string facebookId)
	{
		this.Version = 2;
		this.RequestType = type;
		this.SenderCloudId = cloudId;
	}

	[JsonSerializable("v", null)]
	public int Version { get; set; }

	[JsonSerializable("t", null)]
	public string RequestType { get; set; }

	[JsonSerializable("c", null)]
	public string SenderCloudId { get; set; }

	[JsonSerializable("s", null)]
	public string FromName { get; set; }

	public Hashtable RequestData
	{
		get
		{
			return this.request.Data;
		}
	}

	public string RequestMessage
	{
		get
		{
			return this.request.Message;
		}
	}

	public string RequestSenderFacebookId
	{
		get
		{
			return (this.request.From == null) ? string.Empty : this.request.From.Id;
		}
	}

	public bool IsRequestConsumed
	{
		get
		{
			return this.request.IsConsumed;
		}
		set
		{
			this.request.IsConsumed = value;
		}
	}

	public string RequestId
	{
		get
		{
			return this.request.Id;
		}
	}

	public long RequestCreatedTime
	{
		get
		{
			return this.request.CreatedTime;
		}
	}

	public static bool IsRequestValidAndSupported(FacebookRequest r)
	{
		string value = (r.From == null) ? null : r.From.Id;
		if (r.Data != null && r.Data.ContainsKey("e") && !string.IsNullOrEmpty(r.Data["e"] as string))
		{
			value = (r.Data["e"] as string);
		}
		bool flag = string.IsNullOrEmpty(value) || r.Data == null || r.Data.GetType() != typeof(Hashtable) || !r.Data.ContainsKey("t") || r.Data["t"] == null || !r.Data.ContainsKey("v") || r.Data["v"] == null || (double)r.Data["v"] > 2.0;
		return !flag;
	}

	public void SetRequest(FacebookRequest request)
	{
		this.request = request;
	}

	private FacebookRequest request;

	private const int SUPPORTED_VERSION = 2;
}
