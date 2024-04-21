using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxSdkUnityEditor : MaxSdkBase
{
	static MaxSdkUnityEditor()
	{
		MaxSdkBase.InitCallbacks();
	}

	public static MaxVariableServiceUnityEditor VariableService
	{
		get
		{
			return MaxVariableServiceUnityEditor.Instance;
		}
	}

	public static void SetSdkKey(string sdkKey)
	{
		MaxSdkUnityEditor._hasSdkKey = true;
	}

	public static void InitializeSdk()
	{
		MaxSdkUnityEditor._ensureHaveSdkKey();
		MaxSdkUnityEditor._isInitialized = true;
		MaxSdkUnityEditor._hasSdkKey = true;
		MaxSdkUnityEditor.ExecuteWithDelay(delegate
		{
			MaxSdkUnityEditor._isInitialized = true;
		});
	}

	public static bool IsInitialized()
	{
		return MaxSdkUnityEditor._isInitialized;
	}

	public static void SetUserId(string userId)
	{
	}

	public static void ShowMediationDebugger()
	{
	}

	public static MaxSdkBase.ConsentDialogState GetConsentDialogState()
	{
		return MaxSdkBase.ConsentDialogState.Unknown;
	}

	public static void SetHasUserConsent(bool hasUserConsent)
	{
		MaxSdkUnityEditor._hasUserConsent = hasUserConsent;
	}

	public static bool HasUserConsent()
	{
		return MaxSdkUnityEditor._hasUserConsent;
	}

	public static void SetIsAgeRestrictedUser(bool isAgeRestrictedUser)
	{
		MaxSdkUnityEditor._isAgeRestrictedUser = isAgeRestrictedUser;
	}

	public static bool IsAgeRestrictedUser()
	{
		return MaxSdkUnityEditor._isAgeRestrictedUser;
	}

	public static void CreateBanner(string adUnitIdentifier, MaxSdkBase.BannerPosition bannerPosition)
	{
		MaxSdkBase.ValidateAdUnitIdentifier(adUnitIdentifier, "create banner");
		MaxSdkUnityEditor.RequestAdUnit(adUnitIdentifier);
	}

	public static void SetBannerPlacement(string adUnitIdentifier, string placement)
	{
	}

	public static void ShowBanner(string adUnitIdentifier)
	{
		MaxSdkBase.ValidateAdUnitIdentifier(adUnitIdentifier, "show banner");
		if (!MaxSdkUnityEditor.IsAdUnitRequested(adUnitIdentifier))
		{
		}
	}

	public static void DestroyBanner(string adUnitIdentifier)
	{
		MaxSdkBase.ValidateAdUnitIdentifier(adUnitIdentifier, "destroy banner");
	}

	public static void HideBanner(string adUnitIdentifier)
	{
		MaxSdkBase.ValidateAdUnitIdentifier(adUnitIdentifier, "hide banner");
	}

	public static void SetBannerBackgroundColor(string adUnitIdentifier, Color color)
	{
		MaxSdkBase.ValidateAdUnitIdentifier(adUnitIdentifier, "set background color");
	}

	public static void LoadInterstitial(string adUnitIdentifier)
	{
		MaxSdkBase.ValidateAdUnitIdentifier(adUnitIdentifier, "load interstitial");
	}

	public static bool IsInterstitialReady(string adUnitIdentifier)
	{
		MaxSdkBase.ValidateAdUnitIdentifier(adUnitIdentifier, "check interstitial loaded");
		return MaxSdkUnityEditor.IsAdUnitRequested(adUnitIdentifier);
	}

	public static void ShowInterstitial(string adUnitIdentifier)
	{
		MaxSdkUnityEditor.ShowInterstitial(adUnitIdentifier, null);
	}

	public static void ShowInterstitial(string adUnitIdentifier, string placement)
	{
		MaxSdkBase.ValidateAdUnitIdentifier(adUnitIdentifier, "show interstitial");
		if (!MaxSdkUnityEditor.IsAdUnitRequested(adUnitIdentifier))
		{
			return;
		}
	}

	public static void LoadRewardedAd(string adUnitIdentifier)
	{
		MaxSdkBase.ValidateAdUnitIdentifier(adUnitIdentifier, "load rewarded ad");
		MaxSdkUnityEditor.RequestAdUnit(adUnitIdentifier);
	}

	public static bool IsRewardedAdReady(string adUnitIdentifier)
	{
		MaxSdkBase.ValidateAdUnitIdentifier(adUnitIdentifier, "check rewarded ad loaded");
		return MaxSdkUnityEditor.IsAdUnitRequested(adUnitIdentifier);
	}

	public static void ShowRewardedAd(string adUnitIdentifier)
	{
		MaxSdkUnityEditor.ShowRewardedAd(adUnitIdentifier, null);
	}

	public static void ShowRewardedAd(string adUnitIdentifier, string placement)
	{
		if (!MaxSdkUnityEditor.IsAdUnitRequested(adUnitIdentifier))
		{
			return;
		}
		MaxSdkBase.ValidateAdUnitIdentifier(adUnitIdentifier, "show rewarded ad");
	}

	public static void TrackEvent(string name, IDictionary<string, string> parameters = null)
	{
	}

	private static void RequestAdUnit(string adUnitId)
	{
		MaxSdkUnityEditor._ensureInitialized();
		MaxSdkUnityEditor._requestedAdUnits.Add(adUnitId);
	}

	private static bool IsAdUnitRequested(string adUnitId)
	{
		MaxSdkUnityEditor._ensureInitialized();
		return MaxSdkUnityEditor._requestedAdUnits.Contains(adUnitId);
	}

	private static void _ensureHaveSdkKey()
	{
		if (MaxSdkUnityEditor._hasSdkKey)
		{
			return;
		}
	}

	private static void _ensureInitialized()
	{
		MaxSdkUnityEditor._ensureHaveSdkKey();
		if (MaxSdkUnityEditor._isInitialized)
		{
			return;
		}
	}

	private static void ExecuteWithDelay(Action action)
	{
		MaxSdkCallbacks.Instance.StartCoroutine(MaxSdkUnityEditor.ExecuteAction(action));
	}

	private static IEnumerator ExecuteAction(Action action)
	{
		yield return null;
		action();
		yield break;
	}

	private static bool _isInitialized;

	private static bool _hasSdkKey;

	private static bool _hasUserConsent = true;

	private static bool _isAgeRestrictedUser = false;

	private static readonly HashSet<string> _requestedAdUnits = new HashSet<string>();
}
