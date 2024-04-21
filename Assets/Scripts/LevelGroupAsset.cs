using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelGroupAsset : LevelCollectionEntry, ILevelCollection
{
	public List<LevelStub> LevelStubs
	{
		get
		{
			return this.levelStubs;
		}
		set
		{
			this.levelStubs = value;
		}
	}

	public AssetBundle AssetBundle { get; set; }

	public int NumberOfAvailableLevels
	{
		get
		{
			return this.LevelStubs.Count;
		}
	}

	public int GetHumanNumber(LevelProxy levelProxy)
	{
		int index = levelProxy.Index;
		if (index < 0 || index >= this.NumberOfAvailableLevels)
		{
			return -1;
		}
		return this.LevelStubs[index].humanNumber;
	}

	public abstract string GetAnalyticsDescriptor();

	public string GetPersistedKey(LevelProxy levelProxy)
	{
		return "DummyEntryGate";
	}

	public bool ValidateLevelIndex(int index)
	{
		return index < this.NumberOfAvailableLevels && index > -1;
	}

	[SerializeField]
	[HideInInspector]
	private List<LevelStub> levelStubs;
}
