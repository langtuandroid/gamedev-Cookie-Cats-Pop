using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Cloud;
using Fibers;
using JetBrains.Annotations;
using TactileModules.FeatureManager;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;
using TactileModules.PuzzleGames.LevelDash.Analytics;
using TactileModules.PuzzleGames.LevelDash.Providers;
using UnityEngine;

namespace TactileModules.PuzzleGames.LevelDash
{
    [MainMapFeature]
    public class LevelDashManager : SingleInstance<LevelDashManager>, IFeatureTypeHandler<LevelDashInstanceCustomData, FeatureMetaData, LevelDashTypeCustomData>, IEndPreviousFeature, IFeatureNotifications, IFeatureTypeHandler
    {
        public LevelDashManager([NotNull] TactileModules.FeatureManager.FeatureManager featureManager, [NotNull] CloudClientBase cloudClient, [NotNull] ILevelDashDataProvider dataProvider)
        {
            if (featureManager == null)
            {
                throw new ArgumentException("[LevelDash] featureManager");
            }
            if (cloudClient == null)
            {
                throw new ArgumentException("[LevelDash] cloudClient");
            }
            if (dataProvider == null)
            {
                throw new ArgumentException("[LevelDash] provider");
            }
            this.featureManager = featureManager;
            this.cloudClient = cloudClient;
            this.dataProvider = dataProvider;
            this.levelDashCloud = new LevelDashCloud(cloudClient, cloudClient.cloudInterface);
            dataProvider.LevelCompleted += this.OnCompleteLevel;
        }

        //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public event Action<LevelDashManager> OnLevelDashStarted;

        //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public event Action<LevelDashManager> OnLevelDashEntriesRefresh;

        //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public event Action<LevelDashManager> OnLevelDashEnded;

        public LevelDashInstanceCustomData CustomInstanceData
        {
            get
            {
                if (!this.HasActiveFeature())
                {
                    return null;
                }
                return this.GetActivatedFeature().GetCustomInstanceData<LevelDashInstanceCustomData, FeatureMetaData, LevelDashTypeCustomData>(this);
            }
        }

        public LevelDashTypeCustomData TypeCustomData
        {
            get
            {
                return this.featureManager.GetFeatureTypeCustomData<LevelDashInstanceCustomData, FeatureMetaData, LevelDashTypeCustomData>(this);
            }
        }

        public bool IsActiveOnClient
        {
            get
            {
                ActivatedFeatureInstanceData activatedFeature = this.GetActivatedFeature();
                return activatedFeature != null;
            }
        }

        public bool AreBasicStartConditionsMet()
        {
            return this.Config.FeatureEnabled && this.dataProvider.FarthestCompletedLevelHumanNumber >= this.Config.LevelRequired && this.dataProvider.MaxAvailableLevelHumanNumber - this.dataProvider.FarthestCompletedLevelHumanNumber >= this.Config.MinimumRequiredLevels && !this.featureManager.ShouldDeactivateFeature(this);
        }

        public bool HasPresentedStartView()
        {
            return this.CustomInstanceData != null && this.CustomInstanceData.StartLevel > 0;
        }

        private bool ShouldStart()
        {
            return this.AreBasicStartConditionsMet() && this.featureManager.CanActivateFeature(this);
        }

        private int SecondsLeft
        {
            get
            {
                return this.featureManager.GetStabilizedTimeLeftToFeatureDurationEnd(this);
            }
        }

        public bool IsActive()
        {
            return this.Config.FeatureEnabled && this.HasActiveFeature() && this.SecondsLeft > 0;
        }

        private bool ShouldEnd()
        {
            return this.featureManager.ShouldDeactivateFeature(this);
        }

        public bool ShouldShowPreviousEndView()
        {
            return this.TypeCustomData.MyRankForPreviousLevelDash > 0;
        }

        public void StartFeature()
        {
            this.CustomInstanceData.StartLevel = this.dataProvider.FarthestCompletedLevelHumanNumber;
            this.Save();
            if (this.OnLevelDashStarted != null)
            {
                this.OnLevelDashStarted(this);
            }
            ActivatedFeatureInstanceData activatedFeature = this.GetActivatedFeature();
            LevelDashAnalytics.LogLevelDashStarted(this.CustomInstanceData.StartLevel, activatedFeature.Id, this.LocalState.Entry.TournamentId);
        }

