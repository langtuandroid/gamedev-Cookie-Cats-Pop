using System;
using System.Collections;
using System.Collections.Generic;
using ConfigSchema;
using Fibers;
using Tactile;

public class MapPopupManager
{
	public MapPopupManager.PopupResult popupResult { get; private set; }

	public MapPopupManager.IPopupManagerProvider popupManagerProvider { get; private set; }

	public bool IsPopupFlowRunning { get; private set; }

	public static MapPopupManager Instance
	{
		get
		{
			if (MapPopupManager.instance == null)
			{
				throw new Exception("PopupManager not intialized yet! Please call CreateInstance before accessing");
			}
			return MapPopupManager.instance;
		}
	}

	public static MapPopupManager CreateInstance(GameSessionManager gameSessionManager, Func<MapPopupManager.PopupLocation, List<MapPopupManager.PopupFlow>, List<MapPopupManager.PopupFlow>> popupSorter = null)
	{
		if (MapPopupManager.instance == null)
		{
			MapPopupManager.instance = new MapPopupManager
			{
				popupSorter = popupSorter
			};
			gameSessionManager.NewSessionStarted += delegate()
			{
				MapPopupManager.instance.isNewSession = true;
			};
			return MapPopupManager.instance;
		}
		throw new Exception("Instance of PopupManager already created!");
	}

	private static List<MapPopupManager.PopupFlow> DefaultSorter(List<MapPopupManager.PopupFlow> popupDatas)
	{
		popupDatas.Sort(delegate(MapPopupManager.PopupFlow a, MapPopupManager.PopupFlow b)
		{
			MapPopupManager.PopupConfig.PopupConfigData configData = a.MapPopup.GetConfigData();
			MapPopupManager.PopupConfig.PopupConfigData configData2 = b.MapPopup.GetConfigData();
			if (configData.PopupType == MapPopupManager.PopupType.BlockFlow ^ configData2.PopupType == MapPopupManager.PopupType.BlockFlow)
			{
				return (configData.PopupType != MapPopupManager.PopupType.BlockFlow) ? -1 : 1;
			}
			if (configData.PopupType == MapPopupManager.PopupType.BlockOtherPopups ^ configData2.PopupType == MapPopupManager.PopupType.BlockOtherPopups)
			{
				return (configData.PopupType != MapPopupManager.PopupType.BlockOtherPopups) ? -1 : 1;
			}
			if (configData.IsUrgent ^ configData2.IsUrgent)
			{
				return (!configData.IsUrgent) ? 1 : -1;
			}
			return configData2.Priority - configData.Priority;
		});
		return popupDatas;
	}

	public void RegisterPopupObject(MapPopupManager.IMapPopup popup)
	{
		if (this.registeredTypes.Contains(popup.GetType()))
		{
			throw new Exception("Popup is already registered: " + popup);
		}
		this.popupObjects.Add(popup);
		this.registeredTypes.Add(popup.GetType());
	}

	public IEnumerator Run(MapPopupManager.IPopupManagerProvider provider, bool willShowLevelStartView, int levelUnlockedIndex)
	{
		this.IsPopupFlowRunning = true;
		this.ResetRunStats();
		this.popupManagerProvider = provider;
		this.popupResult = new MapPopupManager.PopupResult();
		yield return new Fiber.OnExit(delegate()
		{
			this.popupResult.numPopupsShowed = this.numberOfPopupsShown;
			this.popupResult.startViewBlocked = this.startViewBlocked;
			this.popupResult.popupsBlocked = this.popupsBlocked;
			this.popupResult.flowBroken = this.flowBroken;
			this.ResetRunStats();
		});
		yield return this.RunInternal(provider, levelUnlockedIndex, willShowLevelStartView);
		this.IsPopupFlowRunning = false;
		yield break;
	}

	public IEnumerator RunCustomLocation(MapPopupManager.IPopupManagerProvider provider, bool willShowLevelStartView, int levelUnlockedIndex, MapPopupManager.PopupLocation location)
	{
		this.ResetRunStats();
		yield return this.RunLocation(provider, location, levelUnlockedIndex, willShowLevelStartView);
		yield break;
	}

