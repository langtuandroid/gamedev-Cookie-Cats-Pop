using System;
using System.Collections;
using Fibers;
using TactileModules.FeatureManager;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;

namespace TactileModules.MapFeature
{
    public abstract class MapFeatureStartPopup : MapPopupManager.IMapPopup
    {
        protected MapFeatureStartPopup(TactileModules.FeatureManager.FeatureManager featureManager, MapFeatureHandler featureHandler)
        {
            this.featureManager = featureManager;
            this.featureHandler = featureHandler;
            MapPopupManager.Instance.RegisterPopupObject(this);
        }

        private bool MapFeatureShouldShowPopup()
        {
            return this.ShouldShowPopup() && this.CanActivateFeature(this.featureHandler as IFeatureTypeHandler);
        }

        protected abstract bool ShouldShowPopup();

        protected virtual bool CanActivateFeature(IFeatureTypeHandler featureHandler)
        {
            return this.featureManager.CanActivateFeature(featureHandler);
        }

        protected virtual void ActivateFeature(FeatureData featureData)
        {
            this.featureManager.ActivateFeature(this.featureHandler as IFeatureTypeHandler, featureData);
        }

        private IEnumerator ShowPopup(FeatureData featureData)
        {
            this.ActivateFeature(featureData);
            EnumeratorResult<bool> result = new EnumeratorResult<bool>();
            yield return this.featureHandler.ShowStartPopup(result);
            if (result.value)
            {
                this.featureHandler.SwitchToFeatureMapView();
            }
            yield break;
        }

        public void TryShowPopup(int unlockedLevelIndex, MapPopupManager.PopupFlow popupFlow)
        {
            if (this.MapFeatureShouldShowPopup())
            {
                FeatureData feature = this.featureManager.GetFeature(this.featureHandler as IFeatureTypeHandler);
                popupFlow.AddPopup(this.ShowPopup(feature));
            }
        }

        private readonly TactileModules.FeatureManager.FeatureManager featureManager;

        private readonly MapFeatureHandler featureHandler;
    }
}
