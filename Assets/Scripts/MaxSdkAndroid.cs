using System;
using System.Collections.Generic;
using UnityEngine;

public class MaxSdkAndroid : MaxSdkBase
{
	static MaxSdkAndroid()
	{
		MaxSdkBase.InitCallbacks();
	}

	public static MaxVariableServiceAndroid VariableService
	{
		get
		{
			return MaxVariableServiceAndroid.Instance;
		}
	}

	public static void SetSdkKey(string sdkKey)
	{
		MaxSdkAndroid.MaxUnityPluginClass.CallStatic("setSdkKey", new object[]
		{
			sdkKey
		});
	}

	public static void InitializeSdk()
	{
		MaxSdkAndroid.MaxUnityPluginClass.CallStatic("initializeSdk", new object[0]);
	}

	public static bool IsInitialized()
	{
		return MaxSdkAndroid.MaxUnityPluginClass.CallStatic<bool>("isInitialized", new object[0]);
	}

	public static void SetUserId(string userId)
	{
		MaxSdkAndroid.MaxUnityPluginClass.CallStatic("setUserId", new object[]
		{
			userId
		});
	}

	public static void ShowMediationDebugger()
	{
		MaxSdkAndroid.MaxUnityPluginClass.CallStatic("showMediationDebugger", new object[0]);
	}

	public static MaxSdkBase.ConsentDialogState GetConsentDialogState()
	{
		if (!MaxSdkAndroid.IsInitialized())
		{
		}
		return (MaxSdkBase.ConsentDialogState)MaxSdkAndroid.MaxUnityPluginClass.CallStatic<int>("getConsentDialogState", new object[0]);
	}

	public static void SetHasUserConsent(bool hasUserConsent)
	{
		MaxSdkAndroid.MaxUnityPluginClass.CallStatic("setHasUserConsent", new object[]
		{
			hasUserConsent
		});
	}

	public static bool HasUserConsent()
	{
		return MaxSdkAndroid.MaxUnityPluginClass.CallStatic<bool>("hasUserConsent", new object[0]);
	}

	public static void SetIsAgeRestrictedUser(bool isAgeRestrictedUser)
	{
		MaxSdkAndroid.MaxUnityPluginClass.CallStatic("setIsAgeRestrictedUser", new object[]
		{
			isAgeRestrictedUser
		});
	}

	public static bool IsAgeRestrictedUser()
	{
		return MaxSdkAndroid.MaxUnityPluginClass.CallStatic<bool>("isAgeRestrictedUser", new object[0]);
	}

	public static void CreateBanner(string adUnitIdentifier, MaxSdkBase.BannerPosition bannerPosition)
	{
		MaxSdkBase.ValidateAdUnitIdentifier(adUnitIdentifier, "create banner");
		MaxSdkAndroid.MaxUnityPluginClass.CallStatic("createBanner", new object[]
		{
			adUnitIdentifier,
			bannerPosition.ToString()
		});
	}

	public static void SetBannerPlacement(string adUnitIdentifier, string placement)
	{
		MaxSdkBase.ValidateAdUnitIdentifier(adUnitIdentifier, "set banner placement");
		MaxSdkAndroid.MaxUnityPluginClass.CallStatic("setBannerPlacement", new object[]
		{
			adUnitIdentifier,
			placement
		});
	}

	public static void ShowBanner(string adUnitIdentifier)
	{
		MaxSdkBase.ValidateAdUnitIdentifier(adUnitIdentifier, "show banner");
		MaxSdkAndroid.MaxUnityPluginClass.CallStatic("showBanner", new object[]
		{
			adUnitIdentifier
		});
	}

	public static void DestroyBanner(string adUnitIdentifier)
	{
		MaxSdkBase.ValidateAdUnitIdentifier(adUnitIdentifier, "destroy banner");
		MaxSdkAndroid.MaxUnityPluginClass.CallStatic("destroyBanner", new object[]
		{
			adUnitIdentifier
		});
	}

