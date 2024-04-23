using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Fibers;
using TactileModules.Foundation.CloudSynchronization;
using UnityEngine;

namespace TactileModules.TactileCloud.AssetBundles
{
    public class AssetBundleManager : SingleInstance<AssetBundleManager>, ICloudSynchronizable, IAvailableAssetBundles
    {
        public AssetBundleManager(CloudClientBase cloudClient, AssetBundleManager.PauseLoadingCheck pauseLoadingCheck, IAssetBundleDownloader assetBundleDownloader, IPersistableStateHandler persistableStateHandler)
        {
            this.cloudClient = cloudClient;
            this._pauseLoadingCheck = pauseLoadingCheck;
            this.assetBundleDownloader = assetBundleDownloader;
            this.persistableStateHandler = persistableStateHandler;
            if (this.PersistedState.LastSeenBundleVersion != int.Parse(SystemInfoHelper.BundleVersion))
            {
                this.PersistedState.AvailableAssetBundlesVersion = 0;
                this.PersistedState.AvailableAssetBundles = new Dictionary<string, AssetBundleInfo>();
            }
            this.PersistedState.LastSeenBundleVersion = int.Parse(SystemInfoHelper.BundleVersion);
            persistableStateHandler.Save();
        }

        private PersistableState PersistedState
        {
            get
            {
                return this.persistableStateHandler.Get();
            }
        }

        public Dictionary<string, AssetBundleInfo> AvailableAssetBundles
        {
            get
            {
                return this.PersistedState.AvailableAssetBundles;
            }
        }

        //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public event Action<Dictionary<string, AssetBundleInfo>> AvailableAssetBundlesUpdated;

        public IEnumerator LoadAvailableAssetBundlesCr()
        {
            if (this.loadAvailableAssetBundlesInProgress)
            {
                while (this.loadAvailableAssetBundlesInProgress)
                {
                    yield return null;
                }
            }
            else
            {
                this.loadAvailableAssetBundlesInProgress = true;
                yield return this.LoadAvailableAssetBundlesInternalCr();
                this.loadAvailableAssetBundlesInProgress = false;
            }
            yield break;
        }

        private IEnumerator LoadAvailableAssetBundlesInternalCr()
        {
            while (this.cloudClient.CachedDevice == null)
            {
                yield return null;
            }
            yield return this.cloudClient.GetAssetBundles(this.PersistedState.AvailableAssetBundlesVersion, delegate (object error, int latestVersion, Dictionary<string, AssetBundleInfo> latestAvailableAssetBundles)
            {
                if (latestAvailableAssetBundles != null)
                {
                    this.PersistedState.AvailableAssetBundlesVersion = latestVersion;
                    this.PersistedState.AvailableAssetBundles = latestAvailableAssetBundles;
                    this.persistableStateHandler.Save();
                    this.OnAvailableAssetBundlesUpdated();
                }
                FiberCtrl.Pool.Run(this.LoadManagedAssetBundles(), false);
            });
            yield break;
        }

        public IEnumerator LoadManagedAssetBundles()
        {
            foreach (KeyValuePair<string, List<ManagedAssetbundle>> keyValuePair in this.managedAssetBundles)
            {
                Dictionary<string, AssetBundleInfo> availableAssetBundles = this.PersistedState.AvailableAssetBundles;
                if (availableAssetBundles.ContainsKey(keyValuePair.Key))
                {
                    if (!this.loadedManagedAssetBundles.ContainsKey(keyValuePair.Key) || this.loadedManagedAssetBundles[keyValuePair.Key] != availableAssetBundles[keyValuePair.Key].URL)
                    {
                        this.UpdateManagedAssetBundleState(keyValuePair.Key, ManagedAssetbundle.State.AVAILABLE);
                        if (!this.assetBundleLoadingQueue.Contains(keyValuePair.Key))
                        {
                            if (Caching.IsVersionCached(availableAssetBundles[keyValuePair.Key].URL, 0))
                            {
                                this.assetBundleLoadingQueue.Insert(0, keyValuePair.Key);
                            }
                            else
                            {
                                this.assetBundleLoadingQueue.Add(keyValuePair.Key);
                            }
                        }
                    }
                }
                else
                {
                    this.UpdateManagedAssetBundleState(keyValuePair.Key, ManagedAssetbundle.State.UNAVAILABLE);
                }
            }
            yield return this.ProcessAssetbundleQueue();
            yield break;
        }

