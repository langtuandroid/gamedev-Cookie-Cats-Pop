using System;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;
using TactileModules.PuzzleGame.ScheduledBooster.Data;

namespace TactileModules.PuzzleGame.ScheduledBooster.Model
{
	public class ScheduledBooster : IScheduledBooster
	{
		public ScheduledBooster(IFeatureManager featureManager, ActivatedFeatureInstanceData featureInstanceData, ScheduledBoosterDefinition definition)
		{
			this.featureManager = featureManager;
			this.FeatureInstanceData = featureInstanceData;
			this.Definition = definition;
		}

		public bool IsActive
		{
			get
			{
				return this.isActive;
			}
		}

		public ActivatedFeatureInstanceData FeatureInstanceData { get; private set; }

		public ScheduledBoosterDefinition Definition { get; private set; }

		public string Type
		{
			get
			{
				return this.GetMetaData().ScheduledBoosterType;
			}
		}

		public int Price
		{
			get
			{
				return this.GetMetaData().BoosterPrice;
			}
		}

		public int GetSecondsLeft()
		{
			return this.featureManager.GetStabilizedTimeLeftToFeatureDurationEnd(this.FeatureInstanceData);
		}

		public string GetTimeRemainingAsFormattedString()
		{
			return L.FormatSecondsAsColumnSeparated(this.GetSecondsLeft(), L.Get("Ended"), TimeFormatOptions.None);
		}

		public ScheduledBoosterInstanceCustomData GetInstanceCustomData()
		{
			return (ScheduledBoosterInstanceCustomData)this.FeatureInstanceData.GetCustomInstanceData();
		}

		public ScheduledBoosterMetaData GetMetaData()
		{
			return this.FeatureInstanceData.GetMetaData<ScheduledBoosterMetaData>();
		}

		public bool IsFree()
		{
			return this.GetInstanceCustomData().NumberOfBoostersUsed < 1;
		}

		public void Activate()
		{
			this.isActive = true;
		}

		public void Deactivate()
		{
			this.isActive = false;
		}

		private bool isActive;

		private readonly IFeatureManager featureManager;
	}
}
