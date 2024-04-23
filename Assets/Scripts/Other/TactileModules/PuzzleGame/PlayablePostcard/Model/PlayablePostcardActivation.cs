using System;
using TactileModules.PuzzleGame.PlayablePostcard.UI;
using UnityEngine;

namespace TactileModules.PuzzleGame.PlayablePostcard.Model
{
	public class PlayablePostcardActivation
	{
		public PlayablePostcardActivation(PlayablePostcardHandler handler, FeatureDataProvider<PlayablePostcardInstanceCustomData> instanceData, FeatureDataProvider<PlayablePostcardMetaData> metaData, PostcardAssetBundleDownloader postcardAssetBundleDownloader)
		{
			this.handler = handler;
			this.instanceData = instanceData;
			this.metaData = metaData;
			this.postcardAssetBundleDownloader = postcardAssetBundleDownloader;
		}

		public void StartPopupHasBeenShown()
		{
			this.instanceData.Get().HasShownStartPopup = true;
		}

		public bool HasShownStartPopup()
		{
			return this.CanShow() && this.instanceData.Get().HasShownStartPopup;
		}

		public bool CanShowElseStartLoading()
		{
			if (this.CanShow())
			{
				return true;
			}
			if (this.ShouldShow())
			{
				this.StartLoading();
			}
			return false;
		}

		private bool CanShow()
		{
			return this.handler.IsActive() && this.postcardAssetBundleDownloader.IsAssetBundleLoaded(this.metaData.Get().AssetBundleName);
		}

		private bool ShouldShow()
		{
			return this.handler.IsActive() && this.postcardAssetBundleDownloader.CanAssetBundleBeDownloaded(this.metaData.Get().AssetBundleName);
		}

		private void StartLoading()
		{
			this.postcardAssetBundleDownloader.LoadAssetBundle(this.metaData.Get().AssetBundleName);
		}

		public PostcardItemContainer GetAssetBundle()
		{
			AssetBundle assetBundle = this.postcardAssetBundleDownloader.GetAssetBundle(this.metaData.Get().AssetBundleName);
			return assetBundle.LoadAsset<GameObject>(this.metaData.Get().AssetBundleName).GetComponent<PostcardItemContainer>();
		}

		public string GetAssetBundleName()
		{
			return this.metaData.Get().AssetBundleName;
		}

		public string GetAssetBundleUrl()
		{
			return this.postcardAssetBundleDownloader.GetUrlFromAssetBundleName(this.metaData.Get().AssetBundleName);
		}

		private readonly PlayablePostcardHandler handler;

		private readonly FeatureDataProvider<PlayablePostcardInstanceCustomData> instanceData;

		private readonly FeatureDataProvider<PlayablePostcardMetaData> metaData;

		private readonly PostcardAssetBundleDownloader postcardAssetBundleDownloader;
	}
}
