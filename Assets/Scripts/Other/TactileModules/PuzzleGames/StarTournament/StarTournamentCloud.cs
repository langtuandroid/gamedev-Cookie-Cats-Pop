using System;
using System.Collections;
using System.Collections.Generic;
using Cloud;
using Fibers;
using JetBrains.Annotations;
using TactileModules.FeatureManager;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.TactileCloud;

namespace TactileModules.PuzzleGames.StarTournament
{
    public sealed class StarTournamentCloud
    {
        public StarTournamentCloud([NotNull] CloudClientBase cloudClientBase, [NotNull] ICloudInterfaceBase cloudInterfaceBase)
        {
            if (cloudClientBase == null)
            {
                throw new ArgumentNullException("cloudClientBase");
            }
            if (cloudInterfaceBase == null)
            {
                throw new ArgumentNullException("cloudInterfaceBase");
            }
            this.cloudClientBase = cloudClientBase;
            this.cloudInterfaceBase = cloudInterfaceBase;
        }

        public bool hasSentPresentRequest { get; private set; }

        private bool HasValidDevice
        {
            get
            {
                return this.cloudClientBase.HasValidDevice;
            }
        }

        private bool HasValidUser
        {
            get
            {
                return this.cloudClientBase.HasValidUser;
            }
        }

        private CloudUser CachedMe
        {
            get
            {
                return this.cloudClientBase.CachedMe;
            }
        }

        private CloudDevice CachedDevice
        {
            get
            {
                return this.cloudClientBase.CachedDevice;
            }
        }

        private string FeatureId
        {
            get
            {
                string result = string.Empty;
                ActivatedFeatureInstanceData activatedFeature = this.Manager.GetActivatedFeature();
                if (activatedFeature != null)
                {
                    result = activatedFeature.Id;
                }
                return result;
            }
        }

        public int ReceivedResult
        {
            get
            {
                return this.Manager.LocalPersistedState.ReceivedResult;
            }
            set
            {
                this.Manager.LocalPersistedState.ReceivedResult = value;
            }
        }

        public bool IsResultReceived
        {
            get
            {
                return this.ReceivedResult != -100;
            }
        }

        private StarTournamentManager Manager
        {
            get
            {
                return TactileModules.FeatureManager.FeatureManager.GetFeatureHandler<StarTournamentManager>();
            }
        }

        public IEnumerator ShowResults(object error)
        {
            yield return this.ShowResults(error, (StarTournamentCloud.PresentResult)this.ReceivedResult);
            yield break;
        }

        public void UpdateFeatureState()
        {
            if (!this.Manager.Config.FeatureEnabled)
            {
                return;
            }
            if (this.Manager.ShouldStartTournament())
            {
                FiberCtrl.Pool.Run(this.Join(), true);
            }
            else if (this.Manager.ShouldEndTournament() && !this.Manager.LocalPersistedState.HasReceivedFinalStatus)
            {
                FiberCtrl.Pool.Run(this.Status(new Action<object>(this.OnFinalStatusReceived)), false);
            }
            else
            {
                this.EnsureJoinIdIsSet();
            }
        }

        private void EnsureJoinIdIsSet()
        {
            if (this.Manager.HasActiveFeature() && this.Manager.CustomInstanceData != null && string.IsNullOrEmpty(this.Manager.CustomInstanceData.JoinedInstanceId))
            {
                ActivatedFeatureInstanceData activatedFeature = TactileModules.FeatureManager.FeatureManager.Instance.GetActivatedFeature(this.Manager);
                this.Manager.CustomInstanceData.JoinedInstanceId = activatedFeature.Id;
                PuzzleGameData.UserSettings.SaveLocal();
            }
        }

        private void OnFinalStatusReceived(object error)
        {
            this.Manager.LocalPersistedState.HasReceivedFinalStatus = (error == null);
        }

