using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using TactileModules.Ads.Analytics;
using TactileModules.Ads.RewardedVideo;

namespace TactileModules.Ads
{
	public class RewardedVideoPresenter : IRewardedVideoPresenter
	{
		public RewardedVideoPresenter(IMusicControlProvider musicController, IRewardedVideoCoolDowns rewardedVideoCoolDowns, IRewardedVideoAnalytics rewardedVideoAnalytics)
		{
			this.musicController = musicController;
			this.rewardedVideoCoolDowns = rewardedVideoCoolDowns;
			this.rewardedVideoAnalytics = rewardedVideoAnalytics;
			this.providers = new List<IRewardedVideoProvider>();
			this.requirements = new List<IRewardedVideoRequirement>();
		}

		public void RegisterRewardedVideoProvider(IRewardedVideoProvider provider)
		{
			if (!this.providers.Contains(provider))
			{
				this.providers.Add(provider);
				this.providers.Sort((IRewardedVideoProvider a, IRewardedVideoProvider b) => b.Priority - a.Priority);
				return;
			}
			throw new Exception(string.Format("Provider {0} already registered", provider));
		}

		public bool IsPlacementEnabled(AdGroupContext adGroupContext)
		{
			return this.rewardedVideoCoolDowns.IsPlacementEnabled(adGroupContext);
		}

		public bool IsInCooldown(AdGroupContext adGroupContext)
		{
			return this.rewardedVideoCoolDowns.IsInCooldown(adGroupContext);
		}

		public string GetTimeLeftString(AdGroupContext adGroupContext)
		{
			return this.rewardedVideoCoolDowns.GetTimeLeftString(adGroupContext);
		}

		public bool CanShowRewardedVideo(AdGroupContext adGroupContext)
		{
			if (!this.MeetsRequirements(adGroupContext))
			{
				return false;
			}
			if (this.IsInCooldown(adGroupContext))
			{
				return false;
			}
			foreach (IRewardedVideoProvider rewardedVideoProvider in this.providers)
			{
				if (rewardedVideoProvider.IsVideoAvailable)
				{
					return true;
				}
			}
			return false;
		}

		private bool MeetsRequirements(AdGroupContext adGroupContext)
		{
			foreach (IRewardedVideoRequirement rewardedVideoRequirement in this.requirements)
			{
				if (!rewardedVideoRequirement.MeetsRequirement(adGroupContext))
				{
					return false;
				}
			}
			return true;
		}

		public bool IsRequestingVideo()
		{
			foreach (IRewardedVideoProvider rewardedVideoProvider in this.providers)
			{
				if (rewardedVideoProvider.IsVideoAvailable)
				{
					return false;
				}
			}
			foreach (IRewardedVideoProvider rewardedVideoProvider2 in this.providers)
			{
				if (rewardedVideoProvider2.IsPreparingVideo)
				{
					return true;
				}
			}
			return false;
		}

		public void RequestVideo()
		{
			foreach (IRewardedVideoProvider rewardedVideoProvider in this.providers)
			{
				rewardedVideoProvider.RequestVideo();
			}
		}

		public void RegisterRequirement(IRewardedVideoRequirement rewardedVideoRequirement)
		{
			this.requirements.Add(rewardedVideoRequirement);
		}

		public bool IsPlayingVideo()
		{
			return this.isPlayingVideo;
		}

		public IEnumerator ShowRewardedVideo(RewardedVideoParameters data, Action<bool> videoCompleted)
		{
			using (List<IRewardedVideoProvider>.Enumerator enumerator = this.providers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IRewardedVideoProvider provider = enumerator.Current;
					if (provider.IsVideoAvailable)
					{
						yield return new Fiber.OnExit(delegate()
						{
							this.musicController.ResumeMusic();
							this.isPlayingVideo = false;
						});
						this.isPlayingVideo = true;
						this.musicController.PauseMusic();
						yield return provider.ShowVideo(data.AdGroupContext, delegate
						{
							this.rewardedVideoAnalytics.LogVideoImpression(data, provider.AdProviderDisplayName);
						}, delegate(bool completed)
						{
							videoCompleted(completed);
							if (completed)
							{
								this.rewardedVideoAnalytics.LogVideoCompleted(data, provider.AdProviderDisplayName);
								this.rewardedVideoCoolDowns.VideoWasCompleted(data.AdGroupContext);
							}
						});
						yield break;
					}
				}
			}
			videoCompleted(false);
			yield break;
		}

		public IEnumerator FetchAndShowRewardedVideo(RewardedVideoParameters data, Action<bool> videoCompleted, int timeoutSeconds)
		{
			if (!this.CanShowRewardedVideo(data.AdGroupContext))
			{
				this.RequestVideo();
			}
			DateTime timeoutTime = DateTime.UtcNow.AddSeconds((double)timeoutSeconds);
			while (!this.CanShowRewardedVideo(data.AdGroupContext) && DateTime.Now < timeoutTime)
			{
				yield return null;
			}
			if (this.CanShowRewardedVideo(data.AdGroupContext))
			{
				yield return this.ShowRewardedVideo(data, videoCompleted);
			}
			yield break;
		}

		private readonly IMusicControlProvider musicController;

		private readonly IRewardedVideoCoolDowns rewardedVideoCoolDowns;

		private readonly IRewardedVideoAnalytics rewardedVideoAnalytics;

		private readonly List<IRewardedVideoProvider> providers;

		private readonly List<IRewardedVideoRequirement> requirements;

		private bool isPlayingVideo;
	}
}