	public static void HideBanner(string adUnitIdentifier)
	{
		MaxSdkBase.ValidateAdUnitIdentifier(adUnitIdentifier, "hide banner");
		MaxSdkAndroid.MaxUnityPluginClass.CallStatic("hideBanner", new object[]
		{
			adUnitIdentifier
		});
	}

	public static void SetBannerBackgroundColor(string adUnitIdentifier, Color color)
	{
		MaxSdkBase.ValidateAdUnitIdentifier(adUnitIdentifier, "set background color");
		MaxSdkAndroid.MaxUnityPluginClass.CallStatic("setBannerBackgroundColor", new object[]
		{
			adUnitIdentifier,
			MaxSdkUtils.ParseColor(color)
		});
	}

	public static void LoadInterstitial(string adUnitIdentifier)
	{
		MaxSdkBase.ValidateAdUnitIdentifier(adUnitIdentifier, "load interstitial");
		MaxSdkAndroid.MaxUnityPluginClass.CallStatic("loadInterstitial", new object[]
		{
			adUnitIdentifier
		});
	}

	public static bool IsInterstitialReady(string adUnitIdentifier)
	{
		MaxSdkBase.ValidateAdUnitIdentifier(adUnitIdentifier, "check interstitial loaded");
		return MaxSdkAndroid.MaxUnityPluginClass.CallStatic<bool>("isInterstitialReady", new object[]
		{
			adUnitIdentifier
		});
	}

	public static void ShowInterstitial(string adUnitIdentifier)
	{
		MaxSdkAndroid.ShowInterstitial(adUnitIdentifier, null);
	}

	public static void ShowInterstitial(string adUnitIdentifier, string placement)
	{
		MaxSdkBase.ValidateAdUnitIdentifier(adUnitIdentifier, "show interstitial");
		if (MaxSdkAndroid.IsInterstitialReady(adUnitIdentifier))
		{
			MaxSdkAndroid.MaxUnityPluginClass.CallStatic("showInterstitial", new object[]
			{
				adUnitIdentifier,
				placement
			});
		}
	}

	public static void LoadRewardedAd(string adUnitIdentifier)
	{
		MaxSdkBase.ValidateAdUnitIdentifier(adUnitIdentifier, "load rewarded ad");
		MaxSdkAndroid.MaxUnityPluginClass.CallStatic("loadRewardedAd", new object[]
		{
			adUnitIdentifier
		});
	}

	public static bool IsRewardedAdReady(string adUnitIdentifier)
	{
		MaxSdkBase.ValidateAdUnitIdentifier(adUnitIdentifier, "check rewarded ad loaded");
		return MaxSdkAndroid.MaxUnityPluginClass.CallStatic<bool>("isRewardedAdReady", new object[]
		{
			adUnitIdentifier
		});
	}

	public static void ShowRewardedAd(string adUnitIdentifier)
	{
		MaxSdkAndroid.ShowRewardedAd(adUnitIdentifier, null);
	}

	public static void ShowRewardedAd(string adUnitIdentifier, string placement)
	{
		MaxSdkBase.ValidateAdUnitIdentifier(adUnitIdentifier, "show rewarded ad");
		if (MaxSdkAndroid.IsRewardedAdReady(adUnitIdentifier))
		{
			MaxSdkAndroid.MaxUnityPluginClass.CallStatic("showRewardedAd", new object[]
			{
				adUnitIdentifier,
				placement
			});
		}
	}

	public static void TrackEvent(string name, IDictionary<string, string> parameters = null)
	{
		MaxSdkAndroid.MaxUnityPluginClass.CallStatic("trackEvent", new object[]
		{
			name,
			MaxSdkUtils.DictToPropsString(parameters)
		});
	}

	private static readonly AndroidJavaClass MaxUnityPluginClass = new AndroidJavaClass("com.applovin.mediation.unity.MaxUnityPlugin");
}