        private IEnumerator Join()
        {
            if (this.hasSentJoinRequest)
            {
                yield break;
            }
            yield return new Fiber.OnExit(delegate ()
            {
                this.hasSentJoinRequest = false;
            });
            this.hasSentJoinRequest = true;
            FeatureData featureToStart = TactileModules.FeatureManager.FeatureManager.Instance.GetFeature(this.Manager);
            yield return this.StarTournamentJoin(featureToStart.Id, this.Manager.FarthestUnlockedLevelNumber, delegate (object err, string featureId, Entry entry, List<Entry> entries, List<CloudUser> users)
            {
                if (err == null)
                {
                    ActivatedFeatureInstanceData activatedFeatureInstanceData = TactileModules.FeatureManager.FeatureManager.Instance.ActivateFeature(this.Manager, featureToStart);
                    if (activatedFeatureInstanceData != null)
                    {
                        this.Manager.LocalPersistedState.UpdateState(entries, users, entry);
                        this.Manager.CustomInstanceData.JoinedInstanceId = featureId;
                        this.Manager.CustomInstanceData.StartedViewPresented = false;
                        PuzzleGameData.UserSettings.SaveLocal();
                    }
                }
            });
            yield break;
        }

        private IEnumerator ShowResults(object error, StarTournamentCloud.PresentResult result)
        {
            object view = null;
            if (error == null)
            {
                switch (result + 1)
                {
                    case StarTournamentCloud.PresentResult.TournamentActive:
                        view = PuzzleGameData.DialogViews.ShowMessageBox(L.Get("Star Tournament"), L.Get("Star Tournament which you were participating is outdated."), L.Get("OK"), null);
                        this.Manager.EndStarTournament();
                        break;
                    case StarTournamentCloud.PresentResult.Presented:
                        view = PuzzleGameData.DialogViews.ShowMessageBox(L.Get("Star Tournament is active"), L.Get("Star Tournament is still active, please try again later."), L.Get("OK"), null);
                        break;
                    case StarTournamentCloud.PresentResult.PreviouslyPresented:
                        {
                            FeatureData featureData = this.Manager.GetActivatedFeature().FeatureData;
                            StarTournamentConfig.Reward reward = this.Manager.GetReward(this.Manager.GetRank(this.Manager.LocalPersistedState.LocalState.Entry));
                            yield return this.Manager.ShowStarTournamentEndedView(reward, featureData);
                            this.Manager.EndStarTournament();
                            break;
                        }
                    case (StarTournamentCloud.PresentResult)3:
                        view = PuzzleGameData.DialogViews.ShowMessageBox(L.Get("Star Tournament"), L.Get("You have already claimed the reward on another device."), L.Get("OK"), null);
                        this.Manager.EndStarTournament();
                        break;
                    default:
                        view = PuzzleGameData.DialogViews.ShowMessageBox(L.Get("Star Tournament"), L.Get("Something went wrong, please try again later."), L.Get("OK"), null);
                        break;
                }
                if (view != null)
                {
                    yield return PuzzleGameData.DialogViews.WaitForClosingView(view);
                }
            }
            else
            {
                yield return this.DisplayErrorMessage(ReturnCode.ClientConnectionError);
            }
            yield break;
        }

        private IEnumerator DisplayErrorMessage(ReturnCode error)
        {
            object v = PuzzleGameData.DialogViews.ShowMessageBox(L.Get("Star Tournament"), L.Get("Something went wrong, please try again later."), L.Get("OK"), null);
            yield return PuzzleGameData.DialogViews.WaitForClosingView(v);
            yield break;
        }

        public IEnumerator WaitForSyncingEnd()
        {
            while (!this.syncFiber.IsTerminated)
            {
                yield return null;
            }
            yield break;
        }

        public void SyncStatus()
        {
            this.syncFiber.Start(this.SyncStatusCr());
        }

        private IEnumerator SyncStatusCr()
        {
            object progressView = PuzzleGameData.DialogViews.ShowProgressView(L.Get("Loading..."));
            ReturnCode error = ReturnCode.NoError;
            if (this.Manager.IsStarTournamentActive)
            {
                yield return this.SubmitScore(0, delegate (object e)
                {
                    error = ((e != null) ? ((ReturnCode)e) : error);
                });
                yield return this.HandleError(error, progressView);
                yield return this.HandleActive(progressView);
            }
            else
            {
                yield return this.HandleEnded(progressView);
            }
            yield break;
        }

