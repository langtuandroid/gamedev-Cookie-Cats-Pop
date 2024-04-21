using System;
using System.Collections;
using System.Collections.Generic;
using Cloud;
using Fibers;
using Tactile;
using TactileModules.FeatureManager;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGames.EndlessChallenge.Data;
using TactileModules.PuzzleGames.EndlessChallenge.Popups;
using TactileModules.PuzzleGames.GameCore;
using UnityEngine;
using TactileModules.PuzzleGames.EndlessChallenge;


    public class EndlessChallengeHandler : IFeatureTypeHandler<EndlessChallengeInstanceCustomData, EndlessChallengeMetaData, EndlessChallengeCustomData>, IFeatureNotifications, IFeatureTypeHandler
    {
        public EndlessChallengeHandler(TactileModules.FeatureManager.FeatureManager featureManager, CloudClient cloudClient, LevelDatabaseCollection levelDatabaseCollection, ConfigurationManager configurationManager, UIViewManager uiViewManager, IFlowStack flowStack, IPlayFlowFactory playFlowFactory)
        {
            this.featureManager = featureManager;
            this.levelDatabaseCollection = levelDatabaseCollection;
            this.configurationManager = configurationManager;
            this.uiViewManager = uiViewManager;
            this.flowStack = flowStack;
            this.playFlowFactory = playFlowFactory;
            this.CloudClient = cloudClient;
            this.endlessChallengeCloud = new EndlessChallengeCloud(cloudClient);
            this.localPersistedState = new EndlessChallengeLocalPersistedState();
            new EndlessChallengeStartPopup(this);
            new EndlessChallengeReminderPopup(this);
            new EndlessChallengeEndPopup(this);
        }

        private EndlessChallengeInstanceCustomData FeatureInstanceData
        {
            get
            {
                if (!this.HasActiveFeature())
                {
                    return null;
                }
                return this.GetActivatedFeature().GetCustomInstanceData<EndlessChallengeInstanceCustomData, EndlessChallengeMetaData, EndlessChallengeCustomData>(this);
            }
        }

        private EndlessChallengeCustomData FeatureTypeData
        {
            get
            {
                return this.GetFeatureTypeCustomData<EndlessChallengeInstanceCustomData, EndlessChallengeMetaData, EndlessChallengeCustomData>();
            }
        }

        public CloudClient CloudClient { get; private set; }

        public string FeatureType
        {
            get
            {
                return "endless-challenge";
            }
        }

        public bool AllowMultipleFeatureInstances
        {
            get
            {
                return false;
            }
        }

        public int MetaDataVersion
        {
            get
            {
                return 1;
            }
        }

        public int HighestRow
        {
            get
            {
                return this.FeatureInstanceData.HighestRow;
            }
            private set
            {
                this.FeatureInstanceData.HighestRow = value;
            }
        }

        public int AllTimeHighestRow
        {
            get
            {
                return this.FeatureTypeData.AllTimeHighestRow;
            }
            private set
            {
                this.FeatureTypeData.AllTimeHighestRow = value;
            }
        }

        public string DeviceId
        {
            get
            {
                return (!this.CloudClient.HasValidDevice) ? string.Empty : this.CloudClient.CachedDevice.CloudId;
            }
        }

        public string UserId
        {
            get
            {
                return (!this.CloudClient.HasValidUser) ? string.Empty : this.CloudClient.CachedMe.CloudId;
            }
        }

        private EndlessChallengeLevelDatabase LevelDatabase
        {
            get
            {
                return this.levelDatabaseCollection.GetLevelDatabase<EndlessChallengeLevelDatabase>("EndlessChallenge");
            }
        }

        public LevelProxy EndlessLevel
        {
            get
            {
                return this.LevelDatabase.GetLevel(0);
            }
        }

        public bool IsEndlessChallengeActive
        {
            get
            {
                return this.Config.IsActive && this.HasActiveFeature();
            }
        }

        public EndlessChallengeConfig Config
        {
            get
            {
                return this.configurationManager.GetConfig<EndlessChallengeConfig>();
            }
        }

        public int SecondsLeft
        {
            get
            {
                return this.featureManager.GetStabilizedTimeLeftToFeatureDurationEnd(this);
            }
        }

        public void MapButtonClicked()
        {
            EnumeratorResult<UIViewManager.UIViewState> viewState = new EnumeratorResult<UIViewManager.UIViewState>();
            FiberCtrl.Pool.Run(this.TryShowLeaderboard(viewState), false);
        }

        public IEnumerator TryShowLeaderboard(EnumeratorResult<UIViewManager.UIViewState> viewState)
        {
            if (this.ShouldDeactivateFeature())
            {
                UIViewManager.UIViewStateGeneric<EndlessChallengeEndPopupView> vs = this.uiViewManager.ShowView<EndlessChallengeEndPopupView>(new object[0]);
                yield return vs.WaitForClose();
                this.DeactivateEndlessChallenge();
                yield break;
            }
            object progressView = PuzzleGameData.DialogViews.ShowProgressView(L.Get("Loading..."));
            EndlessChallengeCloud.EndlessChallengeResponse response = new EndlessChallengeCloud.EndlessChallengeResponse();
            yield return this.endlessChallengeCloud.SubmitScore(this.HighestRow, response);
            if (response.ReturnCode == ReturnCode.NoError)
            {
                yield return this.endlessChallengeCloud.GetStatus(response);
            }
            yield return this.CloseProgressView(progressView);
            if (response.ReturnCode == ReturnCode.NoError)
            {
                this.localPersistedState.UpdateState(response.Entries, response.Users, response.Entry);
                PuzzleGameData.UserSettings.SaveLocal();
            }
            bool showOthersOnLeaderboard = response.ReturnCode == ReturnCode.NoError;
            UIViewManager.UIViewStateGeneric<EndlessChallengeLeaderboardView> lvs = this.uiViewManager.ShowView<EndlessChallengeLeaderboardView>(new object[0]);
            lvs.View.UpdateTable(showOthersOnLeaderboard);
            viewState.value = lvs;
            yield break;
        }

        private IEnumerator CloseProgressView(object progressView)
        {
            PuzzleGameData.DialogViews.CloseView(progressView);
            yield return PuzzleGameData.DialogViews.WaitForClosingView(progressView);
            yield break;
        }

        public void TrySetHighestRow(int row)
        {
            if (row > this.HighestRow)
            {
                this.HighestRow = row;
                UserSettingsManager.Instance.SaveLocalSettings();
            }
            if (row > this.AllTimeHighestRow)
            {
                this.AllTimeHighestRow = row;
                UserSettingsManager.Instance.SaveLocalSettings();
            }
        }

        public void ActivateEndlessChallenge()
        {
            if (this.featureManager.CanActivateFeature(this))
            {
                this.featureManager.ActivateFeature(this);
                FiberCtrl.Pool.Run(this.Join(), false);
            }
        }

        public void DeactivateEndlessChallenge()
        {
            this.featureManager.DeactivateFeature(this);
        }

        public bool ShouldDeactivateFeature()
        {
            return this.featureManager.ShouldDeactivateFeature(this);
        }

        public string GetTimeRemainingForEndlessChallengeAsString()
        {
            return L.FormatSecondsAsColumnSeparated(this.SecondsLeft, L.Get("Ended"), TimeFormatOptions.None);
        }

        public List<EndlessChallengeCheckpointData> GetCheckPointsData()
        {
            ActivatedFeatureInstanceData activatedFeature = this.GetActivatedFeature();
            EndlessChallengeMetaData metaData = activatedFeature.GetMetaData<EndlessChallengeInstanceCustomData, EndlessChallengeMetaData, EndlessChallengeCustomData>(this);
            return metaData.EndlessChallengeCheckpoints;
        }

        public void FeatureRemovedFromDashboard(ActivatedFeatureInstanceData activatedFeatureInstanceData)
        {
            activatedFeatureInstanceData.FeatureInstanceActivationData.ActivatedFeatureData.SetEndUnixTime(1);
            UserSettingsManager.Instance.SaveLocalSettings();
        }

        public void FadeToBlack()
        {
        }

        private IEnumerator Join()
        {
            int farthestLevelNumber = PuzzleGameData.PlayerState.FarthestUnlockedLevelHumanNumber;
            EndlessChallengeCloud.EndlessChallengeResponse response = new EndlessChallengeCloud.EndlessChallengeResponse();
            yield return this.endlessChallengeCloud.Join(farthestLevelNumber, response);
            if (response.ReturnCode == ReturnCode.NoError)
            {
                this.localPersistedState.UpdateState(response.Entries, response.Users, response.Entry);
                PuzzleGameData.UserSettings.SaveLocal();
            }
            yield break;
        }

        public EndlessChallengeBonusInfo GetEndlessChallengeBonusInfo(EndlessChallengeBonusType type)
        {
            foreach (EndlessChallengeBonusInfo endlessChallengeBonusInfo in SingletonAsset<EndlessChallengeSetup>.Instance.endlessChallengeBonusInfos)
            {
                if (endlessChallengeBonusInfo.type == type)
                {
                    return endlessChallengeBonusInfo;
                }
            }
            return null;
        }

        public bool ShouldActivateEndlessChallenge()
        {
            return this.Config.IsActive && PuzzleGame.PlayerState.FarthestUnlockedLevelHumanNumber >= this.Config.LevelRequiredForEndlessChallenge && this.featureManager.CanActivateFeature(this);
        }

        public List<Entry> GetAllParticipants()
        {
            List<Entry> entries = this.localPersistedState.LocalState.Entries;
            entries.Sort(delegate (Entry entry1, Entry entry2)
            {
                int maxRows = entry1.Score.MaxRows;
                int maxRows2 = entry2.Score.MaxRows;
                if (entry1.IsOwnedByDeviceOrUser(this.DeviceId, this.UserId))
                {
                    maxRows = entry1.Score.MaxRows;
                }
                else if (entry2.IsOwnedByDeviceOrUser(this.DeviceId, this.UserId))
                {
                    maxRows2 = entry2.Score.MaxRows;
                }
                return maxRows2 - maxRows;
            });
            return this.localPersistedState.LocalState.Entries;
        }

        public CloudUser GetCloudUser(string userId)
        {
            return this.localPersistedState.LocalState.GetUser(userId);
        }

        public void StartFlow()
        {
            if (this.EndlessLevel.LevelMetaData is EndlessLevelMetaData)
            {
                EndlessChallengePlayFlow c = new EndlessChallengePlayFlow(this, this.EndlessLevel, this.playFlowFactory);
                this.flowStack.Push(c);
            }
        }

        public IEnumerator SubmitAndUpdateAfterPlayedLevel(string levelSessionId, LevelSession levelSession, int rowReached)
        {
            int rank = 0;
            EndlessChallengeCloud.EndlessChallengeResponse response = new EndlessChallengeCloud.EndlessChallengeResponse();
            yield return this.endlessChallengeCloud.SubmitScore(this.HighestRow, response);
            if (response.ReturnCode == ReturnCode.NoError)
            {
                yield return this.endlessChallengeCloud.GetStatus(response);
                if (response.ReturnCode == ReturnCode.NoError)
                {
                    this.localPersistedState.UpdateState(response.Entries, response.Users, response.Entry);
                    List<Entry> allParticipants = this.GetAllParticipants();
                    for (int i = 0; i < allParticipants.Count; i++)
                    {
                        if (allParticipants[i].DeviceId == response.Entry.DeviceId)
                        {
                            rank = i + 1;
                            break;
                        }
                    }
                }
            }
            EndlessChallengeAnalytics.LogEndlessChallengePlayedEvent(rowReached, this.HighestRow, this.AllTimeHighestRow, rank, levelSession.EndlessChallengeStats.CheckpointsReached, levelSession.EndlessChallengeStats.CookieJarSmall, levelSession.EndlessChallengeStats.CookieJarFilUp, levelSession.EndlessChallengeStats.BoosterRainbow, levelSession.EndlessChallengeStats.BoosterFinalPower, levelSession.EndlessChallengeStats.Coins, levelSession.EndlessChallengeStats.ExtraMoves, levelSession.GoodMoves, levelSession.MovesUsed + levelSession.BallQueue.BallsLeft, levelSession.MovesUsed, (int)levelSession.Stats.GetSecondsPlayed(), levelSession.PregameBoosters.Count, levelSession.Stats.GetTimesIngameBoostersWasUsed(), levelSessionId);
            yield break;
        }

        public FeatureNotificationSettings FeatureNotificationSettings
        {
            get
            {
                return this.Config.FeatureNotificationSettings;
            }
        }

        public string GetNotificationText(TimeSpan timeSpan, ActivatedFeatureInstanceData instanceData)
        {
            return string.Format(L.Get("You only have {0} hours left to play the endless challenge!"), timeSpan.TotalHours);
        }

        public Hashtable UpgradeMetaData(Hashtable metaData, int fromVersion, int toVersion)
        {
            throw new NotSupportedException();
        }

        public EndlessChallengeInstanceCustomData NewFeatureInstanceCustomData(FeatureData featureData)
        {
            return new EndlessChallengeInstanceCustomData();
        }

        public EndlessChallengeCustomData NewFeatureTypeCustomData()
        {
            return new EndlessChallengeCustomData();
        }

        public void MergeFeatureInstanceStates<FeatureState>(ref FeatureState toMerge, FeatureState current, FeatureState cloud) where FeatureState : EndlessChallengeInstanceCustomData
        {
            toMerge.HighestRow = Mathf.Max(current.HighestRow, cloud.HighestRow);
        }

        public void MergeFeatureTypeState<HandlerState>(ref HandlerState toMerge, HandlerState current, HandlerState cloud) where HandlerState : EndlessChallengeCustomData
        {
            toMerge.AllTimeHighestRow = Mathf.Max(current.AllTimeHighestRow, cloud.AllTimeHighestRow);
        }

        private readonly TactileModules.FeatureManager.FeatureManager featureManager;

        private readonly LevelDatabaseCollection levelDatabaseCollection;

        private readonly ConfigurationManager configurationManager;

        private readonly UIViewManager uiViewManager;

        private readonly IFlowStack flowStack;

        private readonly IPlayFlowFactory playFlowFactory;

        private readonly EndlessChallengeLocalPersistedState localPersistedState;

        private readonly EndlessChallengeCloud endlessChallengeCloud;

        private readonly Fiber onLevelPlayedFiber = new Fiber();
    }

