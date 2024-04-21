using System;
using System.Diagnostics;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;

namespace TactileModules.FeatureManager
{
	public abstract class FeatureActivationBase<T> : IFeatureActivation where T : FeatureMetaData
	{
		public FeatureActivationBase(FeatureManager featureManager, IFeatureTypeHandler featureHandler)
		{
			this.featureManager = featureManager;
			this.featureHandler = featureHandler;
			featureManager.OnFeatureDeactivated += this.HandleFeatureDeactivated;
		}

		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<ActivatedFeatureInstanceData> FeatureActivated;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<ActivatedFeatureInstanceData> FeatureDeactivated;



		private void HandleFeatureDeactivated(ActivatedFeatureInstanceData data)
		{
			if (data.FeatureData.Type == this.featureHandler.FeatureType)
			{
				this.FeatureDeactivated(data);
			}
		}

		public void ActivateFeature()
		{
			FeatureData feature = this.featureManager.GetFeature(this.featureHandler);
			ActivatedFeatureInstanceData activatedFeatureInstanceData = this.featureManager.ActivateFeature(this.featureHandler, feature);
			this.SetupFeatureInstance(activatedFeatureInstanceData);
			this.FeatureActivated(activatedFeatureInstanceData);
		}

		public void ActivateLocalFeature()
		{
			ActivatedFeatureInstanceData activatedFeatureInstanceData = this.featureManager.ActivateLocalFeature(this.featureHandler, this.LocalFeatureDuration);
			this.SetupFeatureInstance(activatedFeatureInstanceData);
			this.FeatureActivated(activatedFeatureInstanceData);
		}

		public void DeactivateFeature()
		{
			this.featureManager.DeactivateFeature(this.featureHandler);
		}

		public bool ShouldActivateFeature()
		{
			if (!this.FeatureEnabled())
			{
				return false;
			}
			if (!this.featureManager.CanActivateFeature(this.featureHandler))
			{
				return false;
			}
			FeatureData availableFeature = this.featureManager.GetAvailableFeature(this.featureHandler);
			T featureInstanceMetaData = this.featureManager.GetFeatureInstanceMetaData<T>(availableFeature);
			return this.ShouldActivateFeatureInternal(featureInstanceMetaData);
		}

		public bool ShouldActivateLocalFeature(int unlockedLevelIndex)
		{
			return this.FeatureEnabled() && !this.HasActiveFeature() && this.ShouldActivateLocalFeatureInternal(unlockedLevelIndex);
		}

		public bool ShouldDeactivateFeature()
		{
			return this.featureManager.ShouldDeactivateFeature(this.featureHandler);
		}

		public bool HasActiveFeature()
		{
			return this.featureManager.HasActiveFeature(this.featureHandler);
		}

		public int GetSecondsLeft()
		{
			return this.featureManager.GetStabilizedTimeLeftToFeatureDurationEnd(this.featureHandler);
		}

		protected abstract int LocalFeatureDuration { get; }

		public abstract bool FeatureEnabled();

		protected abstract bool ShouldActivateFeatureInternal(T metaData);

		protected abstract bool ShouldActivateLocalFeatureInternal(int unlockedLevelIndex);

		protected abstract void SetupFeatureInstance(ActivatedFeatureInstanceData activatedFeatureInstance);

		protected readonly FeatureManager featureManager;

		protected readonly IFeatureTypeHandler featureHandler;
	}
}