        private IEnumerator HandleActive(object loadingView)
        {
            ReturnCode error = ReturnCode.NoError;
            yield return this.Status(delegate (object e)
            {
                error = ((e != null) ? ReturnCode.ClientConnectionError : error);
            });
            yield return this.HandleError(error, loadingView);
            PuzzleGameData.DialogViews.CloseView(loadingView);
            yield return PuzzleGameData.DialogViews.WaitForClosingView(loadingView);
            yield return this.Manager.ShowLeaderboard();
            yield break;
        }

        private IEnumerator HandleEnded(object loadingView)
        {
            ReturnCode error = ReturnCode.NoError;
            StarTournamentCloud.PresentResult presentResult = StarTournamentCloud.PresentResult.Unknown;
            yield return this.GetRewardStatus(delegate (object e, StarTournamentCloud.PresentResult r)
            {
                error = ((e != null) ? ReturnCode.ClientConnectionError : error);
                presentResult = r;
            });
            yield return this.HandleError(error, loadingView);
            if (presentResult == StarTournamentCloud.PresentResult.TournamentActive)
            {
                yield return this.HandleError(ReturnCode.NotLatestVersion, loadingView);
                yield return this.Status(delegate (object e)
                {
                    error = ((e != null) ? ReturnCode.ClientConnectionError : error);
                });
                yield return this.HandleError(error, loadingView);
                PuzzleGameData.DialogViews.CloseView(loadingView);
                yield return PuzzleGameData.DialogViews.WaitForClosingView(loadingView);
                yield return this.ShowResults(null, presentResult);
                yield break;
            }
            PuzzleGameData.DialogViews.CloseView(loadingView);
            yield return PuzzleGameData.DialogViews.WaitForClosingView(loadingView);
            yield return this.ShowResults(null, presentResult);
            yield break;
        }

        private IEnumerator HandleError(ReturnCode error, object loadingView)
        {
            if (error == ReturnCode.NotLatestVersion)
            {
                TactileModules.FeatureManager.FeatureManager.Instance.UpdateFeatureLists(delegate (object e)
                {
                    error = ((e != null) ? ReturnCode.ClientConnectionError : ReturnCode.NoError);
                });
                yield return TactileModules.FeatureManager.FeatureManager.Instance.WaitForFeatureListUpdate();
            }
            if (error != ReturnCode.NoError)
            {
                PuzzleGameData.DialogViews.CloseView(loadingView);
                yield return PuzzleGameData.DialogViews.WaitForClosingView(loadingView);
                yield return this.DisplayErrorMessage(error);
                this.syncFiber.Terminate();
            }
            yield break;
        }

        public IEnumerator Status(Action<object> callback)
        {
            yield return this.StarTournamentStatus(this.FeatureId, delegate (object err, string featureId, Entry entry, List<Entry> entries, List<CloudUser> users)
            {
                if (err == null)
                {
                    this.Manager.LocalPersistedState.UpdateState(entries, users, entry);
                }
                if (callback != null)
                {
                    callback(err);
                }
            });
            yield break;
        }

        public void Submit(int diffStars)
        {
            if (diffStars > 0)
            {
                diffStars = Math.Min(diffStars, 3);
                FiberCtrl.Pool.Run(this.SubmitScore(diffStars, null), false);
            }
        }

        private IEnumerator SubmitScore(int stars, Action<object> callback = null)
        {
            this.Manager.LocalPersistedState.PersistedUnsubmittedStars += stars;
            object error = null;
            yield return this.StarTournamentSubmitScore(this.FeatureId, this.Manager.FarthestUnlockedLevelNumber, this.Manager.LocalPersistedState.PersistedUnsubmittedStars, delegate (object err, string featureId)
            {
                if (err != null)
                {
                    error = err;
                }
                else
                {
                    this.Manager.LocalPersistedState.PersistedUnsubmittedStars = 0;
                }
            });
            if (callback != null)
            {
                callback(error);
            }
            yield break;
        }

