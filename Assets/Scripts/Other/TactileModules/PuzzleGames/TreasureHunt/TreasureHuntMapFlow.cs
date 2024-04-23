using System;
using System.Collections;
using System.Collections.Generic;
using TactileModules.FeatureManager;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.SagaCore;

namespace TactileModules.PuzzleGames.TreasureHunt
{
    public class TreasureHuntMapFlow : MapFlow
    {
        public TreasureHuntMapFlow(MapIdentifier mapIdentifier, MapFacade mapFacade, IFullScreenManager fullScreenManager, IFlowStack flowStack, TreasureHuntManager manager, IPlayFlowFactory playFlowFactory) : base(mapIdentifier, mapFacade, fullScreenManager, flowStack)
        {
            this.manager = manager;
            this.playFlowFactory = playFlowFactory;
        }

        protected override IEnumerator PostPlayFlowSequence(int nextDotIndexToOpen)
        {
            yield return base.AnimateAvatarProgressIfAny();
            bool isReward = false;
            LevelProxy nextLevel = this.manager.TreasureHuntLevelDatabase.GetLevel(nextDotIndexToOpen);
            if (nextLevel.IsValid)
            {
                isReward = (nextLevel.LevelAsset is RewardLevelAsset);
            }
            int secondsLeft = this.manager.SecondsLeft;
            if (nextLevel.IsValid && !isReward && secondsLeft > 0)
            {
                base.StartFlowForDot(nextLevel.Index);
            }
            else if (isReward && !this.CustomData.rewardClaimed)
            {
                this.CustomData.rewardClaimed = true;
                this.manager.Save();
                yield return this.manager.ClaimReward();
                base.EndThisFlow();
                yield break;
            }
            if (secondsLeft <= 0 && !this.CustomData.rewardClaimed && this.manager.HasActiveFeature())
            {
                TactileModules.FeatureManager.FeatureManager.Instance.DeactivateFeature(this.manager);
                yield return this.manager.ShowEventEndedView();
                base.EndThisFlow();
            }
            yield break;
        }

        protected override IEnumerator AfterScreenAcquired()
        {
            UIViewManager.UIViewStateGeneric<TreasureHuntMapButtonView> uiviewStateGeneric = UIViewManager.Instance.ShowView<TreasureHuntMapButtonView>(new object[0]);
            uiviewStateGeneric.View.ExitClicked += base.EndThisFlow;
            yield break;
        }

        protected override void AfterScreenLost()
        {
        }

        protected override int GetFarthestUnlockedDotIndex()
        {
            return this.manager.FarthestCompletedLevel + 1;
        }

        protected override IFlow CreateFlowForDot(int dotIndex)
        {
            if (dotIndex == this.GetFarthestUnlockedDotIndex())
            {
                return new TreasureHuntPlayFlow(this.manager, this.manager.GetNextLevel, this.playFlowFactory);
            }
            this.manager.GetFeatureInstanceCustomData<TreasureHuntInstanceCustomData, TreasureHuntMetaData, FeatureTypeCustomData>().farthestCompletedLevel = dotIndex;
            base.MapContentController.Refresh();
            return null;
        }

        public override SagaAvatarInfo CreateMeAvatarInfo()
        {
            return new SagaAvatarInfo
            {
                dotIndex = this.manager.GetNextLevel.Index
            };
        }

        public override Dictionary<CloudUser, SagaAvatarInfo> CreateFriendsAvatarInfos()
        {
            return null;
        }

        private TreasureHuntInstanceCustomData CustomData
        {
            get
            {
                return this.manager.GetFeatureInstanceCustomData<TreasureHuntInstanceCustomData, TreasureHuntMetaData, FeatureTypeCustomData>();
            }
        }

        private readonly TreasureHuntManager manager;

        private readonly IPlayFlowFactory playFlowFactory;
    }
}
