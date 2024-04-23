using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Tactile;
using TactileModules.FeatureManager;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;
using TactileModules.Portraits;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGames.Configuration;
using TactileModules.PuzzleGames.StarTournament.Views;

namespace TactileModules.PuzzleGames.StarTournament
{
    [MainMapFeature]
    public sealed class StarTournamentManager : IFeatureTypeHandler<StarTournamentInstanceCustomData, FeatureMetaData, StarTournamentTypeCustomData>, IEndPreviousFeature, IFeatureNotifications, IFeatureTypeHandler
    {
        public StarTournamentManager([NotNull] TactileModules.FeatureManager.FeatureManager featureManager, [NotNull] IStarTournamentProvider provider, [NotNull] CloudClientBase cloudClient, FacebookClient facebookClient, IPlayFlowEvents playFlowEvents, IMainProgression mainProgression, IConfigGetter<StarTournamentConfig> configGetter, InventoryManager inventoryManager, IRandomPortraitsAndNames randomPortraitsAndNames)
        {
            if (featureManager == null)
            {
                throw new ArgumentNullException("featureManager");
            }
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
            if (cloudClient == null)
            {
                throw new ArgumentNullException("cloudClient");
            }
            this.RandomPortraitsAndNames = randomPortraitsAndNames;
            this.featureManager = featureManager;
            this.facebookClient = facebookClient;
            this.mainProgression = mainProgression;
            this.configGetter = configGetter;
            this.inventoryManager = inventoryManager;
            this.Provider = provider;
            this.CloudClient = cloudClient;
            this.ActiveLevel = new CurrentActiveLevel();
            this.StarTournamentCloud = new StarTournamentCloud(cloudClient, cloudClient.cloudInterface);
            this.LocalPersistedState = new StarTournamentLocalPersistedState();
            playFlowEvents.PlayFlowCreated += this.HandlePlayFlowCreated;
        }

        public IStarTournamentProvider Provider { get; private set; }

        public CloudClientBase CloudClient { get; private set; }

        public StarTournamentCloud StarTournamentCloud { get; private set; }

        public StarTournamentLocalPersistedState LocalPersistedState { get; private set; }

        public IRandomPortraitsAndNames RandomPortraitsAndNames { get; private set; }

        private void HandlePlayFlowCreated(ICorePlayFlow playFlow)
        {
            if (playFlow.LevelProxy.RootDatabase is MainLevelDatabase)
            {
                playFlow.LevelStartedHook.Register(new Func<ILevelAttempt, IEnumerator>(this.LevelStartedHook));
                playFlow.LevelEndedHook.Register(new Func<ILevelAttempt, IEnumerator>(this.LevelEndedHook));
            }
        }

        private IEnumerator LevelEndedHook(ILevelAttempt levelAttempt)
        {
            int numStarsInResult = 0;
            if (levelAttempt.FinalEndState == LevelEndState.Completed)
            {
                numStarsInResult = levelAttempt.Stats.Stars;
            }
            this.OnLevelEnd(numStarsInResult);
            yield break;
        }

        private IEnumerator LevelStartedHook(ILevelAttempt playLevel)
        {
            this.OnLevelStart(playLevel.LevelProxy);
            yield break;
        }

        public IEnumerator ShowStarTournamentStartedView(bool isReminder)
        {
            UIViewManager.UIViewStateGeneric<StarTournamentStartView> viewState = UIViewManager.Instance.ShowView<StarTournamentStartView>(new object[0]);
            viewState.View.Initialize(this, this.mainProgression, this.CloudClient, this.facebookClient, isReminder);
            yield return viewState.WaitForClose();
            if (viewState.ClosingResult is UIViewManager.UIViewState)
            {
                yield return ((UIViewManager.UIViewState)viewState.ClosingResult).WaitForClose();
            }
            yield break;
        }

        public CurrentActiveLevel ActiveLevel { get; private set; }

        public int FarthestUnlockedLevelNumber
        {
            get
            {
                return this.mainProgression.GetFarthestUnlockedLevelHumanNumber();
            }
        }

        public StarTournamentConfig Config
        {
            get
            {
                return this.configGetter.Get();
            }
        }

        public bool HasSentPresentRequest
        {
            get
            {
                return this.StarTournamentCloud.hasSentPresentRequest;
            }
        }

        public string GetTimeRemainingForStarTournamentAsString()
        {
            return (!this.IsStarTournamentActive) ? L.Get("Ended") : L.FormatSecondsAsColumnSeparated(this.SecondsLeft, L.Get("Ended"), TimeFormatOptions.None);
        }

        public bool IsStarTournamentActive
        {
            get
            {
                return this.Config.FeatureEnabled && this.HasActiveFeature() && this.SecondsLeft > 0;
            }
        }

