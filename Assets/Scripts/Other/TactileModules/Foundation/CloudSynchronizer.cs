using System;
using System.Collections;
using System.Diagnostics;
using Tactile;
using TactileModules.FeatureManager;
using TactileModules.Foundation.CloudSynchronization;
using TactileModules.TactileCloud.AssetBundles;

namespace TactileModules.Foundation
{
    public class CloudSynchronizer
    {
        public CloudSynchronizer(UserSettingsManager userSettingsManager, CloudClientBase cloudClient, FacebookClient fbClient, UserSettingsBackupManager userSettingsBackupManager, ConfigurationManager configurationManager, AssetBundleManager assetBundleManager, LeaderboardManager leaderboardManager, TactileModules.FeatureManager.FeatureManager featureManager, ICloudSynchronizable userSupportSynchronizer, Func<bool> isRequestsBlocked)
        {
            this.userSettingsManager = userSettingsManager;
            this.isRequestsBlocked = isRequestsBlocked;
            this.configurationManager = configurationManager;
            this.assetBundleManager = assetBundleManager;
            this.leaderboardManager = leaderboardManager;
            this.featureManager = featureManager;
            this.userSupportSynchronizer = userSupportSynchronizer;
            this.userSettingsBackupManager = userSettingsBackupManager;
            this.cloudClient = cloudClient;
            this.fbClient = fbClient;
            this.HasSyncedFullThisSession = false;
            ActivityManager.onResumeEvent += this.ApplicationWillEnterForeground;
            this.SyncCloud();
        }

        ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public event Action OnSynchronized;



        public bool HasSyncedFullThisSession { get; private set; }

        public void SyncCloud()
        {
            FiberCtrl.Pool.Run(this.SyncCloudCr(), false);
        }

        private IEnumerator SyncCloudCr()
        {
            if (this.syncCloudInProgress)
            {
                yield break;
            }
            this.syncCloudInProgress = true;
            while (!this.fbClient.IsInitialized)
            {
                yield return null;
            }
            if (this.cloudClient.HasValidUser)
            {
                yield return this.WaitWhileBlocksRequests();
                if (!this.HasSyncedFullThisSession)
                {
                    this.userSettingsManager.ClearHasLoadedSettingsThisSession();
                }
                this.userSettingsManager.SyncUserSettings();
            }
            yield return this.WaitWhileBlocksRequests();
            if (this.fbClient.IsSessionValid)
            {
                yield return this.fbClient.Update();
            }
            else if (this.cloudClient.HasValidUser)
            {
                this.cloudClient.ClearCachedAndPersistedUserData();
            }
            yield return this.WaitWhileBlocksRequests();
            yield return this.cloudClient.UpdateRegistrationCr();
            if (!this.HasSyncedFullThisSession)
            {
                if (this.userSettingsBackupManager.provider.GetConfiguration().BackupOnFullSync)
                {
                    this.userSettingsBackupManager.CreateNewBackup();
                }
                if (this.cloudClient.CachedDevice != null)
                {
                    yield return this.WaitWhileBlocksRequests();
                    yield return this.configurationManager.DownloadConfigurationCr();
                    yield return this.WaitWhileBlocksRequests();
                    yield return this.assetBundleManager.LoadAvailableAssetBundlesCr();
                }
                yield return this.WaitWhileBlocksRequests();
                yield return this.leaderboardManager.SubmitLocalScoresCr();
                yield return this.userSupportSynchronizer.Synchronize();
            }
            yield return this.featureManager.Synchronize();
            this.HasSyncedFullThisSession = true;
            this.syncCloudInProgress = false;
            this.OnSynchronized();
            yield break;
        }

        private void ApplicationWillEnterForeground()
        {
            this.HasSyncedFullThisSession = false;
            this.SyncCloud();
        }

        private IEnumerator WaitWhileBlocksRequests()
        {
            while (this.isRequestsBlocked())
            {
                yield return null;
            }
            yield break;
        }

        private readonly UserSettingsManager userSettingsManager;

        private readonly Func<bool> isRequestsBlocked;

        private readonly ConfigurationManager configurationManager;

        private readonly FacebookClient fbClient;

        private readonly LeaderboardManager leaderboardManager;

        private readonly TactileModules.FeatureManager.FeatureManager featureManager;

        private readonly ICloudSynchronizable userSupportSynchronizer;

        private readonly AssetBundleManager assetBundleManager;

        private readonly UserSettingsBackupManager userSettingsBackupManager;

        private readonly CloudClientBase cloudClient;

        private bool syncCloudInProgress;
    }
}