	private IEnumerator RunInternal(MapPopupManager.IPopupManagerProvider provider, int levelUnlockedIndex, bool willShowLevelStartView)
	{
		this.RefreshPopupFlows(levelUnlockedIndex, this.isNewSession);
		if (this.isNewSession)
		{
			yield return this.RunLocation(provider, MapPopupManager.PopupLocation.SessionStart, levelUnlockedIndex, willShowLevelStartView);
			this.ConsumeNewSession();
		}
		if (this.flowBroken)
		{
			yield break;
		}
		if (!this.popupsBlocked)
		{
			yield return this.RunLocation(provider, MapPopupManager.PopupLocation.PreProgress, levelUnlockedIndex, willShowLevelStartView);
		}
		if (this.flowBroken)
		{
			yield break;
		}
		if (levelUnlockedIndex > -1)
		{
			yield return provider.AnimateProgress();
		}
		if (!this.popupsBlocked)
		{
			yield return this.RunLocation(provider, MapPopupManager.PopupLocation.PostProgress, levelUnlockedIndex, willShowLevelStartView);
		}
		yield break;
	}

	private IEnumerator RunLocation(MapPopupManager.IPopupManagerProvider provider, MapPopupManager.PopupLocation location, int levelUnlockedIndex, bool willShowLevelStartView)
	{
		List<MapPopupManager.PopupFlow> locationDataList = this.GetPopupFlowsForLocation(location, willShowLevelStartView);
		List<MapPopupManager.PopupFlow> sortedData = (this.popupSorter != null) ? this.popupSorter(location, locationDataList) : MapPopupManager.DefaultSorter(locationDataList);
		List<MapPopupManager.PopupFlow> clampedDataList = this.GetClampedPopupDataListForLocation(location, sortedData, provider, willShowLevelStartView, levelUnlockedIndex);
		foreach (MapPopupManager.PopupFlow data in clampedDataList)
		{
			if (data.GetSilentActions().Count != 0 || data.GetPopups().Count != 0)
			{
				bool didShow = false;
				yield return this.ShowPopup(data, delegate(bool b)
				{
					didShow = b;
				});
				if (didShow)
				{
					this.startViewBlocked |= (data.MapPopup.GetConfigData().PopupType == MapPopupManager.PopupType.BlockStartView);
					this.flowBroken |= (data.MapPopup.GetConfigData().PopupType == MapPopupManager.PopupType.BlockFlow);
					this.popupsBlocked |= (data.MapPopup.GetConfigData().PopupType == MapPopupManager.PopupType.BlockOtherPopups);
					if (this.popupsBlocked || this.flowBroken)
					{
						yield break;
					}
				}
			}
		}
		yield break;
	}

	private IEnumerator ShowPopup(MapPopupManager.PopupFlow flow, Action<bool> didShowCallback)
	{
		MapPopupManager.PopupConfig.PopupConfigData data = flow.MapPopup.GetConfigData();
		if (data == null || !data.Enabled)
		{
			didShowCallback(false);
			yield break;
		}
		bool showedAny = false;
		foreach (Action action in flow.GetSilentActions())
		{
			action();
		}
		foreach (IEnumerator enumerator in flow.GetPopups())
		{
			yield return enumerator;
			this.numberOfPopupsShown++;
			showedAny = true;
		}
		didShowCallback(showedAny);
		yield break;
	}

	private int GetAmountOfPopupsFromIMapPopups(List<MapPopupManager.PopupFlow> mapPopups)
	{
		int num = 0;
		foreach (MapPopupManager.PopupFlow popupFlow in mapPopups)
		{
			num += popupFlow.GetPopups().Count;
		}
		return num;
	}

