using System;
using System.Collections;

public class FacebookRequest
{
	[JsonSerializable("data", null)]
	public Hashtable Data { get; set; }

	[JsonSerializable("message", null)]
	public string Message { get; set; }

	[JsonSerializable("from", null)]
	public FacebookRequest.FacebookRequestUser From { get; set; }

	[JsonSerializable("to", null)]
	public FacebookRequest.FacebookRequestUser To { get; set; }

	[JsonSerializable("id", null)]
	public string Id { get; set; }

	[JsonSerializable("created_time", null)]
	public long CreatedTime { get; set; }

	public bool IsConsumed { get; set; }

	public class FacebookRequestUser
	{
		[JsonSerializable("name", null)]
		public string Name { get; set; }

		[JsonSerializable("id", null)]
		public string Id { get; set; }
	}
}