        public IEnumerator ShowLeaderboard()
        {
            UIViewManager.UIViewStateGeneric<StarTournamentLeaderboardView> vs = UIViewManager.Instance.ShowView<StarTournamentLeaderboardView>(new object[0]);
            vs.View.Initialize(this, this.mainProgression, this.CloudClient, this.facebookClient);
            yield return vs.WaitForClose();
            yield break;
        }

        public IEnumerator ShowStarTournamentEndedView(StarTournamentConfig.Reward reward, FeatureData featureData)
        {
            if (reward == null)
            {
                UIViewManager.UIViewStateGeneric<StarTournamentLeaderboardView> view = UIViewManager.Instance.ShowView<StarTournamentLeaderboardView>(new object[0]);
                view.View.Initialize(this, this.mainProgression, this.CloudClient, this.facebookClient);
                yield return view.WaitForClose();
            }
            else
            {
                UIViewManager.UIViewStateGeneric<StarTournamentRewardView> view2 = UIViewManager.Instance.ShowView<StarTournamentRewardView>(new object[0]);
                view2.View.Initialize(reward);
                yield return view2.WaitForClose();
            }
            yield break;
        }

        public IEnumerator ShowStarTournamentEndedViewForOld(OldStartTournamentInfo info)
        {
            int coins = info.RewardForOldTournament.Items[0].Amount;
            this.inventoryManager.AddCoins(coins, "OldInstanceOfStarTournament");
            UIViewManager.UIViewStateGeneric<MessageBoxView> view = UIViewManager.Instance.ShowView<MessageBoxView>(new object[]
            {
                L.Get("Congratulations!"),
                string.Format(L.Get("You took a {0} place at previous Star Tournament and won {1} coins"), info.RewardForOldTournament.Rank, coins),
                L.Get("Ok")
            });
            yield return view.WaitForClose();
            yield break;
        }

        public bool ShouldStartTournament()
        {
            return this.FarthestUnlockedLevelNumber >= this.Config.LevelRequired && this.featureManager.CanActivateFeature(this);
        }

        public bool ShouldEndTournament()
        {
            return this.featureManager.ShouldDeactivateFeature(this);
        }

        public void EndStarTournament()
        {
            this.featureManager.DeactivateFeature(this);
            this.StarTournamentCloud.ReceivedResult = -100;
            this.LocalPersistedState.ResetLocalState();
            this.LocalPersistedState.PersistedUnsubmittedStars = 0;
        }

        public List<Entry> GetAllParticipants()
        {
            List<Entry> entries = this.LocalPersistedState.LocalState.Entries;
            string deviceId = (!this.CloudClient.HasValidDevice) ? string.Empty : this.CloudClient.CachedDevice.CloudId;
            string userId = (!this.CloudClient.HasValidUser) ? string.Empty : this.CloudClient.CachedMe.CloudId;
            entries.Sort(delegate (Entry entry1, Entry entry2)
            {
                int stars = entry1.Stars;
                int stars2 = entry2.Stars;
                if (entry1.IsOwnedByDeviceOrUser(deviceId, userId))
                {
                    stars = entry1.Stars;
                }
                else if (entry2.IsOwnedByDeviceOrUser(deviceId, userId))
                {
                    stars2 = entry2.Stars;
                }
                return stars2 - stars;
            });
            return this.LocalPersistedState.LocalState.Entries;
        }

        public CloudUser GetCloudUser(string userId)
        {
            return this.LocalPersistedState.LocalState.GetUser(userId);
        }

        public int GetRewardsAmount()
        {
            return this.Config.Rewards.Count;
        }

        public IEnumerator PerformEndFlow()
        {
            if (this.GetReward(this.GetRank(this.LocalPersistedState.LocalState.Entry)) == null)
            {
                yield return this.ShowStarTournamentEndedView(null, null);
                this.EndStarTournament();
            }
            else
            {
                while (this.HasActiveFeature())
                {
                    UIViewManager.UIViewStateGeneric<StarTournamentEndedView> viewState = UIViewManager.Instance.ShowView<StarTournamentEndedView>(new object[0]);
                    yield return viewState.WaitForClose();
                    if ((int)viewState.ClosingResult != 1)
                    {
                        yield break;
                    }
                    this.StarTournamentCloud.SyncStatus();
                    yield return this.StarTournamentCloud.WaitForSyncingEnd();
                }
            }
            yield break;
        }

        public StarTournamentConfig.Reward GetReward(int rank)
        {
            for (int i = 0; i < this.Config.Rewards.Count; i++)
            {
                StarTournamentConfig.Reward reward = this.Config.Rewards[i];
                if (reward.Rank == rank)
                {
                    return reward;
                }
            }
            return null;
        }

        public int GetRank(Entry entry)
        {
            List<Entry> allParticipants = this.GetAllParticipants();
            for (int i = 0; i < allParticipants.Count; i++)
            {
                Entry entry2 = allParticipants[i];
                if (entry.DeviceId == entry2.DeviceId && entry.UserId == entry2.UserId)
                {
                    return i + 1;
                }
            }
            return -1;
        }