	private List<MapPopupManager.PopupFlow> GetClampedPopupDataListForLocation(MapPopupManager.PopupLocation popupLocation, List<MapPopupManager.PopupFlow> locationDataList, MapPopupManager.IPopupManagerProvider provider, bool willShowLevelStartView, int levelUnlockedIndex)
	{
		int num = 0;
		List<MapPopupManager.PopupFlow> popupFlowsForLocation = this.GetPopupFlowsForLocation(MapPopupManager.PopupLocation.PreProgress, willShowLevelStartView);
		List<MapPopupManager.PopupFlow> popupFlowsForLocation2 = this.GetPopupFlowsForLocation(MapPopupManager.PopupLocation.PostProgress, willShowLevelStartView);
		num += this.GetAmountOfPopupsFromIMapPopups(popupFlowsForLocation);
		num += this.GetAmountOfPopupsFromIMapPopups(popupFlowsForLocation2);
		int num2 = 0;
		List<MapPopupManager.PopupFlow> list;
		if (popupLocation == MapPopupManager.PopupLocation.SessionStart)
		{
			int maxPopupSequence = provider.PopupConfig.MaxPopupSequence;
			list = new List<MapPopupManager.PopupFlow>();
			foreach (MapPopupManager.PopupFlow popupFlow in locationDataList)
			{
				if (popupFlow.MapPopup.GetConfigData().IsUrgent && popupFlow.NeedsToRun)
				{
					list.Add(popupFlow);
					num2 += popupFlow.NumberOfPopups;
				}
			}
			foreach (MapPopupManager.PopupFlow popupFlow2 in locationDataList)
			{
				if (!popupFlow2.MapPopup.GetConfigData().IsUrgent)
				{
					if (num2 + num < maxPopupSequence)
					{
						if ((popupFlow2.MapPopup.GetConfigData().PopupType == MapPopupManager.PopupType.BlockFlow || popupFlow2.MapPopup.GetConfigData().PopupType == MapPopupManager.PopupType.BlockOtherPopups) && !popupFlow2.MapPopup.GetConfigData().IsUrgent)
						{
							int num3 = 0;
							num3 += this.GetUrgentPopupDatasForLocation(MapPopupManager.PopupLocation.PreProgress, willShowLevelStartView, levelUnlockedIndex).Count;
							if (num3 + this.GetUrgentPopupDatasForLocation(MapPopupManager.PopupLocation.PostProgress, willShowLevelStartView, levelUnlockedIndex).Count == 0 && popupFlow2.NeedsToRun)
							{
								list.Add(popupFlow2);
								num2 += popupFlow2.NumberOfPopups;
								if (num2 > maxPopupSequence)
								{
									popupFlow2.RemovePopups(num2 - maxPopupSequence);
								}
							}
						}
						else if (!list.Contains(popupFlow2) && popupFlow2.NeedsToRun)
						{
							list.Add(popupFlow2);
							num2 += popupFlow2.NumberOfPopups;
							if (num2 > maxPopupSequence)
							{
								popupFlow2.RemovePopups(num2 - maxPopupSequence);
							}
						}
					}
				}
			}
		}
		else
		{
			list = new List<MapPopupManager.PopupFlow>(locationDataList);
		}
		return list;
	}

	private List<MapPopupManager.PopupFlow> GetUrgentPopupDatasForLocation(MapPopupManager.PopupLocation popupLocation, bool willShowLevelStartView, int unlockedLevelIndex)
	{
		List<MapPopupManager.PopupFlow> list = new List<MapPopupManager.PopupFlow>();
		foreach (MapPopupManager.PopupFlow popupFlow in this.GetPopupFlowsForLocation(popupLocation, willShowLevelStartView))
		{
			if (popupFlow.MapPopup.GetConfigData().IsUrgent && popupFlow.NeedsToRun)
			{
				list.Add(popupFlow);
			}
		}
		return list;
	}

	private void RefreshPopupFlows(int unlockedLevelIndex, bool isNewSession)
	{
		this.allCurrentPopupFlows = new List<MapPopupManager.PopupFlow>();
		foreach (MapPopupManager.IMapPopup mapPopup in this.popupObjects)
		{
			if (isNewSession || mapPopup.GetConfigData().PopupLocation != MapPopupManager.PopupLocation.SessionStart)
			{
				MapPopupManager.PopupFlow popupFlow = new MapPopupManager.PopupFlow(mapPopup);
				mapPopup.TryShowPopup(unlockedLevelIndex, popupFlow);
				this.allCurrentPopupFlows.Add(popupFlow);
			}
		}
	}

	private List<MapPopupManager.PopupFlow> GetPopupFlowsForLocation(MapPopupManager.PopupLocation popupLocation, bool willShowLevelStartView)
	{
		List<MapPopupManager.PopupFlow> list = new List<MapPopupManager.PopupFlow>();
		foreach (MapPopupManager.PopupFlow popupFlow in this.allCurrentPopupFlows)
		{
			MapPopupManager.PopupConfig.PopupConfigData configData = popupFlow.MapPopup.GetConfigData();
			if (configData.PopupLocation == popupLocation && (!willShowLevelStartView || configData.IsUrgent))
			{
				list.Add(popupFlow);
			}
		}
		return list;
	}

	public void ConsumeNewSession()
	{
		this.isNewSession = false;
	}

	private void ResetRunStats()
	{
		this.flowBroken = false;
		this.popupsBlocked = false;
		this.startViewBlocked = false;
		this.numberOfPopupsShown = 0;
		this.popupManagerProvider = null;
		this.allCurrentPopupFlows = new List<MapPopupManager.PopupFlow>();
	}

