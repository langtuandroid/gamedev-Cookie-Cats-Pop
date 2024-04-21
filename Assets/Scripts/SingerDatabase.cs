using System;
using System.Collections.Generic;
using UnityEngine;

[SingletonAssetPath("Assets/[Database]/Resources/SingerDatabase.asset")]
public class SingerDatabase : SingletonAsset<SingerDatabase>
{
	public SingerInfo FindInfo(string id)
	{
		foreach (SingerInfo singerInfo in this.singers)
		{
			if (singerInfo.id == id)
			{
				return singerInfo;
			}
		}
		return null;
	}

	public Transform LoadPrefabFromResources(string id)
	{
		SingerInfo singerInfo = this.FindInfo(id);
		if (singerInfo != null)
		{
			return Resources.Load<Transform>("SingerPrefabs/singer_" + singerInfo.id);
		}
		return null;
	}

	public List<string> GetDefaultBand()
	{
		return new List<string>(this.defaultBand);
	}

	public string GetMissingBandMember()
	{
		if (!this.defaultBand.Contains("pink"))
		{
			return "Pink";
		}
		if (!this.defaultBand.Contains("red"))
		{
			return "Red";
		}
		if (!this.defaultBand.Contains("green"))
		{
			return "Green";
		}
		if (!this.defaultBand.Contains("blue"))
		{
			return "Blue";
		}
		if (!this.defaultBand.Contains("yellow"))
		{
			return "Yellow";
		}
		return "Red";
	}

	[SerializeField]
	private List<SingerInfo> singers;

	[SerializeField]
	private List<string> defaultBand;
}
