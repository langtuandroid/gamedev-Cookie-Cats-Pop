using System;
using System.Collections.Generic;
using Shared.OneLifeChallenge;
using Tactile;
using TactileModules.Foundation;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.PuzzleGames.TreasureHunt;
using TactileModules.SagaCore;
using UnityEngine;

public class Analytics : AnalyticsBase
{
	public Analytics(CloudClient cloudClient, ConfigurationManager configManager) : base(cloudClient, new AnalyticsBase.AdjustSettings(AdjustEventConstants.ADJUST_IO_APP_TOKEN, Constants.ADJUST_IO_SECRET_ID, Constants.ADJUST_IO_SECRET_PART_1, Constants.ADJUST_IO_SECRET_PART_2, Constants.ADJUST_IO_SECRET_PART_3, Constants.ADJUST_IO_SECRET_PART_4, AdjustEventConstants.ADJUST_IO_USER_REGISTERED_EVENT_TOKEN, AdjustEventConstants.ADJUST_IO_USER_CHEATED_EVENT_TOKEN, AdjustEventConstants.ADJUST_IO_IAP_EVENT_TOKEN, AdjustEventConstants.ADJUST_IO_USER_IS_PAYING_EVENT_TOKEN), Constants.TACTILE_ANALYTICS_APP_ID, configManager.GetConfig<TactileAnalytics.Config>())
	{
		this.missioncontext = new Analytics.MissionContext();
		GameEventManager.Instance.OnGameEvent += this.HandleGameEvent;
	}

	public Analytics.MissionContext missioncontext { get; private set; }

	public LevelSession LevelSession { get; private set; }

	private ConfigurationManager ConfigurationManager
	{
		get
		{
			return ManagerRepository.Get<ConfigurationManager>();
		}
	}

	private LevelDatabaseCollection LevelDatabaseCollection
	{
		get
		{
			return ManagerRepository.Get<LevelDatabaseCollection>();
		}
	}

	public new static Analytics Instance => AnalyticsBase.Instance as Analytics;

	private void HandleGameEvent(GameEvent e)
	{
		if (e.type == 1000)
		{
			this.LogTournamentCompleted(e.value);
		}
		else if (e.type == 26)
		{
			this.LogPowerComboUsed(e.context as LevelSession);
		}
	}

	private void Session_Ended(LevelSession session)
	{
		this.LevelSession = null;
		session.Ended -= this.Session_Ended;
	}

	public void AddSubContext(string context)
	{
		this.subContextStack.Add(context);
	}

	public void RemoveSubContext(string context)
	{
		this.subContextStack.Remove(context);
	}

	public void AddMissionScheduledEventContexts(List<string> contexts)
	{
		this.missionScheduledEventContextStack.AddRange(contexts);
	}

	public void RemoveMissionScheduledEventContexts(List<string> contexts)
	{
		for (int i = 0; i < contexts.Count; i++)
		{
			this.missionScheduledEventContextStack.Remove(contexts[i]);
		}
	}

	public void HookIntoGameCore(IFlowStack flowStack)
	{
		flowStack.Changed += this.HandleFlowStackChanged;
	}

	private void HandleFlowStackChanged(IFlow newFlow, IFlow oldFlow)
	{
		if (newFlow is MainMapFlow)
		{
			this.SetMasterContext(Analytics.MasterContext.Main);
		}
		else if (newFlow is OneLifeChallengeMapFlow)
		{
			this.SetMasterContext(Analytics.MasterContext.OneLifeChallenge);
		}
		else if (newFlow is GateFlow)
		{
			this.SetMasterContext(Analytics.MasterContext.Gate);
		}
		else if (newFlow is TreasureHuntMapFlow)
		{
			this.SetMasterContext(Analytics.MasterContext.TreasureHunt);
		}
	}

	public void SetMasterContext(Analytics.MasterContext context)
	{
		this.masterContext = context;
	}

	public void UpdateMissioncontext(LevelProxy levelProxy)
	{
		this.missioncontext.UpdateContext(levelProxy);
	}

	public void InvalidateMissionContext()
	{
		this.missioncontext.InvalidateContext();
	}

	private string GetSubContextStackAsString()
	{
		string text = string.Empty;
		foreach (string str in this.subContextStack)
		{
			if (!string.IsNullOrEmpty(text))
			{
				text += ":";
			}
			text += str;
		}
		return text;
	}