        private int SecondsLeft
        {
            get
            {
                return this.featureManager.GetStabilizedTimeLeftToFeatureDurationEnd(this);
            }
        }

        public StarTournamentInstanceCustomData CustomInstanceData
        {
            get
            {
                if (!this.HasActiveFeature())
                {
                    return null;
                }
                return this.GetActivatedFeature().GetCustomInstanceData<StarTournamentInstanceCustomData, FeatureMetaData, StarTournamentTypeCustomData>(this);
            }
        }

        private void OnLevelStart(LevelProxy levelProxy)
        {
            if (!this.IsStarTournamentActive)
            {
                return;
            }
            this.ActiveLevel.Start(levelProxy.Index, levelProxy.Stars);
        }

        private void OnLevelEnd(int numStarsInResult)
        {
            if (!this.IsStarTournamentActive)
            {
                return;
            }
            int num = numStarsInResult - this.ActiveLevel.StarsLevelStart;
            this.StarTournamentCloud.Submit(num);
            this.ActiveLevel.End(num);
        }

        public void FeatureRemovedFromDashboard(ActivatedFeatureInstanceData activatedFeatureInstanceData)
        {
            activatedFeatureInstanceData.FeatureInstanceActivationData.ActivatedFeatureData.SetEndUnixTime(1);
        }

        public void FadeToBlack()
        {
            this.StarTournamentCloud.UpdateFeatureState();
        }

        public string FeatureType
        {
            get
            {
                return "star-tournament";
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
                return 0;
            }
        }

        public Hashtable UpgradeMetaData(Hashtable metaData, int fromVersion, int toVersion)
        {
            throw new NotSupportedException();
        }

        public StarTournamentInstanceCustomData NewFeatureInstanceCustomData(FeatureData featureData)
        {
            return new StarTournamentInstanceCustomData();
        }

        public StarTournamentTypeCustomData NewFeatureTypeCustomData()
        {
            return new StarTournamentTypeCustomData();
        }

        public void MergeFeatureInstanceStates<FeatureState>(ref FeatureState toMerge, FeatureState current, FeatureState cloud) where FeatureState : StarTournamentInstanceCustomData
        {
            if (string.IsNullOrEmpty(current.JoinedInstanceId) && !string.IsNullOrEmpty(cloud.JoinedInstanceId))
            {
                toMerge.JoinedInstanceId = cloud.JoinedInstanceId;
            }
            else if (!string.IsNullOrEmpty(current.JoinedInstanceId) && string.IsNullOrEmpty(cloud.JoinedInstanceId))
            {
                toMerge.JoinedInstanceId = current.JoinedInstanceId;
            }
            else if (!string.IsNullOrEmpty(current.JoinedInstanceId) && !string.IsNullOrEmpty(cloud.JoinedInstanceId) && current.JoinedInstanceId == cloud.JoinedInstanceId)
            {
                toMerge.JoinedInstanceId = current.JoinedInstanceId;
            }
            else
            {
                toMerge.JoinedInstanceId = string.Empty;
            }
            toMerge.StartedViewPresented = (current.StartedViewPresented || cloud.StartedViewPresented);
        }

        public void MergeFeatureTypeState<HandlerState>(ref HandlerState toMerge, HandlerState current, HandlerState cloud) where HandlerState : StarTournamentTypeCustomData
        {
            if (current.OldTournamentInfo != null && cloud.OldTournamentInfo == null)
            {
                toMerge.OldTournamentInfo = current.OldTournamentInfo;
            }
            else if (current.OldTournamentInfo == null && cloud.OldTournamentInfo != null)
            {
                toMerge.OldTournamentInfo = cloud.OldTournamentInfo;
            }
            else if (current.OldTournamentInfo != null && cloud.OldTournamentInfo != null)
            {
                if (cloud.OldTournamentInfo.FeatureId != current.OldTournamentInfo.FeatureId)
                {
                    toMerge.OldTournamentInfo = current.OldTournamentInfo;
                }
                else
                {
                    toMerge.OldTournamentInfo = current.OldTournamentInfo;
                    toMerge.OldTournamentInfo.NeedToShowEndedViewOldTournament = current.OldTournamentInfo.NeedToShowEndedViewOldTournament + (cloud.OldTournamentInfo.NeedToShowEndedViewOldTournament - current.OldTournamentInfo.NeedToShowEndedViewOldTournament);
                }
            }
        }

        public void EndPreviousInstanceOfFeature(ActivatedFeatureInstanceData activatedFeatureInstanceData)
        {
            this.StarTournamentCloud.RequestPresentForOldTournament(activatedFeatureInstanceData.Id);
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
            return this.Provider.GetTextForNotification(timeSpan, instanceData);
        }

        private readonly TactileModules.FeatureManager.FeatureManager featureManager;

        private readonly FacebookClient facebookClient;

        private readonly IMainProgression mainProgression;

        private readonly IConfigGetter<StarTournamentConfig> configGetter;

        private readonly InventoryManager inventoryManager;
    }
}