	private readonly List<MapPopupManager.IMapPopup> popupObjects = new List<MapPopupManager.IMapPopup>();

	private readonly List<Type> registeredTypes = new List<Type>();

	private Func<MapPopupManager.PopupLocation, List<MapPopupManager.PopupFlow>, List<MapPopupManager.PopupFlow>> popupSorter;

	private List<MapPopupManager.PopupFlow> allCurrentPopupFlows = new List<MapPopupManager.PopupFlow>();

	private bool flowBroken;

	private bool popupsBlocked;

	private bool startViewBlocked;

	private bool isNewSession;

	private int numberOfPopupsShown;

	private static MapPopupManager instance;

	public enum PopupLocation
	{
		SessionStart,
		PreProgress,
		PostProgress,
		Custom
	}

	public enum PopupType
	{
		Normal,
		BlockStartView,
		BlockOtherPopups,
		BlockFlow
	}

	[ConfigProvider("PopupConfig")]
	public class PopupConfig
	{
		public PopupConfig()
		{
			this.PopupConfigDatas = new List<MapPopupManager.PopupConfig.PopupConfigData>();
		}

		[JsonSerializable("maxPopupSequence", null)]
		public int MaxPopupSequence { get; set; }

		[JsonSerializable("timePriorityIncreaseStep", null)]
		[Obsolete("No longer relevant", true)]
		public int TimePriorityIncreaseStep { get; set; }

		[ArrayFormat(ArrayFormatAttribute.ArrayFormat.table)]
		[JsonSerializable("popupConfigDatas", typeof(MapPopupManager.PopupConfig.PopupConfigData))]
		public List<MapPopupManager.PopupConfig.PopupConfigData> PopupConfigDatas { get; set; }

		public class PopupConfigData
		{
			[JsonSerializable("popupClassName", null)]
			public string PopupClassName { get; set; }

			[JsonSerializable("enabled", null)]
			public bool Enabled { get; set; }

			[JsonSerializable("isUrgent", null)]
			public bool IsUrgent { get; set; }

			[JsonSerializable("priority", null)]
			public int Priority { get; set; }

			[JsonSerializable("location", null)]
			public MapPopupManager.PopupLocation PopupLocation { get; set; }

			[JsonSerializable("type", null)]
			public MapPopupManager.PopupType PopupType { get; set; }
		}
	}

	public class PopupResult
	{
		public bool shouldShowLevelStartView
		{
			get
			{
				return !this.startViewBlocked && !this.popupsBlocked && !this.flowBroken;
			}
		}

		public bool AnyPopupsShowed
		{
			get
			{
				return this.numPopupsShowed > 0;
			}
		}

		public int numPopupsShowed;

		public bool startViewBlocked;

		public bool popupsBlocked;

		public bool flowBroken;
	}

	public class PopupFlow
	{
		public PopupFlow(MapPopupManager.IMapPopup mapPopup)
		{
			this.MapPopup = mapPopup;
		}

		public MapPopupManager.IMapPopup MapPopup { get; private set; }

		public bool NeedsToRun
		{
			get
			{
				return this.popupFlows.Count > 0 || this.silentActions.Count > 0;
			}
		}

		public bool AnythingToShow
		{
			get
			{
				return this.popupFlows.Count > 0;
			}
		}

		public int NumberOfPopups
		{
			get
			{
				return this.popupFlows.Count;
			}
		}

		public void AddPopup(IEnumerator flow)
		{
			this.popupFlows.Add(flow);
		}

		public void AddSilentAction(Action a)
		{
			this.silentActions.Add(a);
		}

		public List<IEnumerator> GetPopups()
		{
			return this.popupFlows;
		}

		public List<Action> GetSilentActions()
		{
			return this.silentActions;
		}

		public void RemovePopups(int number)
		{
			if (!this.MapPopup.GetConfigData().IsUrgent)
			{
				for (int i = number - 1; i >= 0; i--)
				{
					this.popupFlows.RemoveAt(i);
				}
			}
		}

		private readonly List<IEnumerator> popupFlows = new List<IEnumerator>();

		private readonly List<Action> silentActions = new List<Action>();
	}

	public interface IMapPopup
	{
		void TryShowPopup(int unlockedLevelIndex, MapPopupManager.PopupFlow popupFlow);
	}

	public interface IPopupManagerProvider
	{
		IEnumerator AnimateProgress();

		MapPopupManager.PopupConfig PopupConfig { get; }
	}
}
