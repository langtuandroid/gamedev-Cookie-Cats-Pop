using System;
using System.Collections.Generic;
using UnityEngine;

namespace TactileModules.GameCore.StreamingAssets
{
	[Serializable]
	public class StreamingAsset : IStreamingAsset
	{
		public StreamingAsset()
		{
			this.source = new AssetReference();
		}

		public StreamingAsset(StreamingAssetsDependencies dependencies)
		{
			this.source = new AssetReference();
			this.dependencies = dependencies;
		}

		public bool ForceUseLocal { get; set; }

		public void InvokeAssetChanged()
		{
			if (this.AssetChanged != null)
			{
				this.AssetChanged();
			}
		}

		public void SetDependencies(StreamingAssetsDependencies dependencies)
		{
			this.dependencies = dependencies;
		}

		public AssetReference Source
		{
			get
			{
				return this.source;
			}
		}

		public UnityEngine.Object Asset
		{
			get
			{
				StreamingAsset.LoadedAsset loadedAsset;
				return (!StreamingAsset.loadedAssets.TryGetValue(this.source.Guid, out loadedAsset)) ? null : loadedAsset.Asset;
			}
		}

		public T GetAsset<T>() where T : UnityEngine.Object
		{
			return (T)((object)this.Asset);
		}

		public void Load()
		{
			StreamingAsset.LoadedAsset loadedAsset;
			if (!StreamingAsset.loadedAssets.TryGetValue(this.source.Guid, out loadedAsset))
			{
				loadedAsset = new StreamingAsset.LoadedAsset(this.source.Guid, this.dependencies);
				StreamingAsset.loadedAssets.Add(this.source.Guid, loadedAsset);
			}
			loadedAsset.Load(this);
			this.InvokeAssetChanged();
		}

		public void Unload()
		{
			StreamingAsset.LoadedAsset loadedAsset;
			if (StreamingAsset.loadedAssets.TryGetValue(this.source.Guid, out loadedAsset) && loadedAsset.Unload(this))
			{
				StreamingAsset.loadedAssets.Remove(this.source.Guid);
			}
			this.InvokeAssetChanged();
		}

		public void StartDownload()
		{
			this.Load();
			this.Unload();
		}

		[SerializeField]
		private StreamingAssetsDependencies dependencies;

		[SerializeField]
		private AssetReference source;

		private static readonly Dictionary<string, StreamingAsset.LoadedAsset> loadedAssets = new Dictionary<string, StreamingAsset.LoadedAsset>();

		public Action AssetChanged;

		private class LoadedAsset
		{
			public LoadedAsset(string guid, StreamingAssetsDependencies dependencies)
			{
				this.guid = guid;
				this.dependencies = dependencies;
			}

			public UnityEngine.Object Asset
			{
				get
				{
					return this.asset;
				}
				private set
				{
					if (this.asset != null)
					{
						Resources.UnloadAsset(this.asset);
					}
					this.asset = value;
					for (int i = 0; i < this.streamingAssets.Count; i++)
					{
						this.streamingAssets[i].InvokeAssetChanged();
					}
				}
			}

			public void Load(IStreamingAsset streamingAsset)
			{
				if (this.dependencies.DownloadManager == null)
				{
					return;
				}
				this.streamingAssets.Add(streamingAsset);
				if (this.streamingAssets.Count == 1 && this.dependencies != null)
				{
					this.assetBundle = this.dependencies.DownloadManager.Load(this.guid);
					this.assetBundle.DownloadComplete += this.LoadAsset;
					this.LoadAsset();
				}
			}

			public bool Unload(IStreamingAsset streamingAsset)
			{
				if (this.dependencies.DownloadManager == null)
				{
					return false;
				}
				this.streamingAssets.Remove(streamingAsset);
				if (this.streamingAssets.Count == 0)
				{
					if (this.Asset != null)
					{
						this.Asset = null;
					}
					if (this.assetBundle != null)
					{
						this.assetBundle.DownloadComplete -= this.LoadAsset;
					}
					if (this.dependencies != null)
					{
						this.dependencies.DownloadManager.Unload(this.assetBundle);
					}
					this.assetBundle = null;
					return true;
				}
				return false;
			}

			private void LoadAsset()
			{
				if (this.assetBundle.AssetBundle != null)
				{
					this.Asset = this.assetBundle.AssetBundle.LoadAllAssets()[0];
				}
				else
				{
					this.Asset = Resources.Load<Texture2D>(this.guid);
				}
			}

			private readonly string guid;

			private readonly StreamingAssetsDependencies dependencies;

			private readonly List<IStreamingAsset> streamingAssets = new List<IStreamingAsset>();

			private UnityEngine.Object asset;

			private DownloadManager.LoadedAssetBundle assetBundle;
		}
	}
}
