using System;
using UnityEngine;

public sealed class CloudUser
{
	public CloudUser()
	{
		this.IsDeveloper = false;
	}

	[JsonSerializable("_id", null)]
	public string CloudId { get; private set; }

	[JsonSerializable("facebookId", null)]
	public string FacebookId { get; private set; }

	[JsonSerializable("kakaoId", null)]
	public string KakaoId { get; private set; }

	public string ExternalId
	{
		get
		{
			if (!string.IsNullOrEmpty(this.FacebookId))
			{
				return this.FacebookId;
			}
			return this.KakaoId;
		}
	}

	[JsonSerializable("displayName", null)]
	public string DisplayName { get; private set; }

	[JsonSerializable("authSecret", null)]
	public string AuthSecret { get; private set; }

	[JsonSerializable("pushEnabled", null)]
	public bool PushEnabled { get; set; }

	[JsonSerializable("isDeveloper", null)]
	public bool IsDeveloper { get; private set; }

	public string GetFirstName(int maxLength = 20)
	{
		string displayName = this.DisplayName;
		string[] array = displayName.Split(new string[]
		{
			" "
		}, StringSplitOptions.None);
		string text = (array.Length <= 0) ? displayName : array[0];
		return text.Substring(0, Mathf.Min(text.Length, maxLength));
	}
}
