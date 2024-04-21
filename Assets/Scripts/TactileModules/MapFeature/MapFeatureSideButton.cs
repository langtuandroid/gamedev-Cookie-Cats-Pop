using System;
using TactileModules.FeatureManager;
using TactileModules.FeatureManager.Interfaces;
using TactileModules.Foundation;
using UnityEngine;

namespace TactileModules.MapFeature
{
    public abstract class MapFeatureSideButton : SideMapButton
    {
        protected override void UpdateOncePerSecond()
        {
            if (!this.VisibilityChecker(null))
            {
                return;
            }
            this.timerLabel.text = this.MapFeatureHandler.GetTimeLeftAsText();
        }

        protected new void Clicked(UIEvent e)
        {
            int stabilizedTimeLeftToFeatureDurationEnd = this.FeatureManager.GetStabilizedTimeLeftToFeatureDurationEnd(this.MapFeatureHandler as IFeatureTypeHandler);
            if (stabilizedTimeLeftToFeatureDurationEnd > 0)
            {
                this.MapFeatureHandler.SwitchToFeatureMapView();
            }
        }

        public override SideMapButton.AreaSide Side
        {
            get
            {
                return SideMapButton.AreaSide.Left;
            }
        }

        public override bool VisibilityChecker(object data)
        {
            IFeatureTypeHandler featureTypeHandler = this.MapFeatureHandler as IFeatureTypeHandler;
            return this.FeatureManager.HasActiveFeature(featureTypeHandler) && this.FeatureManager.GetStabilizedTimeLeftToFeatureDurationEnd(featureTypeHandler) > 0 && this.VisibilityCheckerImplementor(data);
        }

        protected abstract bool VisibilityCheckerImplementor(object data);

        protected abstract MapFeatureHandler MapFeatureHandler { get; }

        private TactileModules.FeatureManager.FeatureManager FeatureManager
        {
            get
            {
                return ManagerRepository.Get<TactileModules.FeatureManager.FeatureManager>();
            }
        }

        public override Vector2 Size
        {
            get
            {
                return this.GetElementSize();
            }
        }

        [SerializeField]
        private UILabel timerLabel;
    }
}
