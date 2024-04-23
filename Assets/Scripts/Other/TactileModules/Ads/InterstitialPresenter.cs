using System;
using System.Collections;
using System.Collections.Generic;
using TactileModules.RuntimeTools;
using TactileModules.TactilePrefs;

namespace TactileModules.Ads
{
	public class InterstitialPresenter : IInterstitialPresenter, IInterstitialPresenterDataProvider
	{
		public InterstitialPresenter(ILocalStorageObject<InterstitialProviderManagerData> storage, ITactileDateTime dateTimeGetter)
		{
			this.storage = storage;
			this.dateTimeGetter = dateTimeGetter;
			this.providers = new List<IInterstitialProvider>();
			if (!storage.Exists())
			{
				storage.Save(new InterstitialProviderManagerData());
			}
			this.storageData = storage.Load();
		}

		public void Register(IInterstitialProvider provider)
		{
			if (!this.providers.Contains(provider))
			{
				this.providers.Add(provider);
				this.providers.Sort((IInterstitialProvider a, IInterstitialProvider b) => b.Priority - a.Priority);
				return;
			}
			throw new Exception(string.Format("Provider {0} already registered", provider));
		}

		public void RequestInterstitial()
		{
			foreach (IInterstitialProvider interstitialProvider in this.providers)
			{
				interstitialProvider.RequestInterstitial();
			}
		}

		public bool IsInterstitialAvailable
		{
			get
			{
				foreach (IInterstitialProvider interstitialProvider in this.providers)
				{
					if (interstitialProvider.IsInterstitialAvailable)
					{
						return true;
					}
				}
				return false;
			}
		}

		public IEnumerator ShowInterstitial()
		{
			if (!this.InterstitialRequirementsAreMet())
			{
				yield break;
			}
			foreach (IInterstitialProvider provider in this.providers)
			{
				if (provider.IsInterstitialAvailable)
				{
					yield return provider.ShowInterstitial();
					this.storageData.LastInterstitialShown = this.dateTimeGetter.UtcNow;
					this.storageData.TotalInterstitialsShown++;
					this.storage.Save(this.storageData);
					yield break;
				}
			}
			yield break;
		}

		public void RegisterRequirement(IInterstitialRequirement interstitialRequirement)
		{
			this.interstitialRequirements.Add(interstitialRequirement);
		}

		public bool InterstitialRequirementsAreMet()
		{
			foreach (IInterstitialRequirement interstitialRequirement in this.interstitialRequirements)
			{
				if (!interstitialRequirement.RequirementIsMet(this.storageData))
				{
					return false;
				}
			}
			return true;
		}

		public IEnumerator FetchAndShowInterstitial(int timeoutSeconds)
		{
			if (!this.IsInterstitialAvailable)
			{
				this.RequestInterstitial();
			}
			DateTime timeoutTime = this.dateTimeGetter.UtcNow.AddSeconds((double)timeoutSeconds);
			while (!this.IsInterstitialAvailable && DateTime.UtcNow < timeoutTime)
			{
				yield return null;
			}
			yield return this.ShowInterstitial();
			yield break;
		}

		int IInterstitialPresenterDataProvider.TotalInterstitialsShown
		{
			get
			{
				return this.storageData.TotalInterstitialsShown;
			}
		}

		private readonly ILocalStorageObject<InterstitialProviderManagerData> storage;

		private readonly ITactileDateTime dateTimeGetter;

		private readonly List<IInterstitialProvider> providers;

		private readonly InterstitialProviderManagerData storageData;

		private readonly List<IInterstitialRequirement> interstitialRequirements = new List<IInterstitialRequirement>();
	}
}
