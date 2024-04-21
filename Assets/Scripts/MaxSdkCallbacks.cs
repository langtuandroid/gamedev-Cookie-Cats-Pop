using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class MaxSdkCallbacks : MonoBehaviour
{
	public static MaxSdkCallbacks Instance { get; private set; }

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<MaxSdkBase.SdkConfiguration> OnSdkInitializedEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action OnVariablesUpdatedEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string> OnBannerAdLoadedEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string, int> OnBannerAdLoadFailedEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string> OnBannerAdClickedEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string> OnBannerAdExpandedEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string> OnBannerAdCollapsedEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string> OnInterstitialLoadedEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string, int> OnInterstitialLoadFailedEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string> OnInterstitialHiddenEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string> OnInterstitialDisplayedEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string, int> OnInterstitialAdFailedToDisplayEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string> OnInterstitialClickedEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string> OnRewardedAdLoadedEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string, int> OnRewardedAdLoadFailedEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string> OnRewardedAdDisplayedEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string> OnRewardedAdHiddenEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string> OnRewardedAdClickedEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string, int> OnRewardedAdFailedToDisplayEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string, MaxSdkBase.Reward> OnRewardedAdReceivedRewardEvent;

	private void Awake()
	{
		if (MaxSdkCallbacks.Instance == null)
		{
			MaxSdkCallbacks.Instance = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
	}

	public void ForwardEvent(string eventPropsStr)
	{
		IDictionary<string, string> dictionary = MaxSdkUtils.PropsStringToDict(eventPropsStr);
		string a = dictionary["name"];
		if (a == "OnSdkInitializedEvent")
		{
			string value = dictionary["consentDialogState"];
			MaxSdkBase.SdkConfiguration sdkConfiguration = new MaxSdkBase.SdkConfiguration();
			if ("1".Equals(value))
			{
				sdkConfiguration.ConsentDialogState = MaxSdkBase.ConsentDialogState.Applies;
			}
			else if ("2".Equals(value))
			{
				sdkConfiguration.ConsentDialogState = MaxSdkBase.ConsentDialogState.DoesNotApply;
			}
			else
			{
				sdkConfiguration.ConsentDialogState = MaxSdkBase.ConsentDialogState.Unknown;
			}
			MaxSdkCallbacks.InvokeEvent<MaxSdkBase.SdkConfiguration>(MaxSdkCallbacks.OnSdkInitializedEvent, sdkConfiguration);
		}
		else if (a == "OnVariablesUpdatedEvent")
		{
			MaxSdkCallbacks.InvokeEvent(MaxSdkCallbacks.OnVariablesUpdatedEvent);
		}
		else
		{
			string text = dictionary["adUnitId"];
			if (a == "OnBannerAdLoadedEvent")
			{
				MaxSdkCallbacks.InvokeEvent<string>(MaxSdkCallbacks.OnBannerAdLoadedEvent, text);
			}
			else if (a == "OnBannerAdLoadFailedEvent")
			{
				int param = 0;
				int.TryParse(dictionary["errorCode"], out param);
				MaxSdkCallbacks.InvokeEvent<string, int>(MaxSdkCallbacks.OnBannerAdLoadFailedEvent, text, param);
			}
			else if (a == "OnBannerAdClickedEvent")
			{
				MaxSdkCallbacks.InvokeEvent<string>(MaxSdkCallbacks.OnBannerAdClickedEvent, text);
			}
			else if (a == "OnBannerAdExpandedEvent")
			{
				MaxSdkCallbacks.InvokeEvent<string>(MaxSdkCallbacks.OnBannerAdExpandedEvent, text);
			}
			else if (a == "OnBannerAdCollapsedEvent")
			{
				MaxSdkCallbacks.InvokeEvent<string>(MaxSdkCallbacks.OnBannerAdCollapsedEvent, text);
			}
			else if (a == "OnInterstitialLoadedEvent")
			{
				MaxSdkCallbacks.InvokeEvent<string>(MaxSdkCallbacks.OnInterstitialLoadedEvent, text);
			}
			else if (a == "OnInterstitialLoadFailedEvent")
			{
				int param2 = 0;
				int.TryParse(dictionary["errorCode"], out param2);
				MaxSdkCallbacks.InvokeEvent<string, int>(MaxSdkCallbacks.OnInterstitialLoadFailedEvent, text, param2);
			}
			else if (a == "OnInterstitialHiddenEvent")
			{
				MaxSdkCallbacks.InvokeEvent<string>(MaxSdkCallbacks.OnInterstitialHiddenEvent, text);
			}
			else if (a == "OnInterstitialDisplayedEvent")
			{
				MaxSdkCallbacks.InvokeEvent<string>(MaxSdkCallbacks.OnInterstitialDisplayedEvent, text);
			}
			else if (a == "OnInterstitialAdFailedToDisplayEvent")
			{
				int param3 = 0;
				int.TryParse(dictionary["errorCode"], out param3);
				MaxSdkCallbacks.InvokeEvent<string, int>(MaxSdkCallbacks.OnInterstitialAdFailedToDisplayEvent, text, param3);
			}
			else if (a == "OnInterstitialClickedEvent")
			{
				MaxSdkCallbacks.InvokeEvent<string>(MaxSdkCallbacks.OnInterstitialClickedEvent, text);
			}
			else if (a == "OnRewardedAdLoadedEvent")
			{
				MaxSdkCallbacks.InvokeEvent<string>(MaxSdkCallbacks.OnRewardedAdLoadedEvent, text);
			}
			else if (a == "OnRewardedAdLoadFailedEvent")
			{
				int param4 = 0;
				int.TryParse(dictionary["errorCode"], out param4);
				MaxSdkCallbacks.InvokeEvent<string, int>(MaxSdkCallbacks.OnRewardedAdLoadFailedEvent, text, param4);
			}
			else if (a == "OnRewardedAdDisplayedEvent")
			{
				MaxSdkCallbacks.InvokeEvent<string>(MaxSdkCallbacks.OnRewardedAdDisplayedEvent, text);
			}
			else if (a == "OnRewardedAdHiddenEvent")
			{
				MaxSdkCallbacks.InvokeEvent<string>(MaxSdkCallbacks.OnRewardedAdHiddenEvent, text);
			}
			else if (a == "OnRewardedAdClickedEvent")
			{
				MaxSdkCallbacks.InvokeEvent<string>(MaxSdkCallbacks.OnRewardedAdClickedEvent, text);
			}
			else if (a == "OnRewardedAdFailedToDisplayEvent")
			{
				int param5 = 0;
				int.TryParse(dictionary["errorCode"], out param5);
				MaxSdkCallbacks.InvokeEvent<string, int>(MaxSdkCallbacks.OnRewardedAdFailedToDisplayEvent, text, param5);
			}
			else if (a == "OnRewardedAdReceivedRewardEvent")
			{
				MaxSdkBase.Reward param6 = new MaxSdkBase.Reward
				{
					Label = dictionary["rewardLabel"]
				};
				int.TryParse(dictionary["rewardAmount"], out param6.Amount);
				MaxSdkCallbacks.InvokeEvent<string, MaxSdkBase.Reward>(MaxSdkCallbacks.OnRewardedAdReceivedRewardEvent, text, param6);
			}
		}
	}

	private static void InvokeEvent(Action evt)
	{
		if (!MaxSdkCallbacks.CanInvokeEvent(evt))
		{
			return;
		}
		evt();
	}

	private static void InvokeEvent<T>(Action<T> evt, T param)
	{
		if (!MaxSdkCallbacks.CanInvokeEvent(evt))
		{
			return;
		}
		evt(param);
	}

	private static void InvokeEvent<T1, T2>(Action<T1, T2> evt, T1 param1, T2 param2)
	{
		if (!MaxSdkCallbacks.CanInvokeEvent(evt))
		{
			return;
		}
		evt(param1, param2);
	}

	private static bool CanInvokeEvent(Delegate evt)
	{
		if (evt == null)
		{
			return false;
		}
		if (evt.GetInvocationList().Length > 5)
		{
		}
		return true;
	}
}
