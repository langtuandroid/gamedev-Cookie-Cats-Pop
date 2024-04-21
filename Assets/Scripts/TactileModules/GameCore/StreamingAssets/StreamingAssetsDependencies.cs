using System;
using UnityEngine;

namespace TactileModules.GameCore.StreamingAssets
{
	public class StreamingAssetsDependencies : ScriptableObject
	{
		public DownloadManager DownloadManager { get; private set; }

		public void SetDependencies(DownloadManager downloadManager)
		{
			this.DownloadManager = downloadManager;
		}
	}
}
