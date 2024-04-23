using System;
using TactileModules.FeatureManager;
using UnityEngine;

namespace TactileModules.PuzzleGames.TreasureHunt
{
    public abstract class TreasureHuntMapDot : MapDotBase
    {
        [Instantiator.SerializeProperty]
        public override int LevelId
        {
            get
            {
                return this.levelId;
            }
            set
            {
                this.levelId = value;
                if (Application.isPlaying)
                {
                    this.UpdateUI();
                }
            }
        }

        private TreasureHuntManager Manager
        {
            get
            {
                return TactileModules.FeatureManager.FeatureManager.GetFeatureHandler<TreasureHuntManager>();
            }
        }

        protected LevelProxy LevelProxy
        {
            get
            {
                return this.Manager.TreasureHuntLevelDatabase.GetLevel(this.LevelId);
            }
        }

        public override bool IsUnlocked
        {
            get
            {
                return this.levelId <= this.Manager.FarthestCompletedLevel + 1;
            }
        }

        public override bool IsCompleted
        {
            get
            {
                return this.levelId <= this.Manager.FarthestCompletedLevel;
            }
        }

        public override void Initialize()
        {
        }

        private int levelId;
    }
}
