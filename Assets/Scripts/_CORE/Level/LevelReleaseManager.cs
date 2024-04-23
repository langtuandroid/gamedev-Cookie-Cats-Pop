using System;
using System.Collections;
using System.Diagnostics;
using ConfigSchema;
using JetBrains.Annotations;
using TactileModules.FeatureManager;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;
using TactileModules.PuzzleGame.MainLevels;

public sealed class LevelReleaseManager : IFeatureTypeHandler<FeatureInstanceCustomData, LevelReleaseManager.LevelReleaseFeatureMetaData, FeatureTypeCustomData>, ILevelAmountProvider, IFeatureTypeHandler
{
	public LevelReleaseManager([NotNull] FeatureManager featureManager, MainProgressionManager mainProgressionManager, LevelReleaseManager.ILevelReleaseManagerProvider provider)
	{
		if (featureManager == null)
		{
			throw new ArgumentNullException("featureManager");
		}
		this.featureManager = featureManager;
		this.provider = provider;
		mainProgressionManager.RegisterLevelAmountProvider(this);
		featureManager.OnFeatureListUpdated += this.TryToActivateNewFeature;
	}

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action OnFeatureUpdated;

	public int MaxAvailableHumanNumber { get; private set; }

	private void TryToActivateNewFeature()
	{
		FeatureData feature = this.featureManager.GetFeature(this);
		if (feature != null)
		{
			ActivatedFeatureInstanceData activatedFeature = this.GetActivatedFeature();
			if (activatedFeature != null && activatedFeature.Id != feature.Id)
			{
				this.featureManager.DeactivateFeature(this);
			}
			if (this.GetActivatedFeature() == null)
			{
				this.featureManager.ActivateFeature(this, feature);
				if (this.OnFeatureUpdated != null)
				{
					this.OnFeatureUpdated();
				}
			}
		}
	}

	public void FeatureRemovedFromDashboard(ActivatedFeatureInstanceData activatedFeatureInstanceData)
	{
		if (this.GetActivatedFeature() != null)
		{
			this.featureManager.DeactivateFeature(this);
		}
	}

	public void FadeToBlack()
	{
		ActivatedFeatureInstanceData activatedFeature = this.GetActivatedFeature();
		if (activatedFeature != null)
		{
			this.MaxAvailableHumanNumber = activatedFeature.GetMetaData<FeatureInstanceCustomData, LevelReleaseManager.LevelReleaseFeatureMetaData, FeatureTypeCustomData>(this).MaxAvailableHumanNumber;
		}
	}

	public string FeatureType
	{
		get
		{
			return "level-release";
		}
	}

	public bool AllowMultipleFeatureInstances
	{
		get
		{
			return false;
		}
	}

	public int MetaDataVersion
	{
		get
		{
			return 3;
		}
	}

	public Hashtable UpgradeMetaData(Hashtable metaData, int fromVersion, int toVersion)
	{
		if (fromVersion == 0 && toVersion == 1)
		{
			return metaData;
		}
		if (fromVersion == 1 && toVersion == 2)
		{
			return metaData;
		}
		if (fromVersion != 2 || toVersion != 3)
		{
			throw new NotSupportedException();
		}
		if (metaData.ContainsKey("maxAvailableLevel"))
		{
			int index = Convert.ToInt32(metaData["maxAvailableLevel"]);
			LevelReleaseManager.LevelReleaseFeatureMetaData obj = new LevelReleaseManager.LevelReleaseFeatureMetaData
			{
				MaxAvailableHumanNumber = this.provider.GetHumanNumber(index)
			};
			return JsonSerializer.ObjectToHashtable(obj);
		}
		if (metaData.ContainsKey("maxAvailableHumanNumber"))
		{
			return metaData;
		}
		throw new Exception("Current meta data did not contain expected data");
	}

	public FeatureInstanceCustomData NewFeatureInstanceCustomData(FeatureData featureData)
	{
		return new FeatureInstanceCustomData();
	}

	public FeatureTypeCustomData NewFeatureTypeCustomData()
	{
		return new FeatureTypeCustomData();
	}

	public void MergeFeatureInstanceStates<FeatureState>(ref FeatureState toMerge, FeatureState current, FeatureState cloud) where FeatureState : FeatureInstanceCustomData
	{
	}

	public void MergeFeatureTypeState<HandlerState>(ref HandlerState toMerge, HandlerState current, HandlerState cloud) where HandlerState : FeatureTypeCustomData
	{
	}

	int ILevelAmountProvider.GetMaxAvailableHumanNumber()
	{
		return this.MaxAvailableHumanNumber;
	}

	private readonly FeatureManager featureManager;

	public readonly LevelReleaseManager.ILevelReleaseManagerProvider provider;

	[RequireAll]
	public class LevelReleaseFeatureMetaData : FeatureMetaData
	{
		[Description("The max available level index currently available")]
		[JsonSerializable("maxAvailableHumanNumber", null)]
		public int MaxAvailableHumanNumber { get; set; }
	}

	public interface ILevelReleaseManagerProvider
	{
		int GetHumanNumber(int index);
	}
}
