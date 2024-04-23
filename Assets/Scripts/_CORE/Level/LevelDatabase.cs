using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TactileModules.FeatureManager;
using TactileModules.TactileCloud.AssetBundles;
using UnityEngine;

public abstract class LevelDatabase : ScriptableObject, ILevelDatabase, ILevelCollection
{
	public List<LevelCollectionEntry> EditorGetLevels()
	{
		return this.EDITOR_levelReferences;
	}

	public int EditorGetLevelIndex(LevelCollectionEntry levelAsset)
	{
		return this.EDITOR_levelReferences.IndexOf(levelAsset);
	}

	public List<LevelStub> LevelStubs
	{
		get
		{
			return this.levelStubs;
		}
	}

	public List<LevelDifficulty> LevelDifficultyList
	{
		get
		{
			return this.levelDifficultyList;
		}
	}

	public AssetBundleInfo AssetBundleInfo { get; set; }

	public AssetBundle AssetBundle { get; set; }

	public string AssetBundleFilename
	{
		get
		{
			if (this.AssetBundleInfo != null)
			{
				return this.AssetBundleInfo.Filename;
			}
			string text = "android";
			return string.Concat(new string[]
			{
				"LevelDatabase-",
				text,
				"-",
				SystemInfoHelper.BundleVersion,
				".assetbundle"
			});
		}
	}

	public string AssetBundleFilenameForAnalytics
	{
		get
		{
			if (this.AssetBundleInfo != null)
			{
				return this.AssetBundleInfo.Filename;
			}
			return string.Empty;
		}
	}

	public int DatabaseRevision
	{
		get
		{
			if (this.AssetBundleInfo == null)
			{
				return int.Parse(SystemInfoHelper.BundleVersion);
			}
			string pattern = ".*-.*-.*-.*-([0-9\\.]+).*-.*.assetbundle";
			Regex regex = new Regex(pattern);
			Match match = regex.Match(this.AssetBundleFilename);
			if (match.Success)
			{
				return int.Parse(match.Groups[1].Value);
			}
			return -1;
		}
	}

	public string AssetBundleURL
	{
		get
		{
			if (this.AssetBundleInfo != null)
			{
				return this.AssetBundleInfo.URL;
			}
			return string.Empty;
		}
	}

	public string GetContentHash()
	{
		return this.contentHash;
	}

	public abstract LevelProxy GetLevel(int levelIndex);

	public abstract string GetAnalyticsDescriptor();

	public abstract MapIdentifier GetMapAndLevelsIdentifier();

	public static string GetAssetName(MapIdentifier mapIdentifier)
	{
		return mapIdentifier + "leveldatabase";
	}

	public abstract string GetPersistedKey(LevelProxy levelProxy);

	public abstract void Save();

	public abstract ILevelAccomplishment GetLevelData(bool createIfNotExisting, LevelProxy levelProxy);

	public abstract void RemoveLevelData(LevelProxy levelProxy);

	public virtual int NumberOfAvailableLevels
	{
		get
		{
			return this.levelStubs.Count;
		}
	}

	public virtual bool ValidateLevelIndex(int index)
	{
		return index < this.levelStubs.Count && index > -1;
	}

	public string GetAnalyticsDescriptor(LevelProxy levelProxy)
	{
		return this.GetAnalyticsDescriptor();
	}

	public abstract double GetGateProgress(LevelProxy levelProxy);

	public double GetLevelProgress(LevelProxy levelProxy)
	{
		int humanNumber = this.GetHumanNumber(levelProxy);
		double gateProgress = this.GetGateProgress(levelProxy);
		return (double)humanNumber + gateProgress;
	}

	public virtual int GetHumanNumber(LevelProxy levelProxy)
	{
		int index = levelProxy.Index;
		if (index < 0 || index >= this.NumberOfAvailableLevels)
		{
			return -1;
		}
		return this.levelStubs[index].humanNumber;
	}