        private IEnumerator GetRewardStatus(Action<object, StarTournamentCloud.PresentResult> callback)
        {
            if (this.IsResultReceived)
            {
                if (callback != null)
                {
                    callback(null, (StarTournamentCloud.PresentResult)this.ReceivedResult);
                }
                yield break;
            }
            if (this.hasSentPresentRequest)
            {
                yield break;
            }
            yield return new Fiber.OnExit(delegate ()
            {
                this.hasSentPresentRequest = false;
            });
            this.hasSentPresentRequest = true;
            yield return this.StarTournamentPresent(this.FeatureId, delegate (object err, StarTournamentCloud.PresentResult presentResult, List<Entry> entries, List<CloudUser> users)
            {
                if (err == null)
                {
                    this.Manager.LocalPersistedState.UpdateState(entries, users, null);
                    if (presentResult == StarTournamentCloud.PresentResult.NoValidEntry || presentResult == StarTournamentCloud.PresentResult.Presented || presentResult == StarTournamentCloud.PresentResult.PreviouslyPresented)
                    {
                        this.Manager.LocalPersistedState.ReceivedResult = (int)presentResult;
                    }
                }
                if (callback != null)
                {
                    callback(err, presentResult);
                }
            });
            yield break;
        }

        public void RequestPresentForOldTournament(string id)
        {
            FiberCtrl.Pool.Run(this.PresentForOldTournament(id), false);
        }

        private IEnumerator PresentForOldTournament(string id)
        {
            yield return this.StarTournamentPresent(id, delegate (object err, StarTournamentCloud.PresentResult presentResult, List<Entry> entries, List<CloudUser> users)
            {
                if (err != null || presentResult != StarTournamentCloud.PresentResult.Presented)
                {
                    return;
                }
                entries.Sort((Entry entry1, Entry entry2) => entry2.Stars - entry1.Stars);
                string deviceId = (!this.HasValidDevice) ? string.Empty : this.CachedDevice.CloudId;
                string userId = (!this.HasValidUser) ? string.Empty : this.CachedMe.CloudId;
                for (int i = 0; i < entries.Count; i++)
                {
                    if (entries[i].IsOwnedByDeviceOrUser(deviceId, userId))
                    {
                        StarTournamentConfig.Reward reward = this.Manager.GetReward(i + 1);
                        if (reward != null)
                        {
                            this.Manager.GetFeatureTypeCustomData<StarTournamentInstanceCustomData, FeatureMetaData, StarTournamentTypeCustomData>().OldTournamentInfo = new OldStartTournamentInfo(id, 1, reward);
                        }
                    }
                }
            });
            yield break;
        }

        public IEnumerator StarTournamentJoin(string featureId, int farthestUnlockedLevelIndex, StarTournamentCloud.StarTournamentUpdateResultDelegate callback)
        {
            if (!this.HasValidDevice)
            {
                callback("No valid cloud device", string.Empty, null, null, null);
                yield break;
            }
            StarTournamentCloud.StarTournamentResponse response = new StarTournamentCloud.StarTournamentResponse();
            yield return this.cloudInterfaceBase.StarTournamentJoin((!this.HasValidUser) ? null : this.CachedMe.CloudId, featureId, farthestUnlockedLevelIndex, response);
            if (response.Success)
            {
                callback(null, response.FeatureId, response.Entry, response.Entries, response.Users);
            }
            else
            {
                callback(response.ReturnCode, string.Empty, null, null, null);
            }
            yield break;
        }

        public IEnumerator StarTournamentStatus(string featureId, StarTournamentCloud.StarTournamentUpdateResultDelegate callback)
        {
            if (!this.HasValidDevice)
            {
                callback("No valid cloud device", string.Empty, null, null, null);
                yield break;
            }
            StarTournamentCloud.StarTournamentResponse response = new StarTournamentCloud.StarTournamentResponse();
            yield return this.cloudInterfaceBase.StarTournamentStatus((!this.HasValidUser) ? null : this.CachedMe.CloudId, featureId, RequestPriority.Interactive, response);
            if (response.Success)
            {
                callback(null, response.FeatureId, response.Entry, response.Entries, response.Users);
            }
            else
            {
                callback(response.ReturnCode, string.Empty, null, null, null);
            }
            yield break;
        }

