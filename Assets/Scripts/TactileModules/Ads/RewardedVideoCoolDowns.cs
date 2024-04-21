using System;
using TactileModules.Ads.Configuration;
using TactileModules.TactilePrefs;

namespace TactileModules.Ads
{
	public class RewardedVideoCoolDowns : IRewardedVideoCoolDowns
	{
		public RewardedVideoCoolDowns(IAdConfigurationProvider adConfigurationProvider, IRewardedVideoStoreFactory rewardedVideoStoreFactory)
		{
			this.adConfigurationProvider = adConfigurationProvider;
			this.rewardedVideoStoreFactory = rewardedVideoStoreFactory;
		}

		public bool IsPlacementEnabled(string rewardedVideoPlacement)
		{
			RewardedVideoContextConfiguration rewardedVideoPlacementConfig = this.adConfigurationProvider.GetRewardedVideoPlacementConfig(rewardedVideoPlacement);
			return rewardedVideoPlacementConfig != null && rewardedVideoPlacementConfig.IsActive;
		}

		public bool IsInCooldown(string rewardedVideoPlacement)
		{
			return this.SecondsLeft(rewardedVideoPlacement) > 0;
		}

		public string GetTimeLeftString(string rewardedVideoPlacement)
		{
			if (!this.IsInCooldown(rewardedVideoPlacement))
			{
				return string.Empty;
			}
			int num = this.SecondsLeft(rewardedVideoPlacement);
			TimeSpan timeSpan = TimeSpan.FromSeconds((double)num);
			return string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
		}

		public void VideoWasCompleted(string rewardedVideoPlacement)
		{
			this.SetDateForLastVideoShown(rewardedVideoPlacement, DateTime.UtcNow);
		}

		private int SecondsLeft(string rewardedVideoPlacement)
		{
			RewardedVideoContextConfiguration rewardedVideoPlacementConfig = this.adConfigurationProvider.GetRewardedVideoPlacementConfig(rewardedVideoPlacement);
			if (rewardedVideoPlacementConfig != null)
			{
				int coolDownInSeconds = rewardedVideoPlacementConfig.CoolDownInSeconds;
				DateTime dateForLastVideoShown = this.GetDateForLastVideoShown(rewardedVideoPlacement);
				double totalSeconds = (DateTime.UtcNow - dateForLastVideoShown).TotalSeconds;
				double num = (double)coolDownInSeconds - totalSeconds;
				return (int)num;
			}
			return 0;
		}

		private DateTime GetDateForLastVideoShown(string rewardedVideoPlacement)
		{
			ILocalStorageString localStorageString = this.rewardedVideoStoreFactory.CreatePlayerPrefsSecuredString(rewardedVideoPlacement);
			string text = localStorageString.Load();
			if (text.Length > 0)
			{
				return DateTime.ParseExact(text, "yyyy-MM-dd HH:mm:ss", null);
			}
			return DateTime.MinValue;
		}

		private void SetDateForLastVideoShown(string rewardedVideoPlacement, DateTime value)
		{
			ILocalStorageString localStorageString = this.rewardedVideoStoreFactory.CreatePlayerPrefsSecuredString(rewardedVideoPlacement);
			string data = value.ToString("yyyy-MM-dd HH:mm:ss");
			localStorageString.Save(data);
		}

		private readonly IAdConfigurationProvider adConfigurationProvider;

		private readonly IRewardedVideoStoreFactory rewardedVideoStoreFactory;
	}
}
