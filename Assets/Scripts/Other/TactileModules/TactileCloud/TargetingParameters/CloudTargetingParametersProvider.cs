using System;
using System.Collections;
using TactileModules.InstallTimeTracking;
using UnityEngine;

namespace TactileModules.TactileCloud.TargetingParameters
{
	public class CloudTargetingParametersProvider : ITargetingParametersProvider
	{
		public CloudTargetingParametersProvider(ICloudClientState cloudClientState, IInstallTime installTime)
		{
			this.cloudClientState = cloudClientState;
			this.installTime = installTime;
		}

		public Hashtable GetAdditionalTargetingParameters()
		{
			Hashtable hashtable = new Hashtable
			{
				{
					"deviceLanguage",
					Application.systemLanguage.ToString()
				},
				{
					"secondsSinceInstall",
					this.installTime.GetSecondsSinceFirstInstall()
				},
				{
					"localeCountryCode",
					SystemInfoHelper.GetLocaleCountryCode()
				},
				{
					"currentLocale",
					SystemInfoHelper.GetCurrentLocale()
				},
				{
					"deviceId",
					SystemInfoHelper.DeviceID
				}
			};
			CloudDevice cachedDevice = this.cloudClientState.CachedDevice;
			if (cachedDevice != null)
			{
				hashtable.Add("countryCode", cachedDevice.CountryCode);
			}
			return hashtable;
		}

		private readonly ICloudClientState cloudClientState;

		private readonly IInstallTime installTime;
	}
}