        public IEnumerator StarTournamentSubmitScore(string featureId, int farthestUnlockLevel, int score, StarTournamentCloud.StarTournamentSubmitScoreResultDelegate callback)
        {
            if (!this.HasValidDevice)
            {
                callback("No valid cloud device!", string.Empty);
                yield break;
            }
            StarTournamentCloud.StarTournamentResponse response = new StarTournamentCloud.StarTournamentResponse();
            yield return this.cloudInterfaceBase.StarTournamentSubmitScore((!this.HasValidUser) ? null : this.CachedMe.CloudId, featureId, farthestUnlockLevel, score, RequestPriority.Interactive, response);
            if (response.Success)
            {
                callback(null, response.FeatureId);
            }
            else
            {
                callback(response.ReturnCode, string.Empty);
            }
            yield break;
        }

        public IEnumerator StarTournamentPresent(string featureId, StarTournamentCloud.StarTournamentPresentResultDelgate callback)
        {
            if (!this.HasValidDevice)
            {
                callback("No valid cloud device!", StarTournamentCloud.PresentResult.Unknown, null, null);
                yield break;
            }
            StarTournamentCloud.StarTournamentResponse response = new StarTournamentCloud.StarTournamentResponse();
            yield return this.cloudInterfaceBase.StarTournamentPresent((!this.HasValidUser) ? null : this.CachedMe.CloudId, featureId, RequestPriority.Interactive, response);
            if (response.Success)
            {
                callback(null, response.PresentResult, response.Entries, response.Users);
            }
            else
            {
                callback(response.ReturnCode, StarTournamentCloud.PresentResult.Unknown, null, null);
            }
            yield break;
        }

        public const int DEFAULT_PRESENT_RESULT = -100;

        private readonly CloudClientBase cloudClientBase;

        private readonly ICloudInterfaceBase cloudInterfaceBase;

        private readonly Fiber syncFiber = new Fiber();

        private bool hasSentJoinRequest;

        public enum PresentResult
        {
            Unknown = -2,
            NoValidEntry,
            TournamentActive,
            Presented,
            PreviouslyPresented
        }

        public delegate void StarTournamentUpdateResultDelegate(object error, string featureId, Entry entry, List<Entry> entries, List<CloudUser> users);

        public delegate void StarTournamentSubmitScoreResultDelegate(object error, string featureId);

        public delegate void StarTournamentPresentResultDelgate(object error, StarTournamentCloud.PresentResult presentResult, List<Entry> entries, List<CloudUser> users);

        public class StarTournamentResponse : Response
        {
            public Entry Entry
            {
                get
                {
                    return JsonSerializer.HashtableToObject<Entry>((Hashtable)base.data["entry"]);
                }
            }

            public List<Entry> Entries
            {
                get
                {
                    Hashtable hashtable = (Hashtable)base.data["competitors"];
                    if (hashtable == null || hashtable.Count <= 0)
                    {
                        return null;
                    }
                    List<Entry> list = new List<Entry>();
                    ArrayList arrayList = (ArrayList)hashtable["entries"];
                    IEnumerator enumerator = arrayList.GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            object obj = enumerator.Current;
                            Hashtable table = (Hashtable)obj;
                            list.Add(JsonSerializer.HashtableToObject<Entry>(table));
                        }
                    }
                    finally
                    {
                        IDisposable disposable;
                        if ((disposable = (enumerator as IDisposable)) != null)
                        {
                            disposable.Dispose();
                        }
                    }
                    return list;
                }
            }

            public List<CloudUser> Users
            {
                get
                {
                    Hashtable hashtable = (Hashtable)base.data["competitors"];
                    if (hashtable == null || hashtable.Count <= 0)
                    {
                        return null;
                    }
                    List<CloudUser> list = new List<CloudUser>();
                    ArrayList arrayList = (ArrayList)hashtable["users"];
                    IEnumerator enumerator = arrayList.GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            object obj = enumerator.Current;
                            Hashtable table = (Hashtable)obj;
                            list.Add(JsonSerializer.HashtableToObject<CloudUser>(table));
                        }
                    }
                    finally
                    {
                        IDisposable disposable;
                        if ((disposable = (enumerator as IDisposable)) != null)
                        {
                            disposable.Dispose();
                        }
                    }
                    return list;
                }
            }

            public string FeatureId
            {
                get
                {
                    return (string)base.data["featureId"];
                }
            }

            public StarTournamentCloud.PresentResult PresentResult
            {
                get
                {
                    return (StarTournamentCloud.PresentResult)((double)base.data["presentResult"]);
                }
            }
        }
    }
}