        public ManagedAssetbundle RegisterManagedAssetBundleHandler(string assetBundleName, AssetBundleManager.IManagedAssetBundleHandler handler)
        {
            if (!this.managedAssetBundles.ContainsKey(assetBundleName))
            {
                this.managedAssetBundles.Add(assetBundleName, new List<ManagedAssetbundle>());
            }
            ManagedAssetbundle managedAssetbundle = new ManagedAssetbundle(this, assetBundleName, handler);
            this.managedAssetBundles[assetBundleName].Add(managedAssetbundle);
            FiberCtrl.Pool.Run(this.LoadManagedAssetBundles(), false);
            return managedAssetbundle;
        }

        public void UnregisterAssetBundleHandler(string assetBundleName, AssetBundleManager.IManagedAssetBundleHandler handler)
        {
            if (this.managedAssetBundles.ContainsKey(assetBundleName))
            {
                List<ManagedAssetbundle> list = this.managedAssetBundles[assetBundleName];
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    ManagedAssetbundle managedAssetbundle = list[i];
                    if (managedAssetbundle.Handler == handler)
                    {
                        list.RemoveAt(i);
                        if (list.Count == 0)
                        {
                            ManagedAssetbundle managedAssetbundle2 = managedAssetbundle;
                            managedAssetbundle2.UnloadBundle();
                            managedAssetbundle2.MarkDisposed();
                        }
                    }
                }
            }
        }

        internal UnityEngine.AssetBundle GetAssetBundleByName(string assetBundleName)
        {
            if (this.loadedAssetBundles.ContainsKey(assetBundleName))
            {
                return this.loadedAssetBundles[assetBundleName];
            }
            return null;
        }

        internal void TryAndUnloadManagedAssetBundle(ManagedAssetbundle managedAssetbundle, bool forceUnload = false)
        {
            string assetBundleName = managedAssetbundle.AssetBundleName;
            bool flag = true;
            for (int i = 0; i < this.managedAssetBundles[assetBundleName].Count; i++)
            {
                ManagedAssetbundle managedAssetbundle2 = this.managedAssetBundles[assetBundleName][i];
                flag &= (managedAssetbundle2.CurrentState == ManagedAssetbundle.State.UNLOAD);
            }
            if (flag || forceUnload)
            {
                if (managedAssetbundle.AssetBundle != null)
                {
                    managedAssetbundle.AssetBundle.Unload(false);
                }
                this.UpdateManagedAssetBundleState(assetBundleName, ManagedAssetbundle.State.UNLOADED);
            }
        }

        private IEnumerator ProcessAssetbundleQueue()
        {
            if (this.isLoadingBundlesInQueue)
            {
                while (this.isLoadingBundlesInQueue)
                {
                    yield return null;
                }
            }
            else
            {
                this.isLoadingBundlesInQueue = true;
                while (this.assetBundleLoadingQueue.Count > 0)
                {
                    yield return this.WaitWhilePauseLoading();
                    string assetBundleToLoad = this.assetBundleLoadingQueue[0];
                    this.assetBundleLoadingQueue.RemoveAt(0);
                    Dictionary<string, AssetBundleInfo> assetBundles = this.PersistedState.AvailableAssetBundles;
                    if (!assetBundles.ContainsKey(assetBundleToLoad))
                    {
                        AssetBundleManager.LogError("AssetBundleManager/ProcessAssetbundleQueue - asset bundle did not exist in available asset bundles - should never happen. assetBundleToLoad=" + assetBundleToLoad);
                    }
                    else
                    {
                        if (this.managedAssetBundles.ContainsKey(assetBundleToLoad) && this.managedAssetBundles[assetBundleToLoad].Count > 0)
                        {
                            this.TryAndUnloadManagedAssetBundle(this.managedAssetBundles[assetBundleToLoad][0], true);
                        }
                        this.UpdateManagedAssetBundleState(assetBundleToLoad, ManagedAssetbundle.State.LOADING);
                        string url = assetBundles[assetBundleToLoad].URL;
                        EnumeratorResult<DownloadResult> result = new EnumeratorResult<DownloadResult>();
                        yield return this.assetBundleDownloader.DownloadAssetBundle(url, result);
                        if (!result.value.Success)
                        {
                            AssetBundleManager.Log("AssetBundleManager/LoadAssetBundles - failed to load asset bundle - AssetBundle=" + result.value.error);
                        }
                        else
                        {
                            UnityEngine.AssetBundle assetBundle = result.value.assetBundle;
                            yield return this.WaitWhilePauseLoading();
                            if (assetBundle != null)
                            {
                                AssetBundleManager.Log("AssetBundleManager/LoadAssetBundles - asset bundle loaded - AssetBundle= " + assetBundleToLoad + ", URL=" + url);
                                this.loadedManagedAssetBundles[assetBundleToLoad] = url;
                                if (this.managedAssetBundles.ContainsKey(assetBundleToLoad))
                                {
                                    List<ManagedAssetbundle> list = this.managedAssetBundles[assetBundleToLoad];
                                    for (int i = list.Count - 1; i >= 0; i--)
                                    {
                                        list[i].UpdateAssetBundle(assetBundle, assetBundles[assetBundleToLoad]);
                                    }
                                }
                            }
                            else
                            {
                                AssetBundleManager.LogError("AssetBundleManager/LoadAssetBundles -  WWW completed without error, but no asset bundle returned - AssetBundle=" + assetBundleToLoad + ", URL=" + url);
                            }
                        }
                    }
                }
                this.isLoadingBundlesInQueue = false;
            }
            yield break;
        }

