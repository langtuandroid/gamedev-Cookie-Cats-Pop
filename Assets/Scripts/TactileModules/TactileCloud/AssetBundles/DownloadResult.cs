using System;
using UnityEngine;

namespace TactileModules.TactileCloud.AssetBundles
{
    public class DownloadResult
    {
        public DownloadResult(UnityEngine.AssetBundle assetBundle, string error)
        {
            this.assetBundle = assetBundle;
            this.error = error;
        }

        public bool Success
        {
            get
            {
                return string.IsNullOrEmpty(this.error);
            }
        }

        public readonly UnityEngine.AssetBundle assetBundle;

        public readonly string error;
    }
}
