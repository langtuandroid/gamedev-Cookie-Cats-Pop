using System;
using System.Collections;
using System.Diagnostics;


public abstract class AnalyticsBase : SingleInstance<AnalyticsBase>
{
	protected AnalyticsBase(CloudClientBase cloudClient, AnalyticsBase.AdjustSettings adjust, string tactileAnalyticsAppId, TactileAnalytics.Config tactileAnalyticsConfig)
	{
		this.cloudClient = cloudClient;
		this.adjustSettings = adjust;
		TactileAnalytics.CreateInstance(tactileAnalyticsAppId, SystemInfoHelper.BundleShortVersion, int.Parse(SystemInfoHelper.BundleVersion), SystemInfoHelper.DeviceID, tactileAnalyticsConfig);
		
		FiberCtrl.Pool.Run(this.TriggerGameServerRegistered(), false);
	}

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action AdjustAttributionChanged;

	

	public static AnalyticsBase Instance
	{
		get
		{
			return SingleInstance<AnalyticsBase>.instance;
		}
	}

   

    private IEnumerator TriggerGameServerRegistered()
	{
		while (this.cloudClient.LastReceivedServerTimeUnixEpocUTC == 0)
		{
			yield return null;
		}
		if (this.GetOneTimeEventSent("GameServerRegistered"))
		{
			yield break;
		}
		this.SetOneTimeEventSent("GameServerRegistered");
		yield break;
	}

	public bool GetOneTimeEventSent(string eventName)
	{
		return TactilePlayerPrefs.GetBool("OneTimeEventSent_" + eventName, false);
	}

	public void SetOneTimeEventSent(string eventName)
	{
		TactilePlayerPrefs.SetBool("OneTimeEventSent_" + eventName, true);
	}

	protected AnalyticsBase.AdjustSettings adjustSettings;

	private const string ADJUST_ATTRIBUTION_KEY = "AnalyticsBaseAdjustAttribution";


	private CloudClientBase cloudClient;

	public class AdjustAquisitionChannel
	{
		[JsonSerializable("ID", null)]
		public string ID { get; set; }

		[JsonSerializable("Network", null)]
		public string Network { get; set; }

		[JsonSerializable("Campaign", null)]
		public string Campaign { get; set; }

		[JsonSerializable("AdGroup", null)]
		public string AdGroup { get; set; }

		[JsonSerializable("Creative", null)]
		public string Creative { get; set; }

		[JsonSerializable("Label", null)]
		public string Label { get; set; }
	}

	protected struct AdjustSettings
	{
		public AdjustSettings(string appToken, string appSecretId, string appSecret1, string appSecret2, string appSecret3, string appSecret4, string userRegisteredToken, string userCheatedToken, string iapToken, string userIsPayingToken)
		{
			this.appToken = appToken;
			this.appSecretId = appSecretId;
			this.appSecret1 = appSecret1;
			this.appSecret2 = appSecret2;
			this.appSecret3 = appSecret3;
			this.appSecret4 = appSecret4;
			this.userRegisteredToken = userRegisteredToken;
			this.userCheatedToken = userCheatedToken;
			this.iapToken = iapToken;
			this.userIsPayingToken = userIsPayingToken;
		}

		public readonly string appToken;

		public readonly string appSecretId;

		public readonly string appSecret1;

		public readonly string appSecret2;

		public readonly string appSecret3;

		public readonly string appSecret4;

		public readonly string userRegisteredToken;

		public readonly string userCheatedToken;

		public readonly string iapToken;

		public readonly string userIsPayingToken;
	}
}
