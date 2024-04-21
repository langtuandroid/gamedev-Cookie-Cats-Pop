using System;
using System.Collections;
using Fibers;
using TactileModules.FeatureManager;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;

namespace TactileModules.MapFeature
{
	public abstract class MapFeatureHandler<T, U, V> : MapFeatureHandler, IFeatureTypeHandler<T, U, V>, IFeatureTypeHandler where T : MapFeatureInstanceCustomData where U : FeatureMetaData where V : FeatureTypeCustomData
	{
		private MapFeatureInstanceCustomData MapFeatureCustomData
		{
			get
			{
				return this.GetFeatureInstanceCustomData<T, U, V>();
			}
		}

		public override bool IsParticipating
		{
			get
			{
				MapFeatureInstanceCustomData mapFeatureCustomData = this.MapFeatureCustomData;
				return mapFeatureCustomData != null && mapFeatureCustomData.IsParticipating;
			}
		}

		public override void SwitchToFeatureMapView()
		{
			this.MapFeatureCustomData.IsParticipating = true;
			this.MapFeatureProvider.Save();
			this.MapFeatureProvider.SwitchToFeatureMapView();
		}

		public override IEnumerator ShowEndPopup()
		{
			yield return this.MapFeatureProvider.ShowFeatureEndedView();
			yield break;
		}

		public override IEnumerator ShowStartPopup(EnumeratorResult<bool> viewClosingResult)
		{
			yield return this.MapFeatureProvider.ShowFeatureStartView(viewClosingResult);
			yield break;
		}

		public override IEnumerator ShowStartSessionPopup(EnumeratorResult<bool> viewClosingResult)
		{
			yield return this.MapFeatureProvider.ShowFeatureStartSessionView(viewClosingResult);
			yield break;
		}

		protected abstract IMapFeatureProvider MapFeatureProvider { get; }

		public abstract void FeatureRemovedFromDashboard(ActivatedFeatureInstanceData activatedFeatureInstanceData);

		public abstract void FadeToBlack();

		public abstract string FeatureType { get; }

		public abstract bool AllowMultipleFeatureInstances { get; }

		public abstract int MetaDataVersion { get; }

		public abstract Hashtable UpgradeMetaData(Hashtable metaData, int fromVersion, int toVersion);

		public abstract T NewFeatureInstanceCustomData(FeatureData featureData);

		public abstract V NewFeatureTypeCustomData();

		public void MergeFeatureInstanceStates<FeatureState>(ref FeatureState toMerge, FeatureState current, FeatureState cloud) where FeatureState : T
		{
			toMerge.IsParticipating = (current.IsParticipating || cloud.IsParticipating);
			this.MergeFeatureInstanceStatesImplementor<FeatureState>(ref toMerge, current, cloud);
		}

		public void MergeFeatureTypeState<HandlerState>(ref HandlerState toMerge, HandlerState current, HandlerState cloud) where HandlerState : V
		{
			this.MergeFeatureTypeStateImplementor<HandlerState>(ref toMerge, current, cloud);
		}

		protected abstract void MergeFeatureInstanceStatesImplementor<FeatureState>(ref FeatureState toMerge, FeatureState current, FeatureState cloud) where FeatureState : T;

		protected abstract void MergeFeatureTypeStateImplementor<HandlerState>(ref HandlerState toMerge, HandlerState current, HandlerState cloud) where HandlerState : V;
	}
}
