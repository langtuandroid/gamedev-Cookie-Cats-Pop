using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using TactileModules.FeatureManager;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.SagaCore;

namespace Shared.OneLifeChallenge
{
    public class OneLifeChallengeMapFlow : MapFlow
    {
        public OneLifeChallengeMapFlow(OneLifeChallengeManager oneLifeChallengeManager, IPlayFlowFactory playLevelSystem, MapFacade mapFacade, IFullScreenManager fullScreenManager, IFlowStack flowStack) : base("OneLifeChallenge", mapFacade, fullScreenManager, flowStack)
        {
            this.oneLifeChallengeManager = oneLifeChallengeManager;
            this.playLevelSystem = playLevelSystem;
        }

        protected override IEnumerator AfterScreenAcquired()
        {
            this.buttonsView = UIViewManager.Instance.ShowView<OneLifeChallengeMapButtonsView>(new object[0]).View;
            this.buttonsView.Initialize(this.oneLifeChallengeManager);
            this.buttonsView.ExitClicked += base.EndThisFlow;
            yield break;
        }

        protected override void AfterScreenLost()
        {
            this.timeCheckFiber.Terminate();
            UIViewLayer viewLayerWithView = UIViewManager.Instance.GetViewLayerWithView(this.buttonsView);
            viewLayerWithView.CloseInstantly();
        }

        public override SagaAvatarInfo CreateMeAvatarInfo()
        {
            return new SagaAvatarInfo
            {
                dotIndex = this.GetFarthestUnlockedDotIndex()
            };
        }

        public override Dictionary<CloudUser, SagaAvatarInfo> CreateFriendsAvatarInfos()
        {
            return null;
        }

        protected override int GetFarthestUnlockedDotIndex()
        {
            return this.oneLifeChallengeManager.GetNextLevel.Index;
        }

        protected override IFlow CreateFlowForDot(int dotIndex)
        {
            if (dotIndex == this.GetFarthestUnlockedDotIndex())
            {
                return new OneLifeChallengePlayFlow(this.oneLifeChallengeManager, this.playLevelSystem);
            }
            return null;
        }

        protected override void OnApplicationWillEnterForeground()
        {
            if (this.timeCheckFiber.IsTerminated && this.HasTimedOut)
            {
                this.timeCheckFiber.Start(this.TimeoutSequence());
            }
        }

        private bool HasTimedOut
        {
            get
            {
                OneLifeChallengeInstanceCustomData featureInstanceCustomData = this.oneLifeChallengeManager.GetFeatureInstanceCustomData<OneLifeChallengeInstanceCustomData, FeatureMetaData, FeatureTypeCustomData>();
                return this.oneLifeChallengeManager.SecondsLeft <= 0 && TactileModules.FeatureManager.FeatureManager.Instance.HasActiveFeature(this.oneLifeChallengeManager) && featureInstanceCustomData != null && !featureInstanceCustomData.RewardClaimed;
            }
        }

        private IEnumerator TimeoutSequence()
        {
            this.oneLifeChallengeManager.DeactivateFeature();
            yield return this.oneLifeChallengeManager.provider.ShowEventEndedView();
            base.EndThisFlow();
            yield break;
        }

        protected override IEnumerator PostPlayFlowSequence(int nextDotIndexToOpen)
        {
            yield return base.MapContentController.Avatars.AnimateProgressIfAny();
            if (this.oneLifeChallengeManager.EventCompleted && !this.oneLifeChallengeManager.GetFeatureInstanceCustomData<OneLifeChallengeInstanceCustomData, FeatureMetaData, FeatureTypeCustomData>().RewardClaimed)
            {
                this.oneLifeChallengeManager.GetFeatureInstanceCustomData<OneLifeChallengeInstanceCustomData, FeatureMetaData, FeatureTypeCustomData>().RewardClaimed = true;
                PuzzleGameData.UserSettings.SaveLocal();
                yield return this.oneLifeChallengeManager.ClaimReward();
                base.EndThisFlow();
            }
            else if (this.HasTimedOut)
            {
                yield return this.TimeoutSequence();
            }
            else if (nextDotIndexToOpen >= 0)
            {
                base.StartFlowForDot(nextDotIndexToOpen);
            }
            yield break;
        }

        private readonly OneLifeChallengeManager oneLifeChallengeManager;

        private readonly IPlayFlowFactory playLevelSystem;

        private OneLifeChallengeMapButtonsView buttonsView;

        private readonly Fiber timeCheckFiber = new Fiber();
    }
}