	private string GetMissionScheduledEventStackAsString()
	{
		string text = string.Empty;
		foreach (string str in this.missionScheduledEventContextStack)
		{
			if (!string.IsNullOrEmpty(text))
			{
				text += ":";
			}
			text += str;
		}
		return text;
	}

	public void LogReviewViewInteraction(bool didReview, bool askAgain)
	{
		TactileAnalytics.Instance.LogEvent(new Analytics.UserReviewInteractionEvent(didReview, askAgain), -1.0, null);
	}

	public void LogPowerComboUsed(LevelSession session)
	{
	}

	public void LogSeagullSeen(bool wasHit)
	{
		TactileAnalytics.Instance.LogEvent(new Analytics.SeagullSeenEvent(wasHit), -1.0, null);
	}

	public void LogTournamentCompleted(int placement)
	{
		
	}

	public void LogThemeHuntItemCollected(string huntId, int huntTotalItems, int huntItemId)
	{
		TactileAnalytics.Instance.LogEvent(new Analytics.ThemeHuntItemCollectedEvent(huntId, huntTotalItems, huntItemId), -1.0, null);
	}

	public void LogFeatureUpgradeException(Exception e, int fromVersion, int toVersion)
	{
		Analytics.ErrorReportingFeatureUpgradeExceptionEvent eventObject = new Analytics.ErrorReportingFeatureUpgradeExceptionEvent(e, fromVersion, toVersion);
		TactileAnalytics.Instance.LogEvent(eventObject, -1.0, null);
	}

	public void LogFeatureUpgradeVersionError(int fromVersion, int toVersion, int resultVersion, int targetVersion)
	{
		Analytics.ErrorReportingFeatureUpgradeVersionEvent eventObject = new Analytics.ErrorReportingFeatureUpgradeVersionEvent(fromVersion, toVersion, resultVersion, targetVersion);
		TactileAnalytics.Instance.LogEvent(eventObject, -1.0, null);
	}

	public Analytics.MasterContext masterContext = Analytics.MasterContext.Main;

	private List<string> subContextStack = new List<string>();

	private List<string> missionScheduledEventContextStack = new List<string>();

	public const string EVENT_TIMESTAMP_FORMAT = "yyyy-MM-dd HH:mm:ss.fff";

	[TactileAnalytics.EventAttribute("SeagullSeen", true)]
	protected class SeagullSeenEvent : BasicEvent
	{
		public SeagullSeenEvent(bool pWasHit)
		{
			this.WasHit = pWasHit;
		}

		private TactileAnalytics.RequiredParam<bool> WasHit { get; set; }
	}

	
	[TactileAnalytics.EventAttribute("themeHuntItemCollected", true)]
	protected class ThemeHuntItemCollectedEvent
	{
		public ThemeHuntItemCollectedEvent(string huntId, int huntTotalItems, int huntItemId)
		{
			this.HuntId = huntId;
			this.HuntTotalItems = huntTotalItems;
			this.HuntItemId = huntItemId;
		}

		private TactileAnalytics.RequiredParam<string> HuntId { get; set; }

		private TactileAnalytics.RequiredParam<int> HuntTotalItems { get; set; }

		private TactileAnalytics.RequiredParam<int> HuntItemId { get; set; }
	}

	[TactileAnalytics.EventAttribute("errorReportingFeatureUpgradeException", false)]
	protected class ErrorReportingFeatureUpgradeExceptionEvent
	{
		public ErrorReportingFeatureUpgradeExceptionEvent(Exception e, int fromVersion, int toVersion)
		{
			this.FromVersion = fromVersion;
			this.ToVersion = toVersion;
			this.Exception = e.ToString();
			this.Stacktrace = e.StackTrace;
		}

		private TactileAnalytics.RequiredParam<string> Exception { get; set; }

		private TactileAnalytics.RequiredParam<string> Stacktrace { get; set; }

		private TactileAnalytics.RequiredParam<int> FromVersion { get; set; }

		private TactileAnalytics.RequiredParam<int> ToVersion { get; set; }
	}

