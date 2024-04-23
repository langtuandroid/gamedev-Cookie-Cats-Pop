using System;
using UnityEngine;

namespace TactileModules.TactileCloud.AssetBundles
{
    public class ManagedAssetbundle
    {
        public ManagedAssetbundle(AssetBundleManager assetBundleManager, string assetBundleName, AssetBundleManager.IManagedAssetBundleHandler handler)
        {
            this._handler = handler;
            this._assetBundleName = assetBundleName;
            this._assetBundleManager = assetBundleManager;
        }

        public ManagedAssetbundle.State CurrentState
        {
            get
            {
                return this._currentState;
            }
            private set
            {
                bool flag = this._currentState != value;
                if (flag)
                {
                }
                this._currentState = value;
                if (flag)
                {
                    this._handler.OnStateChanged(this, this._currentState);
                }
            }
        }

        public string AssetBundleName
        {
            get
            {
                return this._assetBundleName;
            }
        }

        public UnityEngine.AssetBundle AssetBundle
        {
            get
            {
                return this.ab;
            }
        }

        public AssetBundleInfo AssetBundleInfo
        {
            get
            {
                return this.abinfo;
            }
        }

        public AssetBundleManager.IManagedAssetBundleHandler Handler
        {
            get
            {
                return this._handler;
            }
        }

        public void ConsumeChanges(bool unload)
        {
            if (this.CurrentState == ManagedAssetbundle.State.AWAITING_CONSUMPTION)
            {
                this.CurrentState = ManagedAssetbundle.State.CONSUMED;
                if (unload)
                {
                    this.UnloadBundle();
                }
            }
        }

        public void UnloadBundle()
        {
            this.CurrentState = ManagedAssetbundle.State.UNLOAD;
            this._assetBundleManager.TryAndUnloadManagedAssetBundle(this, false);
        }

        internal void CleanupAfterUnload()
        {
            this.ab = null;
            this.abinfo = null;
            this.CurrentState = ManagedAssetbundle.State.UNLOADED;
        }

        internal void MarkUnavailable()
        {
            this.CurrentState = ManagedAssetbundle.State.UNAVAILABLE;
            this.ab = null;
            this.abinfo = null;
        }

        internal void MarkAvailable()
        {
            this.CurrentState = ManagedAssetbundle.State.AVAILABLE;
        }

        internal void MarkLoading()
        {
            this.CurrentState = ManagedAssetbundle.State.LOADING;
        }

        internal void MarkDisposed()
        {
            this.CurrentState = ManagedAssetbundle.State.NONE;
        }

        public void UpdateAssetBundle(UnityEngine.AssetBundle bundle, AssetBundleInfo info)
        {
            this.ab = bundle;
            this.abinfo = info;
            this.CurrentState = ManagedAssetbundle.State.AWAITING_CONSUMPTION;
        }

        public T GetAssetFromBundle<T>(string assetName) where T : UnityEngine.Object
        {
            return this.ab.LoadAsset<T>(assetName);
        }

        private ManagedAssetbundle.State _currentState;

        private AssetBundleManager.IManagedAssetBundleHandler _handler;

        private string _assetBundleName;

        private AssetBundleManager _assetBundleManager;

        private UnityEngine.AssetBundle ab;

        private AssetBundleInfo abinfo;

        public enum State
        {
            NONE,
            AVAILABLE,
            UNAVAILABLE,
            LOADING,
            AWAITING_CONSUMPTION,
            CONSUMED,
            UNLOAD,
            UNLOADED,
            ERROR
        }
    }
}
