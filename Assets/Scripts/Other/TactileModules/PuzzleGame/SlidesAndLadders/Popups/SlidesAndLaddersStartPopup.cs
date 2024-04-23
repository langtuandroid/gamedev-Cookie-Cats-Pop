using System;
using TactileModules.FeatureManager;
using TactileModules.MapFeature;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGame.SlidesAndLadders.Data;
using TactileModules.PuzzleGame.SlidesAndLadders.Model;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Popups
{
    public class SlidesAndLaddersStartPopup : MapFeatureStartPopup
    {
        public SlidesAndLaddersStartPopup(TactileModules.FeatureManager.FeatureManager featureManager, SlidesAndLaddersHandler featureHandler, ConfigProvider<SlidesAndLaddersConfig> config, IMainProgression mainProgressionManager) : base(featureManager, featureHandler)
        {
            this.config = config;
            this.mainProgressionManager = mainProgressionManager;
        }

        protected override bool ShouldShowPopup()
        {
            return this.config.Get().LevelRequired <= this.mainProgressionManager.GetFarthestCompletedLevelHumanNumber();
        }

        private readonly IMainProgression mainProgressionManager;

        private readonly ConfigProvider<SlidesAndLaddersConfig> config;
    }
}