	[TactileAnalytics.EventAttribute("errorReportingFeatureUpgradeVersionEvent", false)]
	protected class ErrorReportingFeatureUpgradeVersionEvent
	{
		public ErrorReportingFeatureUpgradeVersionEvent(int fromVersion, int toVersion, int resultVersion, int targetVersion)
		{
			this.FromVersion = fromVersion;
			this.ToVersion = toVersion;
			this.TargetVersion = targetVersion;
			this.ResultVersion = resultVersion;
		}

		private TactileAnalytics.RequiredParam<int> TargetVersion { get; set; }

		private TactileAnalytics.RequiredParam<int> FromVersion { get; set; }

		private TactileAnalytics.RequiredParam<int> ToVersion { get; set; }

		private TactileAnalytics.RequiredParam<int> ResultVersion { get; set; }
	}

	[TactileAnalytics.EventAttribute("userReviewInteraction", true)]
	protected class UserReviewInteractionEvent : BasicEvent
	{
		public UserReviewInteractionEvent(bool didReview, bool askAgain)
		{
			this.DidReview = didReview;
			this.AskAgain = askAgain;
		}

		private TactileAnalytics.RequiredParam<bool> DidReview { get; set; }

		private TactileAnalytics.RequiredParam<bool> AskAgain { get; set; }
	}

	public class MasterContext
	{
		public MasterContext(string s)
		{
			this.value = s;
		}

		public readonly string value;

		public static readonly Analytics.MasterContext Main = new Analytics.MasterContext("main");

		public static readonly Analytics.MasterContext DailyQuest = new Analytics.MasterContext("daily");

		public static readonly Analytics.MasterContext Gate = new Analytics.MasterContext("gate");

		public static readonly Analytics.MasterContext Tournament = new Analytics.MasterContext("tournament");

		public static readonly Analytics.MasterContext OneLifeChallenge = new Analytics.MasterContext("onelifechallenge");

		public static readonly Analytics.MasterContext TreasureHunt = new Analytics.MasterContext("treasurehunt");
	}

	public class MissionContext
	{
		public bool IsTutorial { get; private set; }

		public bool IsHard { get; private set; }

		public string LevelContext { get; private set; }

		public int LevelId { get; private set; }

		public string MissionID { get; private set; }

		public string MissionName { get; private set; }

		public string MissionType { get; private set; }

		public string GateId { get; private set; }

		public void UpdateContext(LevelProxy levelProxy)
		{
			LevelAsset levelAsset = levelProxy.LevelAsset as LevelAsset;
			ILevelCollection levelCollection = levelProxy.LevelCollection;
			string gateId = string.Empty;
			int num;
			if (levelCollection is GateAsset)
			{
				int currentGateIndex = GateManager.Instance.GetCurrentGateIndex();
				gateId = (currentGateIndex + 1).ToString("D3");
				num = currentGateIndex * 3 + levelProxy.HumanNumber;
			}
			else
			{
				num = levelProxy.HumanNumber;
			}
			string text = "normal";
			string missionID = string.Empty;
			if (levelCollection is GateAsset && !(levelProxy.LevelCollection is TreasureHuntCollectionAsset))
			{
				missionID = string.Concat(new object[]
				{
					text,
					GateManager.Instance.GetCurrentGateIndex(),
					",",
					GateManager.Instance.CurrentGateKeys
				});
			}
			else
			{
				missionID = text + num;
			}
			string missionName = "unknown";
			if (!string.IsNullOrEmpty(levelProxy.LevelAsset.name))
			{
				missionName = levelProxy.LevelAsset.name.Substring(0, Mathf.Min(levelProxy.LevelAsset.name.Length, 70));
			}
			this.IsTutorial = levelAsset.IsTutorial;
			this.IsHard = (levelProxy.LevelDifficulty == LevelDifficulty.Hard);
			this.LevelContext = levelProxy.AnalyticsDescriptors[0];
			this.LevelId = num;
			this.MissionID = missionID;
			this.MissionName = missionName;
			this.MissionType = text;
			this.GateId = gateId;
			this.IsValid = true;
		}

		public void InvalidateContext()
		{
			this.IsValid = false;
			this.IsTutorial = false;
			this.IsHard = false;
			this.LevelContext = "Invalid";
			this.LevelId = -1;
			this.MissionID = "Invalid";
			this.MissionType = "Invalid";
			this.GateId = string.Empty;
		}

		public bool IsValid;
	}
}
