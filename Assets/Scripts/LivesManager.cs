using System;
using System.Collections;
using System.Diagnostics;
using Tactile;
using TactileModules.Foundation;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGames.Lives;
using UnityEngine;

public class LivesManager : MapPopupManager.IMapPopup, ILivesManager
{
	public LivesManager()
	{
		LivesManager _0024this = this;
		this.Load();
		MapPopupManager.Instance.RegisterPopupObject(this);
		LivesConfig config = ConfigurationManager.Get<LivesConfig>();
		this.service = new RegeneratingItemService(this.TimeStampManager, "Life", config.LifeRegenerationMaxCount, config.LifeRegenerationTime);
		ShopManager.Instance.ShopItemBought += delegate(ShopItem shopItem)
		{
			if (shopItem.Type == "ShopItemExtraLives")
			{
				_0024this.FillAllLives();
			}
			if (shopItem.CustomTag.Contains("unlimitedLives"))
			{
				_0024this.EnableUnlimitedLives(-1);
			}
		};
		InventoryManager.Instance.InventoryChanged += this.HandleInventoryChanged;
		TimeStampManager.Instance.TimeDone += this.HandleTimeDone;
		this.lastPopUpTime -= (float)this.ConfigurationManager.GetConfig<LivesConfig>().InfiniteLivesPopupInterval;
	}

	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action UnlimitedLivesChanged;
	public static LivesManager Instance { get; private set; }

	private ConfigurationManager ConfigurationManager
	{
		get
		{
			return ManagerRepository.Get<ConfigurationManager>();
		}
	}

	private TimeStampManager TimeStampManager
	{
		get
		{
			return ManagerRepository.Get<TimeStampManager>();
		}
	}

	public static LivesManager CreateInstance()
	{
		LivesManager.Instance = new LivesManager();
		return LivesManager.Instance;
	}

	private void FillAllLives()
	{
		LivesConfig livesConfig = ConfigurationManager.Get<LivesConfig>();
		int num = true ? livesConfig.NotLoggedInMaxlives : livesConfig.LoggedInMaxlives;
		if (InventoryManager.Instance.GetAmount("Life") < num)
		{
			InventoryManager.Instance.SetAmount("Life", num, null);
		}
		int lifeRegenerationMaxCount = this.ConfigurationManager.GetConfig<TournamentConfig>().LifeRegenerationMaxCount;
		if (TournamentManager.Instance.Lives < lifeRegenerationMaxCount)
		{
			InventoryManager.Instance.SetAmount("TournamentLife", lifeRegenerationMaxCount, null);
		}
	}

	public void TryShowPopup(int levelUnlocked, MapPopupManager.PopupFlow popupFlow)
	{
		if (this.ShouldShowPopup())
		{
			popupFlow.AddPopup(this.ShowPopup());
		}
	}

	private IEnumerator ShowPopup()
	{
		this.lastPopUpTime = Time.realtimeSinceStartup;
		UIViewManager.UIViewStateGeneric<UnlimitedLivesView> vs = UIViewManager.Instance.ShowView<UnlimitedLivesView>(new object[0]);
		yield return vs.WaitForClose();
		yield break;
	}

	public bool ShouldShowPopup()
	{
		return !this.HasUnlimitedLives() && MainProgressionManager.Instance.GetFarthestUnlockedLevelHumanNumber() >= this.ConfigurationManager.GetConfig<LivesConfig>().LevelRequiredForInfiniteLivesPopup && Time.realtimeSinceStartup >= this.lastPopUpTime + (float)this.ConfigurationManager.GetConfig<LivesConfig>().InfiniteLivesPopupInterval;
	}

	public void UseLife()
	{
		if (this.HasUnlimitedLives())
		{
			return;
		}
		InventoryManager.Instance.Consume("Life", 1, null);
	}

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<LivesChangedInfo> LivesChanged;

	public int LifeRegenerationMaxCount { get; private set; }

	public bool HasUnlimitedLives()
	{
		return this.GetTimeLeftForInfiniteLives() > 0;
	}

	public bool IsOutOfLives()
	{
		return InventoryManager.Instance.Lives <= 0;
	}

