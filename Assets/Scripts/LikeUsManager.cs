using System;
using Tactile;
using TactileModules.Foundation;

public class LikeUsManager
{
	private LikeUsManager()
	{
	}

	private ConfigurationManager ConfigurationManager
	{
		get
		{
			return ManagerRepository.Get<ConfigurationManager>();
		}
	}

	public SocialNetworkReward CurrentSocialRewards
	{
		get
		{
			LikeUsManager.PersistableState persistableState = UserSettingsManager.Get<LikeUsManager.PersistableState>();
			SocialConfig config = this.ConfigurationManager.GetConfig<SocialConfig>();
			SocialNetworkReward result;
			if (!persistableState.FacebookFreeGiftConsumed)
			{
				result = config.FacebookRewards;
			}
			else if (!persistableState.TwitterFreeGiftConsumed)
			{
				result = config.TwitterRewards;
			}
			else if (!persistableState.InstagramFreeGiftConsumed)
			{
				result = config.InstagramRewards;
			}
			else
			{
				result = config.YouTubeRewards;
			}
			return result;
		}
	}

	public LikeUsManager.LikeUsType CurrentLikeUsType
	{
		get
		{
			LikeUsManager.PersistableState persistableState = UserSettingsManager.Get<LikeUsManager.PersistableState>();
			LikeUsManager.LikeUsType result;
			if (!persistableState.FacebookFreeGiftConsumed)
			{
				result = LikeUsManager.LikeUsType.Facebook;
			}
			else if (!persistableState.TwitterFreeGiftConsumed)
			{
				result = LikeUsManager.LikeUsType.Twitter;
			}
			else if (!persistableState.InstagramFreeGiftConsumed)
			{
				result = LikeUsManager.LikeUsType.Instagram;
			}
			else
			{
				result = LikeUsManager.LikeUsType.YouTube;
			}
			return result;
		}
	}

	public static LikeUsManager Instance { get; private set; }

	public static LikeUsManager CreateInstance()
	{
		LikeUsManager.Instance = new LikeUsManager();
		return LikeUsManager.Instance;
	}

	public void GiveRewardAndUpdateNextSocialNetwork()
	{
		LikeUsManager.PersistableState persistableState = UserSettingsManager.Get<LikeUsManager.PersistableState>();
		SocialConfig config = this.ConfigurationManager.GetConfig<SocialConfig>();
		for (int i = 0; i < this.CurrentSocialRewards.Rewards.Count; i++)
		{
			InventoryManager.Instance.Add(this.CurrentSocialRewards.Rewards[i].ItemId, this.CurrentSocialRewards.Rewards[i].Amount, "likeUsFreebie");
		}
		switch (this.CurrentLikeUsType)
		{
		case LikeUsManager.LikeUsType.Facebook:
		{
			persistableState.FacebookFreeGiftConsumed = true;
			string appUrl = config.FacebookAppUrl;
			appUrl = config.FacebookAppUrlAndroid;
			LikeUsManager.OpenURL(config.FacebookNormalUrl, appUrl, "com.facebook.katana");
			break;
		}
		case LikeUsManager.LikeUsType.Twitter:
			persistableState.TwitterFreeGiftConsumed = true;
			LikeUsManager.OpenURL(config.TwitterNormalUrl, config.TwitterAppUrl, "com.twitter.android");
			break;
		case LikeUsManager.LikeUsType.Instagram:
			persistableState.InstagramFreeGiftConsumed = true;
			LikeUsManager.OpenURL(config.InstagramNormalUrl, config.InstagramAppUrl, "com.instagram.android");
			break;
		case LikeUsManager.LikeUsType.YouTube:
			persistableState.YouTubeFreeGiftConsumed = true;
			LikeUsManager.OpenURL(config.YouTubeNormalUrl, config.YouTubeAppUrl, "com.youtube.android");
			break;
		}
		UserSettingsManager.Instance.SaveLocalSettings();
	}

	public static void OpenURL(string normalUrl, string appUrl, string packageName)
	{
		if (ActivityAndroid.isPackageInstalled(packageName))
		{
			ActivityAndroid.launchOtherApp(packageName, appUrl);
		}
		else
		{
			EtceteraAndroid.showWebView(normalUrl);
		}
	}

	public enum LikeUsType
	{
		Facebook,
		Twitter,
		Instagram,
		YouTube
	}

	[SettingsProvider("lu", false, new Type[]
	{

	})]
	public class PersistableState : IPersistableState<LikeUsManager.PersistableState>, IPersistableState
	{
		[JsonSerializable("fbFreebie", null)]
		public bool FacebookFreeGiftConsumed { get; set; }

		[JsonSerializable("twFreebie", null)]
		public bool TwitterFreeGiftConsumed { get; set; }

		[JsonSerializable("igFreebie", null)]
		public bool InstagramFreeGiftConsumed { get; set; }

		[JsonSerializable("ytFreebie", null)]
		public bool YouTubeFreeGiftConsumed { get; set; }

		public void MergeFromOther(LikeUsManager.PersistableState newState, LikeUsManager.PersistableState lastCloudState)
		{
			this.FacebookFreeGiftConsumed |= newState.FacebookFreeGiftConsumed;
			this.TwitterFreeGiftConsumed |= newState.TwitterFreeGiftConsumed;
			this.InstagramFreeGiftConsumed |= newState.InstagramFreeGiftConsumed;
			this.YouTubeFreeGiftConsumed |= newState.YouTubeFreeGiftConsumed;
		}
	}
}
