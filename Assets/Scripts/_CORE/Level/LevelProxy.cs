using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TactileModules.Foundation;

public class LevelProxy : ILevelProxy
{
	public LevelProxy(ILevelDatabase rootDatabase, params int[] indexPath)
	{
		this.cachedHashCode = 0;
		this.cachedHashCodeExists = false;
		this.indexPath = indexPath;
		LevelDatabase levelDatabase = rootDatabase as LevelDatabase;
		this.mapIdentifier = ((!(levelDatabase != null)) ? MapIdentifier.Empty : levelDatabase.GetMapAndLevelsIdentifier());
	}

	private LevelProxy(MapIdentifier mapIdentifier, params int[] indexPath)
	{
		this.cachedHashCode = 0;
		this.cachedHashCodeExists = false;
		this.mapIdentifier = mapIdentifier;
		this.indexPath = indexPath;
	}

	public LevelProxy CreateChildProxy(int childIndex)
	{
		int[] array = new int[this.indexPath.Length + 1];
		this.indexPath.CopyTo(array, 0);
		array[array.Length - 1] = childIndex;
		return new LevelProxy(this.mapIdentifier, array);
	}

	public LevelProxy CreateParentProxy()
	{
		if (this.indexPath.Length > 1)
		{
			int[] array = new int[this.indexPath.Length - 1];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = this.indexPath[i];
			}
			return new LevelProxy(this.mapIdentifier, array);
		}
		return LevelProxy.Invalid;
	}

	public bool IsValid
	{
		get
		{
			if (this.GetRootLevelDatabase() == null)
			{
				return false;
			}
			ILevelCollection levelCollection = this.LevelCollection;
			return levelCollection != null && levelCollection.ValidateLevelIndex(this.Index);
		}
	}

	public static bool operator ==(LevelProxy a, LevelProxy b)
	{
		return a.mapIdentifier == b.mapIdentifier && a.Index == b.Index;
	}

	public static bool operator !=(LevelProxy a, LevelProxy b)
	{
		return !(a == b);
	}

	public override bool Equals(object obj)
	{
		return obj is LevelProxy && this == (LevelProxy)obj;
	}

	public override int GetHashCode()
	{
		if (!this.cachedHashCodeExists)
		{
			this.cachedHashCode = this.CalculateHash();
			this.cachedHashCodeExists = true;
		}
		return this.cachedHashCode;
	}

	private int CalculateHash()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(this.mapIdentifier);
		for (int i = 0; i < this.indexPath.Length; i++)
		{
			stringBuilder.Append("_" + this.indexPath[i].ToString());
		}
		return stringBuilder.ToString().GetHashCode();
	}

	public int NumberOfStarsFromPoints(int points)
	{
		int[] starThresholds = this.StarThresholds;
		if (points >= starThresholds[2])
		{
			return 3;
		}
		if (points >= starThresholds[1])
		{
			return 2;
		}
		if (points >= starThresholds[0])
		{
			return 1;
		}
		return 0;
	}

	public int Index
	{
		get
		{
			return (this.indexPath != null && this.indexPath.Length != 0) ? this.indexPath.Last<int>() : -1;
		}
	}

	public string DisplayName
	{
		get
		{
			return this.HumanNumber.ToString();
		}
	}

	private ILevelDatabase GetRootLevelDatabase()
	{
		return ManagerRepository.Get<LevelDatabaseCollection>().GetLevelDatabase<LevelDatabase>(this.mapIdentifier);
	}

	private bool TryToTraverseAndLoadCollections(List<ILevelCollection> levelCollections, bool includeRoot = true)
	{
		ILevelDatabase rootLevelDatabase = this.GetRootLevelDatabase();
		if (includeRoot)
		{
			levelCollections.Add(rootLevelDatabase);
		}
		ILevelCollection levelCollection = rootLevelDatabase;
		for (int i = 0; i < this.indexPath.Length - 1; i++)
		{
			int num = this.indexPath[i];
			if (num < 0 || num >= levelCollection.LevelStubs.Count)
			{
				return false;
			}
			LevelStub levelStub = levelCollection.LevelStubs[this.indexPath[i]];
			levelStub.Load(levelCollection);
			ILevelCollection levelCollection2 = levelStub.GetLevelAsset() as ILevelCollection;
			levelCollections.Add(levelCollection2);
			levelCollection = levelCollection2;
		}
		return true;
	}

	public string[] AnalyticsDescriptors
	{
		get
		{
			List<string> list = new List<string>();
			List<ILevelCollection> list2 = new List<ILevelCollection>();
			if (this.TryToTraverseAndLoadCollections(list2, true))
			{
				foreach (ILevelCollection levelCollection in list2)
				{
					list.Add(levelCollection.GetAnalyticsDescriptor());
				}
			}
			return list.ToArray();
		}
	}

	public string LevelCollectionPath
	{
		get
		{
			List<string> list = new List<string>();
			List<ILevelCollection> list2 = new List<ILevelCollection>();
			if (this.TryToTraverseAndLoadCollections(list2, false))
			{
				foreach (ILevelCollection levelCollection in list2)
				{
					list.Add(levelCollection.GetAnalyticsDescriptor());
				}
			}
			if (list.Count > 0)
			{
				return string.Join("/", list.ToArray());
			}
			return string.Empty;
		}
	}

	public LevelDatabase RootDatabase
	{
		get
		{
			return this.GetRootLevelDatabase() as LevelDatabase;
		}
	}

	public LevelProxy PreviousLevel
	{
		get
		{
			List<int> list = new List<int>(this.indexPath);
			List<int> list2;
			int index;
			(list2 = list)[index = list.Count - 1] = list2[index] - 1;
			return new LevelProxy(this.mapIdentifier, list.ToArray());
		}
	}

	public LevelProxy NextLevel
	{
		get
		{
			List<int> list = new List<int>(this.indexPath);
			List<int> list2;
			int index;
			(list2 = list)[index = list.Count - 1] = list2[index] + 1;
			return new LevelProxy(this.mapIdentifier, list.ToArray());
		}
	}

	public int HumanNumber
	{
		get
		{
			return this.LevelCollection.GetHumanNumber(this);
		}
	}

	public ILevelCollection LevelCollection
	{
		get
		{
			ILevelCollection result = null;
			List<ILevelCollection> list = new List<ILevelCollection>();
			if (this.TryToTraverseAndLoadCollections(list, true))
			{
				foreach (ILevelCollection levelCollection in list)
				{
					result = levelCollection;
				}
			}
			return result;
		}
	}

	public LevelMetaData LevelMetaData
	{
		get
		{
			return this.LevelCollection.GetLevelMetaData(this.Index);
		}
	}

	public string LevelAssetName
	{
		get
		{
			if (!this.IsValid)
			{
				return "Invalid";
			}
			return Path.GetFileNameWithoutExtension(this.LevelStub.AssetResourcePath);
		}
	}

	public LevelDifficulty LevelDifficulty
	{
		get
		{
			if (!this.IsValid)
			{
				return LevelDifficulty.Normal;
			}
			if (this.indexPath.Length == 1)
			{
				return ((LevelDatabase)this.GetRootLevelDatabase()).GetLevelDifficulty(this.indexPath[0]);
			}
			return LevelDifficulty.Normal;
		}
	}

	private LevelStub LevelStub
	{
		get
		{
			return this.LevelCollection.LevelStubs[this.Index];
		}
	}

	public bool IsCompleted
	{
		
		get
		{
			return true; //TODO remove
			return this.Points > 0;
		}
	}

	public bool IsUnlocked
	{
		get
		{
			return true; //TODO remove
			return this.Index == 0 || this.PreviousLevel.IsCompleted;
		}
	}

	public int Points
	{
		get
		{
			ILevelAccomplishment levelData = this.GetLevelData(false);
			return (levelData == null) ? 0 : levelData.Points;
		}
	}

	private void SetPoints(int points)
	{
		ILevelAccomplishment levelData = this.GetLevelData(true);
		if (levelData != null)
		{
			levelData.Points = points;
		}
	}

	public int Stars
	{
		get
		{
			ILevelAccomplishment levelData = this.GetLevelData(false);
			return (levelData == null) ? 0 : levelData.Stars;
		}
	}

	private void SetStars(int stars)
	{
		ILevelAccomplishment levelData = this.GetLevelData(true);
		if (levelData != null)
		{
			levelData.Stars = stars;
		}
	}

	public int[] StarThresholds
	{
		get
		{
			return (this.LevelMetaData == null) ? new int[3] : this.LevelStub.starThresholds;
		}
	}

	public ILevelAccomplishment GetLevelData(bool createIfNotExisting)
	{
		return this.GetRootLevelDatabase().GetLevelData(createIfNotExisting, this);
	}

	public void SaveSessionAccomplishment(int newPoints, bool forceOverWriteResult = false)
	{
		int num = this.NumberOfStarsFromPoints(newPoints);
		if (!forceOverWriteResult)
		{
			if (newPoints > this.Points)
			{
				this.SetPoints(newPoints);
			}
			if (num > this.Stars)
			{
				this.SetStars(num);
			}
		}
		else
		{
			this.SetPoints(newPoints);
			this.SetStars(num);
		}
		this.RootDatabase.Save();
	}

	public void CheatComplete()
	{
		ILevelAccomplishment levelData = this.GetLevelData(true);
		levelData.Points = 100;
		levelData.Stars = 3;
	}

	public void CheatUnComplete()
	{
		ILevelAccomplishment levelData = this.GetLevelData(true);
		levelData.Points = 0;
		levelData.Stars = 0;
	}

	public LevelCollectionEntry LevelAsset
	{
		get
		{
			if (!this.IsValid)
			{
				throw new InvalidOperationException("Level Proxy is invalid, unable to load!");
			}
			this.LevelStub.Load(this.LevelCollection);
			if (!this.IsValid)
			{
				throw new InvalidOperationException("Level Proxy is invalid, unable to retrieve LevelAsset!");
			}
			return this.LevelCollection.LevelStubs[this.Index].GetLevelAsset();
		}
	}

	public static readonly LevelProxy Invalid = new LevelProxy(MapIdentifier.Empty, new int[]
	{
		-1
	});

	private const int INVALID_INDEX = -1;

	private const string EDITOR_LEVELDATABASE_IDENTIFIER = "EditorLevelCollection";

	private readonly MapIdentifier mapIdentifier;

	private readonly int[] indexPath;

	private int cachedHashCode;

	private bool cachedHashCodeExists;
}
