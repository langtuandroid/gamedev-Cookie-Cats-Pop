using System;
using System.Collections.Generic;
using Tactile;
using UnityEngine;

public class SeagullManager : Singleton<SeagullManager>
{
	private SeagullConfig Config
	{
		get
		{
			return ConfigurationManager.Get<SeagullConfig>();
		}
	}

	private int SpawnSeagullCounter
	{
		get
		{
			return TactilePlayerPrefs.GetInt(SeagullManager.TactilePlayerPrefsKey, 0);
		}
		set
		{
			TactilePlayerPrefs.SetInt(SeagullManager.TactilePlayerPrefsKey, value);
		}
	}

	public bool IsSeagullReady
	{
		get
		{
			return this.SpawnSeagullCounter >= this.Config.SpawnIntervalInLevelEnds;
		}
	}

	public bool IsSeagullActive
	{
		get
		{
			return this.Config.IsActive;
		}
	}

	public void MarkCount()
	{
		this.SpawnSeagullCounter++;
	}

	public void ResetCount()
	{
		this.SpawnSeagullCounter = 0;
	}

	public List<ItemAmount> GetRewards()
	{
		return this.Config.Rewards;
	}

	public SeagullEffect.SetupParameters GetBackgroundParams()
	{
		float num = (float)this.SpawnSeagullCounter;
		float num2 = (float)this.Config.SpawnIntervalInLevelEnds;
		float num3 = num / num2;
		float distanceAway = UnityEngine.Random.Range(0.5f, 1f);
		if (this.IsSeagullActive)
		{
			distanceAway = 1f - num3 / 2f + UnityEngine.Random.Range(-0.05f, 0.05f);
		}
		return new SeagullEffect.SetupParameters
		{
			distanceAway = distanceAway,
			flyDuration = UnityEngine.Random.Range(4f, 8f),
			flyLeft = (UnityEngine.Random.value < 0.5f),
			height = UnityEngine.Random.value
		};
	}

	private static string TactilePlayerPrefsKey = "SeagullManager_Counter";
}
