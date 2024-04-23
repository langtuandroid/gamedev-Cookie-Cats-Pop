using System;
using TactileModules.Ads.Analytics;
using TactileModules.CrossPromotion.Cloud.Data;
using UnityEngine;

namespace TactileModules.CrossPromotion.General.Ads.AdModels
{
	public interface ICrossPromotionAd
	{
		string GetVideoPath();

		Texture2D GetImage();

		Texture2D GetButtonImage();

		CrossPromotionVideoAdCreative GetVideoCreative();

		void EnsureIsCached();

		bool IsCached();

		void ReportAsShown(AdGroupContext adGroupContext);

		void ReportAsClicked(AdGroupContext adGroupContext);

		void ReportAsCompletedWatching(AdGroupContext adGroupContext);

		void ReportAsClosed(AdGroupContext adGroupContext);

		bool CanShowInThisSession();

		bool HasExpired();

		void OpenEmbeddedStoreIfPossible(AdGroupContext adGroupContext, Action onComplete);

		void SendToStoreOrLaunchGame(AdGroupContext adGroupContext, Action onComplete);
	}
}