        public void EndFeature()
        {
            LevelDashInstanceCustomData customInstanceData = this.CustomInstanceData;
            int startLevel = customInstanceData.StartLevel;
            int farthestCompletedLevelHumanNumber = this.dataProvider.FarthestCompletedLevelHumanNumber;
            string id = this.GetActivatedFeature().Id;
            int tournamentId = this.LocalState.Entry.TournamentId;
            this.featureManager.DeactivateFeature(this);
            this.ResetLocalState();
            this.Save();
            LevelDashAnalytics.LogLevelDashEnded(startLevel, farthestCompletedLevelHumanNumber, id, tournamentId);
            if (this.OnLevelDashEnded != null)
            {
                this.OnLevelDashEnded(this);
            }
        }

        private void UpdateFeatureState()
        {
            if (this.ShouldStart())
            {
                FiberCtrl.Pool.Run(this.SendJointRequest(), false);
            }
            else if (this.ShouldEnd() && !this.CustomInstanceData.HasReceivedFinalStatus)
            {
                FiberCtrl.Pool.Run(this.SendGetStatusRequest(null), false);
            }
            else if (this.IsActive())
            {
                FiberCtrl.Pool.Run(this.SendGetStatusRequest(new Action<object>(this.OnFinalStatusReceived)), false);
            }
        }

        private void OnFinalStatusReceived(object error)
        {
            this.CustomInstanceData.HasReceivedFinalStatus = (error == null);
        }

        private void TryToSendGetRewardStatus()
        {
            ActivatedFeatureInstanceData activatedFeature = this.GetActivatedFeature();
            if (activatedFeature != null)
            {
                FiberCtrl.Pool.Run(this.SendGetRewardStatusRequest(activatedFeature.Id, new Action<object, List<Entry>, List<CloudUser>, RewardStatus>(this.HanldeReceivedRewardStatus)), false);
            }
        }

        public string GetTimeRemainingAsString()
        {
            return L.FormatSecondsAsColumnSeparated(this.SecondsLeft, L.Get("Ended"), TimeFormatOptions.None);
        }

        public List<Entry> GetAllEntries()
        {
            List<Entry> entries = this.LocalState.Entries;
            this.SortEntries(entries);
            return this.LocalState.Entries;
        }

        private void SortEntries(List<Entry> entries)
        {
            entries.Sort(delegate (Entry entry1, Entry entry2)
            {
                int maxLevel = entry1.MaxLevel;
                int maxLevel2 = entry2.MaxLevel;
                return maxLevel2 - maxLevel;
            });
        }

