using System;
using System.Collections;
using Fibers;
using TactileModules.FeatureManager;
using TactileModules.FeatureManager.Interfaces;

namespace TactileModules.MapFeature
{
    public abstract class MapFeatureSessionStartPopup : MapPopupManager.IMapPopup
    {
        protected MapFeatureSessionStartPopup(TactileModules.FeatureManager.FeatureManager featureManager, MapFeatureHandler featureHandler)
        {
            this.featureManager = featureManager;
            this.featureHandler = featureHandler;
            MapPopupManager.Instance.RegisterPopupObject(this);
        }

        private bool MapFeatureShouldShowPopup()
        {
            IFeatureTypeHandler featureTypeHandler = this.featureHandler as IFeatureTypeHandler;
            return this.ShouldShowPopup() && this.featureManager.HasActiveFeature(featureTypeHandler) && this.featureManager.GetStabilizedTimeLeftToFeatureDurationEnd(featureTypeHandler) > 0 && !this.featureManager.ShouldDeactivateFeature(featureTypeHandler);
        }

        protected abstract bool ShouldShowPopup();

        public void TryShowPopup(int unlockedLevelIndex, MapPopupManager.PopupFlow popupFlow)
        {
            if (this.MapFeatureShouldShowPopup())
            {
                popupFlow.AddPopup(this.ShowPopup());
            }
        }

        private IEnumerator ShowPopup()
        {
            EnumeratorResult<bool> result = new EnumeratorResult<bool>();
            yield return this.featureHandler.ShowStartSessionPopup(result);
            if (result.value)
            {
                this.featureHandler.SwitchToFeatureMapView();
            }
            yield break;
        }

        private readonly TactileModules.FeatureManager.FeatureManager featureManager;

        private readonly MapFeatureHandler featureHandler;
    }
}
