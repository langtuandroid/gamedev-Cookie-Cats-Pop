using System;
using System.Collections;
using JetBrains.Annotations;
using TactileModules.AgeInfo.UI;
using UnityEngine;

namespace TactileModules.AgeInfo
{
	public class AgeInfoManager : SingleInstance
	{
		public AgeInfoManager([NotNull] UIViewManager uiViewManager, [NotNull] TactileAnalytics tactileAnalytics, [NotNull] AgeInfoManager.IDataProvider provider)
		{
			this.provider = provider;
			this.uiViewManager = uiViewManager;
			this.tactileAnalytics = tactileAnalytics;
		}

		private bool UserHasConsented
		{
			get
			{
				return TactilePlayerPrefs.GetBool("age_info_collected", false);
			}
			set
			{
				TactilePlayerPrefs.SetBool("age_info_collected", value);
			}
		}

		private bool ShouldShowPopup()
		{
			return this.provider.Config.IsActive && !this.UserHasConsented && this.DoesCountryRequireConsent();
		}

		public IEnumerator TryShowPopup()
		{
			if (!this.ShouldShowPopup())
			{
				yield break;
			}
			bool requireAgeInput = this.DoesCountryRequireAgeInput();
			UIViewManager.UIViewState vs = null;
			if (requireAgeInput)
			{
				vs = this.uiViewManager.ShowView<AgeInfoView>(new object[]
				{
					this.provider
				});
			}
			else
			{
				vs = this.uiViewManager.ShowView<UserConsentView>(new object[]
				{
					this.provider
				});
			}
			Camera ageInfoViewCamera = this.uiViewManager.FindCameraFromObjectLayer(vs.View.gameObject.layer).cachedCamera;
			ageInfoViewCamera.clearFlags = CameraClearFlags.Color;
			yield return vs.WaitForClose();
			int age = (int)vs.ClosingResult;
			if (requireAgeInput)
			{
				int ageThreshold = this.GetAgeThreshold();
				if (age < ageThreshold)
				{
					yield break;
				}
			}
			this.UserHasConsented = true;
			this.tactileAnalytics.LogEvent(new AgeInfoAnalytics.UserConsentEvent(((UserConsentView)vs.View).GetViewName(), age), -1.0, null);
			yield break;
		}

		public bool DoesCountryRequireConsent()
		{
			return this.GetCountryConsentRequirement() != null;
		}

		public bool DoesCountryRequireAgeInput()
		{
			CountryConsentRequirement countryConsentRequirement = this.GetCountryConsentRequirement();
			return countryConsentRequirement != null && countryConsentRequirement.RequireAge;
		}

		public int GetAgeThreshold()
		{
			CountryConsentRequirement countryConsentRequirement = this.GetCountryConsentRequirement();
			if (!countryConsentRequirement.RequireAge)
			{
				return 0;
			}
			return this.GetCountryConsentRequirement().AgeThreshold;
		}

		private CountryConsentRequirement GetCountryConsentRequirement()
		{
			CountryCodeContainer countryFilter = this.provider.Config.CountryFilter;
			string localeCountryCode = SystemInfoHelper.GetLocaleCountryCode();
			foreach (CountryConsentRequirement countryConsentRequirement in countryFilter.CountryConsentRequirements)
			{
				if (countryConsentRequirement.CountryCode == localeCountryCode)
				{
					return countryConsentRequirement;
				}
			}
			return null;
		}

		private const string AgeCollectedPrefKey = "age_info_collected";

		private readonly AgeInfoManager.IDataProvider provider;

		private readonly UIViewManager uiViewManager;

		private readonly TactileAnalytics tactileAnalytics;

		public interface IDataProvider
		{
			AgeInfoConfig Config { get; }

			void ShowInvalidAgeView(int ageThreshold);
		}
	}
}