        private void UpdateManagedAssetBundleState(string assetBundleName, ManagedAssetbundle.State state)
        {
            if (this.managedAssetBundles.ContainsKey(assetBundleName))
            {
                List<ManagedAssetbundle> list = this.managedAssetBundles[assetBundleName];
                for (int i = 0; i < list.Count; i++)
                {
                    ManagedAssetbundle managedAssetbundle = list[i];
                    switch (state)
                    {
                        case ManagedAssetbundle.State.NONE:
                            managedAssetbundle.MarkDisposed();
                            break;
                        case ManagedAssetbundle.State.AVAILABLE:
                            managedAssetbundle.MarkAvailable();
                            break;
                        case ManagedAssetbundle.State.UNAVAILABLE:
                            managedAssetbundle.MarkUnavailable();
                            break;
                        case ManagedAssetbundle.State.LOADING:
                            managedAssetbundle.MarkLoading();
                            break;
                        case ManagedAssetbundle.State.UNLOADED:
                            managedAssetbundle.CleanupAfterUnload();
                            break;
                    }
                }
            }
        }

        private IEnumerator WaitWhilePauseLoading()
        {
            while (this._pauseLoadingCheck())
            {
                yield return null;
            }
            yield break;
        }

        private void OnAvailableAssetBundlesUpdated()
        {
            if (this.AvailableAssetBundlesUpdated != null)
            {
                this.AvailableAssetBundlesUpdated(this.PersistedState.AvailableAssetBundles);
            }
        }

        private static void Log(string message)
        {

        }

        private static void LogError(string message)
        {
           
        }

        public void FoceReloadAllManagedAssetBundles()
        {
            this.loadedManagedAssetBundles.Clear();
            FiberCtrl.Pool.Run(this.LoadManagedAssetBundles(), false);
        }

        public IEnumerator Synchronize()
        {
            yield return this.LoadAvailableAssetBundlesCr();
            yield break;
        }

        private readonly CloudClientBase cloudClient;

        public AssetBundleManager.PauseLoadingCheck _pauseLoadingCheck;

        private readonly IAssetBundleDownloader assetBundleDownloader;

        private readonly IPersistableStateHandler persistableStateHandler;

        private bool loadAvailableAssetBundlesInProgress;

        private readonly Dictionary<string, List<ManagedAssetbundle>> managedAssetBundles = new Dictionary<string, List<ManagedAssetbundle>>();

        private readonly List<string> assetBundleLoadingQueue = new List<string>();

        private bool isLoadingBundlesInQueue;

        private readonly Dictionary<string, string> loadedManagedAssetBundles = new Dictionary<string, string>();

        private readonly Dictionary<string, UnityEngine.AssetBundle> loadedAssetBundles = new Dictionary<string, UnityEngine.AssetBundle>();

        public delegate bool PauseLoadingCheck();

        public interface IManagedAssetBundleHandler
        {
            void OnStateChanged(ManagedAssetbundle managedAssetbundle, ManagedAssetbundle.State newState);
        }
    }
}