        public List<Entry> GetAllOtherEntries()
        {
            List<Entry> list = new List<Entry>(this.GetAllEntries());
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].IsOwnedByDeviceOrUser(this.GetDeviceId(), this.GetUserId()))
                {
                    list.RemoveAt(i);
                    break;
                }
            }
            return list;
        }

        public int GetRank(string deviceId, string userId)
        {
            return this.GetRank(this.GetAllEntries(), deviceId, userId);
        }

        private int GetMyRank()
        {
            return this.GetRank(this.GetAllEntries(), this.GetDeviceId(), this.GetUserId());
        }

        private int GetRank(List<Entry> entries, string deviceId, string userId)
        {
            int num = 1;
            for (int i = 0; i < entries.Count; i++)
            {
                if (i > 0 && entries[i - 1].MaxLevel != entries[i].MaxLevel)
                {
                    num++;
                }
                if (entries[i].IsOwnedByDeviceOrUser(deviceId, userId))
                {
                    return num;
                }
            }
            return -1;
        }

        public int GetMyIndexInList(List<Entry> entries)
        {
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].IsOwnedByDeviceOrUser(this.GetDeviceId(), this.GetUserId()))
                {
                    return i;
                }
            }
            return -1;
        }

        public CloudUser GetCloudUser(string userId)
        {
            return this.LocalState.GetUser(userId);
        }

        public int GetRewardsCount()
        {
            return this.Config.Rewards.Count;
        }

        public LevelDashConfig.Reward GetMyReward()
        {
            return this.GetReward(this.GetMyRank());
        }

        public LevelDashConfig.Reward GetReward(int rank)
        {
            for (int i = 0; i < this.Config.Rewards.Count; i++)
            {
                LevelDashConfig.Reward reward = this.Config.Rewards[i];
                if (reward.Rank == rank)
                {
                    return reward;
                }
            }
            return null;
        }

        private void OnCompleteLevel()
        {
            if (this.IsActive())
            {
                FiberCtrl.Pool.Run(this.SendSubmitScoreRequest(null), false);
            }
        }

        public void SendGetRewardStatusForPreviousLevelDash(string featureId)
        {
            FiberCtrl.Pool.Run(this.SendGetRewardStatusRequest(featureId, new Action<object, List<Entry>, List<CloudUser>, RewardStatus>(this.OnRewardStatusReponseForPreviousLevelDash)), false);
        }

        private void OnRewardStatusReponseForPreviousLevelDash(object error, List<Entry> entries, List<CloudUser> users, RewardStatus status)
        {
            if (error != null || status != RewardStatus.Presented)
            {
                return;
            }
            this.SortEntries(entries);
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].IsOwnedByDeviceOrUser(this.GetDeviceId(), this.GetUserId()))
                {
                    int num = i + 1;
                    if (num < this.GetRewardsCount())
                    {
                        this.TypeCustomData.MyRankForPreviousLevelDash = num;
                    }
                    break;
                }
            }
        }

        private IEnumerator SendJointRequest()
        {
            FeatureData featureToStart = this.featureManager.GetFeature(this);
            yield return this.levelDashCloud.SendJoinRequest(featureToStart.Id, this.dataProvider.FarthestCompletedLevelHumanNumber, delegate (object err, string featureId, Entry entry, List<Entry> entries, List<CloudUser> users)
            {
                if (err == null)
                {
                    ActivatedFeatureInstanceData activatedFeatureInstanceData = this.featureManager.ActivateFeature(this, featureToStart);
                    if (activatedFeatureInstanceData != null)
                    {
                        this.UpdateLocalPersistableState(entries, users, entry);
                        this.CustomInstanceData.JoinedInstanceId = featureId;
                        this.Save();
                    }
                }
            });
            yield break;
        }

        private IEnumerator SendGetStatusRequest(Action<object> callback = null)
        {
            ActivatedFeatureInstanceData activatedFeatureData = this.GetActivatedFeature();
            if (activatedFeatureData == null)
            {
                yield break;
            }
            yield return this.levelDashCloud.SendGetStatusRequest(activatedFeatureData.Id, delegate (object err, string id, Entry entry, List<Entry> entries, List<CloudUser> users)
            {
                if (err == null)
                {
                    this.UpdateLocalPersistableState(entries, users, entry);
                }
                if (callback != null)
                {
                    callback(err);
                }
            });
            yield break;
        }

        private IEnumerator SendSubmitScoreRequest(Action<object> callback = null)
        {
            ActivatedFeatureInstanceData activatedFeatureData = this.GetActivatedFeature();
            if (activatedFeatureData == null)
            {
                yield break;
            }
            yield return this.levelDashCloud.SendSubmitScore(activatedFeatureData.Id, this.dataProvider.FarthestCompletedLevelHumanNumber, delegate (object err, string id)
            {
                if (err != null)
                {
                }
                if (callback != null)
                {
                    callback(err);
                }
            });
            yield break;
        }

        private IEnumerator SendGetRewardStatusRequest(string featureId, Action<object, List<Entry>, List<CloudUser>, RewardStatus> callback)
        {
            yield return this.levelDashCloud.SendGetRewardStatusRequest(featureId, delegate (object err, RewardStatus status, List<Entry> entries, List<CloudUser> users)
            {
                if (callback != null)
                {
                    callback(err, entries, users, status);
                }
            });
            yield break;
        }

        public string GetDeviceId()
        {
            return (!this.cloudClient.HasValidDevice) ? string.Empty : this.cloudClient.CachedDevice.CloudId;
        }

        public string GetUserId()
        {
            return (!this.cloudClient.HasValidUser) ? string.Empty : this.cloudClient.CachedMe.CloudId;
        }

        public bool ShouldShowMapButton()
        {
            return this.Config.FeatureEnabled && this.IsActiveOnClient;
        }

        public LevelDashConfig Config
        {
            get
            {
                return this.dataProvider.Config();
            }
        }

        private void Save()
        {
            PuzzleGameData.UserSettings.SaveLocal();

        }

        private LocalPersistableState LocalState
        {
            get
            {
                if (this.cachedLocalState != null)
                {
                    return this.cachedLocalState;
                }
                string securedString = TactilePlayerPrefs.GetSecuredString("LevelDashManager:LocalPersistableState", string.Empty);
                this.cachedLocalState = new LocalPersistableState();
                if (securedString.Length > 0)
                {
                    this.cachedLocalState = JsonSerializer.HashtableToObject<LocalPersistableState>(securedString.hashtableFromJson());
                }
                return this.cachedLocalState;
            }
            set
            {
                if (value != null)
                {
                    this.cachedLocalState = value;
                    string value2 = JsonSerializer.ObjectToHashtable(this.cachedLocalState).toJson();
                    TactilePlayerPrefs.SetSecuredString("LevelDashManager:LocalPersistableState", value2);
                }
                else
                {
                    TactilePlayerPrefs.SetSecuredString("LevelDashManager:LocalPersistableState", string.Empty);
                }
            }
        }

        private void UpdateLocalPersistableState(List<Entry> entries, List<CloudUser> users, Entry entry)
        {
            if (entries != null)
            {
                this.LocalState.Entries = entries;
                if (this.OnLevelDashEntriesRefresh != null)
                {
                    this.OnLevelDashEntriesRefresh(this);
                }
                if (entry == null)
                {
                    for (int i = 0; i < entries.Count; i++)
                    {
                        if (entries[i].IsOwnedByDeviceOrUser(this.GetDeviceId(), this.GetUserId()))
                        {
                            this.LocalState.Entry = entries[i];
                        }
                    }
                }
            }
            if (users != null)
            {
                this.LocalState.Users = users;
            }
            if (entry != null)
            {
                this.LocalState.Entry = entry;
            }
            this.SaveLocalState();
        }

        private void SaveLocalState()
        {
            this.LocalState = this.LocalState;
        }

        private void ResetLocalState()
        {
            this.LocalState = new LocalPersistableState();
        }

        public void SyncStatus(Func<LevelDashViewController.SyncResultView, ReturnCode, IEnumerator> syncCallback)
        {
            this.syncFiber.Start(this.SyncStatusCr(syncCallback));
        }

        public IEnumerator WaitForSyncingEnd()
        {
            while (!this.syncFiber.IsTerminated)
            {
                yield return null;
            }
            yield break;
        }

        private IEnumerator SyncStatusCr(Func<LevelDashViewController.SyncResultView, ReturnCode, IEnumerator> syncCallback)
        {
            ReturnCode error = ReturnCode.NoError;
            if (this.IsActive())
            {
                yield return this.SendSubmitScoreRequest(delegate (object e)
                {
                    error = ((e != null) ? ((ReturnCode)e) : error);
                });
                yield return this.HandleError(error, syncCallback);
                yield return this.HandleActive(syncCallback);
            }
            else if (this.ShouldEnd())
            {
                yield return this.HandleEnded(syncCallback);
            }
            yield break;
        }

        private IEnumerator HandleActive(Func<LevelDashViewController.SyncResultView, ReturnCode, IEnumerator> syncCallback)
        {
            ReturnCode error = ReturnCode.NoError;
            yield return this.SendGetStatusRequest(delegate (object e)
            {
                error = ((e != null) ? ReturnCode.ClientConnectionError : error);
            });
            yield return this.HandleError(error, syncCallback);
            if (syncCallback != null)
            {
                yield return syncCallback(LevelDashViewController.SyncResultView.Leaderboard, ReturnCode.NoError);
            }
            yield break;
        }

        private IEnumerator HandleEnded(Func<LevelDashViewController.SyncResultView, ReturnCode, IEnumerator> syncCallback)
        {
            ActivatedFeatureInstanceData activatedFeatureData = this.GetActivatedFeature();
            if (activatedFeatureData != null)
            {
                if (this.CustomInstanceData.IsReceivedValidResultResponse())
                {
                    yield return this.PresetResults(syncCallback);
                }
                else
                {
                    ReturnCode error = ReturnCode.NoError;
                    yield return this.SendGetRewardStatusRequest(activatedFeatureData.Id, delegate (object err, List<Entry> entries, List<CloudUser> users, RewardStatus status)
                    {
                        this.HanldeReceivedRewardStatus(err, entries, users, status);
                        error = ((err != null) ? ((ReturnCode)err) : ReturnCode.NoError);
                    });
                    yield return this.HandleError(error, syncCallback);
                    yield return this.PresetResults(syncCallback);
                }
            }
            yield break;
        }

        private void HanldeReceivedRewardStatus(object err, List<Entry> entries, List<CloudUser> users, RewardStatus status)
        {
            if (err == null)
            {
                this.UpdateLocalPersistableState(entries, users, null);
                this.CustomInstanceData.ReceivedRewardStatus = status;
            }
        }

        private IEnumerator PresetResults(Func<LevelDashViewController.SyncResultView, ReturnCode, IEnumerator> syncCallback)
        {
            if (syncCallback != null)
            {
                yield return syncCallback(LevelDashViewController.SyncResultView.Results, ReturnCode.NoError);
            }
            yield break;
        }

        private IEnumerator HandleError(ReturnCode error, Func<LevelDashViewController.SyncResultView, ReturnCode, IEnumerator> syncCallback)
        {
            if (error == ReturnCode.NotLatestVersion)
            {
                this.featureManager.UpdateFeatureLists(delegate (object e)
                {
                    error = ((e != null) ? ReturnCode.ClientConnectionError : ReturnCode.NoError);
                });
                yield return this.featureManager.WaitForFeatureListUpdate();
            }
            if (error != ReturnCode.NoError)
            {
                if (syncCallback != null)
                {
                    yield return syncCallback(LevelDashViewController.SyncResultView.Error, error);
                }
                this.syncFiber.Terminate();
            }
            yield break;
        }

        public void FeatureRemovedFromDashboard(ActivatedFeatureInstanceData activatedFeatureInstanceData)
        {
            activatedFeatureInstanceData.FeatureInstanceActivationData.ActivatedFeatureData.SetEndUnixTime(1);
        }

        public void FadeToBlack()
        {
            this.UpdateFeatureState();
        }

        public string FeatureType
        {
            get
            {
                return "level-dash";
            }
        }

        public bool AllowMultipleFeatureInstances
        {
            get
            {
                return false;
            }
        }

        public bool DurationClampedByEndDate
        {
            get
            {
                return true;
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
            if (fromVersion == 0 && toVersion == 1)
            {
                return metaData;
            }
            throw new NotImplementedException();
        }

        public LevelDashInstanceCustomData NewFeatureInstanceCustomData(FeatureData featureData)
        {
            return new LevelDashInstanceCustomData();
        }

        public LevelDashTypeCustomData NewFeatureTypeCustomData()
        {
            return new LevelDashTypeCustomData();
        }

        public void MergeFeatureInstanceStates<FeatureState>(ref FeatureState toMerge, FeatureState current, FeatureState cloud) where FeatureState : LevelDashInstanceCustomData
        {
            if (current.StartLevel < 0 && cloud.StartLevel > 0)
            {
                this.TakeState<FeatureState>(ref toMerge, cloud);
            }
            else if (current.StartLevel > 0 && cloud.StartLevel < 0)
            {
                this.TakeState<FeatureState>(ref toMerge, current);
            }
            else if (current.StartLevel > 0 && cloud.StartLevel > 0)
            {
                if (current.StartLevel == cloud.StartLevel)
                {
                    toMerge.StartLevel = current.StartLevel;
                    toMerge.ReceivedRewardStatus = (RewardStatus)Mathf.Max((int)current.ReceivedRewardStatus, (int)cloud.ReceivedRewardStatus);
                }
                else
                {
                    this.TakeState<FeatureState>(ref toMerge, cloud);
                }
            }
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
        }

        private void TakeState<FeatureState>(ref FeatureState toMerge, FeatureState state) where FeatureState : LevelDashInstanceCustomData
        {
            toMerge.JoinedInstanceId = state.JoinedInstanceId;
            toMerge.ReceivedRewardStatus = state.ReceivedRewardStatus;
            toMerge.StartLevel = state.StartLevel;
        }

        public void MergeFeatureTypeState<HandlerState>(ref HandlerState toMerge, HandlerState current, HandlerState cloud) where HandlerState : LevelDashTypeCustomData
        {
            toMerge.MyRankForPreviousLevelDash = Mathf.Max(current.MyRankForPreviousLevelDash, cloud.MyRankForPreviousLevelDash);
        }

        public void EndPreviousInstanceOfFeature(ActivatedFeatureInstanceData activatedFeatureInstanceData)
        {
            this.SendGetRewardStatusForPreviousLevelDash(activatedFeatureInstanceData.Id);
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
            return this.dataProvider.GetTextForNotification(timeSpan, instanceData);
        }

        private ILevelDashDataProvider dataProvider;

        private LevelDashCloud levelDashCloud;

        private CloudClientBase cloudClient;

        private TactileModules.FeatureManager.FeatureManager featureManager;

        private Fiber syncFiber = new Fiber();

        private const string LevelDashLocalStatePersistableKey = "LevelDashManager:LocalPersistableState";

        private LocalPersistableState cachedLocalState;
    }
}
