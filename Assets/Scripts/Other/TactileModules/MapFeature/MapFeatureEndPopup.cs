using System;
using System.Collections;
using TactileModules.FeatureManager;
using TactileModules.FeatureManager.Interfaces;

namespace TactileModules.MapFeature
{
    public abstract class MapFeatureEndPopup : MapPopupManager.IMapPopup
    {
        protected MapFeatureEndPopup(TactileModules.FeatureManager.FeatureManager featureManager, MapFeatureHandler featureHandler)
        {
            this.featureManager = featureManager;
            this.featureHandler = featureHandler;
            MapPopupManager.Instance.RegisterPopupObject(this);
        }

        private bool ShouldEnd()
        {
            return this.featureManager.ShouldDeactivateFeature(this.featureHandler as IFeatureTypeHandler);
        }

        private bool MapFeatureShouldShowPopup()
        {
            return this.ShouldEnd() && this.featureHandler.IsParticipating && this.ShouldShowPopup();
        }

        protected abstract bool ShouldShowPopup();

        public void TryShowPopup(int unlockedLevelIndex, MapPopupManager.PopupFlow popupFlow)
        {
            if (this.MapFeatureShouldShowPopup())
            {
                popupFlow.AddPopup(this.ShowPopup());
            }
            else if (this.ShouldEnd())
            {
                popupFlow.AddSilentAction(delegate
                {
                    this.featureManager.DeactivateFeature(this.featureHandler as IFeatureTypeHandler);
                });
            }
        }

        private IEnumerator ShowPopup()
        {
            this.featureManager.DeactivateFeature(this.featureHandler as IFeatureTypeHandler);
            yield return this.featureHandler.ShowEndPopup();
            yield break;
        }

        private readonly TactileModules.FeatureManager.FeatureManager featureManager;

        private readonly MapFeatureHandler featureHandler;
    }
}