	public void UseLifeIfNotUnlimited(string analyticsTag)
	{
		if (this.HasUnlimitedLives())
		{
			return;
		}
		InventoryManager.Instance.Consume("Life", 1, analyticsTag);
	}

	public int GetRegenerationTimeLeft()
	{
		return this.service.GetSecondsLeftForRegeneration();
	}

	public int GetTimeLeftForInfiniteLives()
	{
		if (TimeStampManager.Instance.TimeStampExist("InfiniteLivesTimeStamp"))
		{
			return TimeStampManager.Instance.GetTimeLeftInSeconds("InfiniteLivesTimeStamp");
		}
		return 0;
	}

	private void HandleInventoryChanged(InventoryManager.ItemChangeInfo info)
	{
		if (info.Item != "UnlimitedLives")
		{
			return;
		}
		if (InventoryManager.Instance.GetAmount("UnlimitedLives") == 0)
		{
			return;
		}
		this.EnableUnlimitedLives(info.ChangeByAmount);
		InventoryManager.Instance.Consume("UnlimitedLives", info.ChangeByAmount, info.ProductOrCause);
	}

	private void EnableUnlimitedLives(int seconds = -1)
	{
		InventoryManager.Instance.SetAmount("Life", Mathf.Max(this.ConfigurationManager.GetConfig<LivesConfig>().LifeRegenerationMaxCount, InventoryManager.Instance.Lives), "unlimited lives");
		int lifeRegenerationMaxCount = this.ConfigurationManager.GetConfig<TournamentConfig>().LifeRegenerationMaxCount;
		if (TournamentManager.Instance.Lives < lifeRegenerationMaxCount)
		{
			InventoryManager.Instance.SetAmount("TournamentLife", lifeRegenerationMaxCount, null);
		}
		if (seconds == -1)
		{
			seconds = this.ConfigurationManager.GetConfig<LivesConfig>().InfiniteLivesDurationInSeconds;
		}
		if (TimeStampManager.Instance.TimeStampExist("InfiniteLivesTimeStamp"))
		{
			int num = Math.Max(0, TimeStampManager.Instance.GetTimeLeftInSeconds("InfiniteLivesTimeStamp"));
			TimeStampManager.Instance.RemoveTimeStampIfItExist("InfiniteLivesTimeStamp");
			TimeStampManager.Instance.CreateTimeStamp("InfiniteLivesTimeStamp", num + seconds);
		}
		else
		{
			TimeStampManager.Instance.CreateTimeStamp("InfiniteLivesTimeStamp", seconds);
		}
		this.OnUnlimitedLivesChanged();
	}

	private void OnUnlimitedLivesChanged()
	{
		this.UnlimitedLivesChanged();
	}

	private void HandleTimeDone(string timer)
	{
		if (timer == "InfiniteLivesTimeStamp")
		{
			this.OnUnlimitedLivesChanged();
		}
	}

	private void Load()
	{
		string securedString = TactilePlayerPrefs.GetSecuredString("livesManagerHasConnectedFacebookPrev", string.Empty);
		if (securedString.Length > 0)
		{
			this.state = JsonSerializer.HashtableToObject<LivesManager.LivesPersistable>(securedString.hashtableFromJson());
		}
		else
		{
			this.state = new LivesManager.LivesPersistable();
		}
	}

	private void Save()
	{
		if (this.state != null)
		{
			TactilePlayerPrefs.SetSecuredString("livesManagerHasConnectedFacebookPrev", JsonSerializer.ObjectToHashtable(this.state).toJson());
		}
		else
		{
			TactilePlayerPrefs.SetSecuredString("livesManagerHasConnectedFacebookPrev", string.Empty);
		}
	}

	private RegeneratingItemService service;

	private float lastPopUpTime;

	private LivesManager.LivesPersistable state;

	private const string PREFS_LIVES_MANAGER_HAS_CONNECTED_FACEBOOK_PREVIOUSLY = "livesManagerHasConnectedFacebookPrev";

	private const string PREFS_LIVES_MANAGER_MARKED_TIME = "livesManagerMarkedTime";

	private const string INFINITE_LIVES_TIME_STAMP = "InfiniteLivesTimeStamp";

	public class LivesPersistable
	{
		[JsonSerializable("hcp", null)]
		public bool HasConnectedPreviously { get; set; }
	}
}