	public int GetRewardAssetIndex(LevelProxy levelProxy)
	{
		int num = 0;
		foreach (LevelStub levelStub in levelProxy.LevelCollection.LevelStubs)
		{
			if (!(((ILevelStubPrivateAccess)levelStub).MetaClassType != typeof(RewardLevelMetaData).Name))
			{
				if (levelProxy.Index == levelStub.index)
				{
					return num;
				}
				num++;
			}
		}
		return num;
	}

	public int GetGateIndex(int entryIndex)
	{
		if (entryIndex < 0 || entryIndex >= this.NumberOfAvailableLevels)
		{
			return -1;
		}
		GateMetaData gateMetaData = this.GetLevelMetaData(entryIndex) as GateMetaData;
		if (gateMetaData == null)
		{
			return -1;
		}
		return gateMetaData.gateIndex;
	}

	public LevelStub GetGateStubFromIndex(int gateIndex)
	{
		for (int i = 0; i < this.levelStubs.Count; i++)
		{
			GateMetaData gateMetaData = this.GetLevelMetaData(i) as GateMetaData;
			if (gateMetaData != null && gateIndex == gateMetaData.gateIndex)
			{
				return this.levelStubs[i];
			}
		}
		return null;
	}

	public int NonGateIndexToDotIndex(LevelProxy levelProxy, Type metaDataType)
	{
		int num = levelProxy.Index;
		if (this.nonGateIndexToDotIndexCache.ContainsKey(num))
		{
			return this.nonGateIndexToDotIndexCache[num];
		}
		ILevelCollection levelCollection = levelProxy.LevelCollection;
		if (num < 0)
		{
			return num;
		}
		Type type = (metaDataType != null) ? metaDataType : typeof(GateMetaData);
		for (int i = 0; i < levelCollection.LevelStubs.Count; i++)
		{
			if (levelCollection.GetLevelMetaData(i).GetType() == type)
			{
				num++;
			}
			if (i == num)
			{
				break;
			}
		}
		this.nonGateIndexToDotIndexCache.Add(levelProxy.Index, num);
		return num;
	}

	public int DotIndexToNonGateIndex(LevelProxy levelProxy, Type metaDataType)
	{
		int index = levelProxy.Index;
		if (this.dotIndexToNonGateIndexCache.ContainsKey(index))
		{
			return this.dotIndexToNonGateIndexCache[index];
		}
		ILevelCollection levelCollection = levelProxy.LevelCollection;
		if (index < 0)
		{
			return index;
		}
		Type type = (metaDataType != null) ? metaDataType : typeof(GateMetaData);
		int num = 0;
		for (int i = 0; i < levelCollection.LevelStubs.Count; i++)
		{
			if (levelCollection.GetLevelMetaData(i).GetType() == type)
			{
				num++;
			}
			if (i == index)
			{
				break;
			}
		}
		int num2 = index - num;
		this.dotIndexToNonGateIndexCache.Add(levelProxy.Index, num2);
		return num2;
	}

	public virtual LevelDifficulty GetLevelDifficulty(int levelId)
	{
		if (!(this.GetLevelMetaData(levelId) is GateMetaData))
		{
			if (FeatureManager.GetFeatureHandler<HardLevelsManager>().IsLevelHard(new LevelProxy(this, new int[]
			{
				levelId
			})))
			{
				return LevelDifficulty.Hard;
			}
			if (levelId < this.LevelDifficultyList.Count)
			{
				return this.LevelDifficultyList[levelId];
			}
		}
		return LevelDifficulty.Normal;
	}

	private const string LEVELDATABASE_POST_ASSETNAME = "leveldatabase";

	[SerializeField]
	protected List<LevelCollectionEntry> EDITOR_levelReferences = new List<LevelCollectionEntry>();

	[SerializeField]
	private List<LevelStub> levelStubs;

	[SerializeField]
	private List<LevelDifficulty> levelDifficultyList = new List<LevelDifficulty>();

	[SerializeField]
	private string contentHash;

	private readonly Dictionary<int, int> nonGateIndexToDotIndexCache = new Dictionary<int, int>();

	private readonly Dictionary<int, int> dotIndexToNonGateIndexCache = new Dictionary<int, int>();
}
