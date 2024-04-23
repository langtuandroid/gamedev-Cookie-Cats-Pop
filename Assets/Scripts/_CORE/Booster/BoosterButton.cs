using System;
using CookieCatsPop.UI;
using Tactile;
using TactileModules.Foundation;
using TactileModules.Inventory;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.Validation;
using UnityEngine;

public class BoosterButton : AnimatedButton
{
	private LevelDatabaseCollection LevelDatabaseCollection
	{
		get
		{
			return ManagerRepository.Get<LevelDatabaseCollection>();
		}
	}

	public string BoosterId
	{
		get
		{
			return this.boosterId;
		}
		set
		{
			this.boosterId = value;
			this.ReflectState();
		}
	}

	public bool Locked
	{
		get
		{
			return this.locked && !base.Disabled;
		}
		set
		{
			this.locked = (value && !base.Disabled);
			this.ReflectState();
		}
	}

	public bool Selected
	{
		get
		{
			return this.selected;
		}
		set
		{
			this.selected = value;
			this.ReflectState();
		}
	}

	public bool Free
	{
		get
		{
			return this.free;
		}
		set
		{
			this.free = value;
			this.ReflectState();
		}
	}

	public bool Unlimited
	{
		get
		{
			return this.unlimited;
		}
		set
		{
			this.unlimited = value;
			this.ReflectState();
		}
	}

	private void Start()
	{
		if (this.unlimitedPivot != null)
		{
			UISafeTimer uisafeTimer = new UISafeTimer(base.gameObject, new Action(this.UpdateUnlimitedTimerLabel), 1f);
			uisafeTimer.Run();
		}
	}

	private void UpdateUnlimitedTimerLabel()
	{
		if (this.Unlimited)
		{
			int timeLeftInSeconds = ManagerRepository.Get<InventorySystem>().UnlimitedItems.GetTimeLeftInSeconds(this.BoosterId);
			string text = L.FormatSecondsAsColumnSeparated(timeLeftInSeconds, "00:00:00", TimeFormatOptions.None);
			this.unlimitedTimerLabel.text = text;
		}
	}

	public void OnEnable()
	{
		if (!this.forcedAmounts)
		{
			this.EnableInventoryChangeListener();
			this.ReflectState();
		}
	}

	public void OnDestroy()
	{
		this.DisableInventoryChangeListener();
	}

	public void EnableInventoryChangeListener()
	{
		if (!this.isListeningToInventory)
		{
			InventoryManager.Instance.InventoryChanged += this.HandleInventoryChanged;
			this.isListeningToInventory = true;
		}
	}

	public void OnDisable()
	{
		this.DisableInventoryChangeListener();
	}

	public void DisableInventoryChangeListener()
	{
		if (this.isListeningToInventory)
		{
			InventoryManager.Instance.InventoryChanged -= this.HandleInventoryChanged;
			this.isListeningToInventory = false;
		}
	}

	public void EnableForcedAmounts()
	{
		if (!this.forcedAmounts)
		{
			this.DisableInventoryChangeListener();
			this.forcedAmounts = true;
		}
	}

	public void DisableForcedAmounts()
	{
		if (this.forcedAmounts)
		{
			this.EnableInventoryChangeListener();
			this.forcedAmounts = false;
		}
	}

	protected override void ReflectState()
	{
		base.ReflectState();
		if (!Application.isPlaying)
		{
			return;
		}
		this.disabledPivot.SetActive(base.Disabled);
		if (this.lockedPivot != null)
		{
			this.lockedPivot.SetActive(this.Locked);
			this.enabledPivot.SetActive(!base.Disabled && !this.Locked);
		}
		else
		{
			this.enabledPivot.SetActive(!base.Disabled);
		}
		this.freePivot.SetActive(!base.Disabled && this.Free);
		this.badgePivot.SetActive(!base.Disabled && !this.Free);
		bool flag = !base.Disabled && this.Selected;
		this.selectedPivot.SetActive(flag);
		if (this.checkMark != null)
		{
			this.checkMark.SetActive(flag && !this.unlimited);
		}
		if (this.unlimitedCheckMark != null)
		{
			this.unlimitedCheckMark.SetActive(flag && this.unlimited);
		}
		if (this.unlimitedPivot != null)
		{
			this.unlimitedPivot.SetActive(!base.Disabled && this.Unlimited);
		}
		if (this.enabledPivot.activeSelf)
		{
			if (!string.IsNullOrEmpty(this.BoosterId))
			{
				BoosterMetaData metaData = InventoryManager.Instance.GetMetaData<BoosterMetaData>(this.BoosterId);
				if (metaData != null)
				{
					this.icon.SpriteName = metaData.IconSpriteName;
					if (this.lockedIcon != null)
					{
						this.lockedIcon.SpriteName = metaData.IconSpriteName;
					}
				}
			}
		}
		else if (this.disabledPivot.activeSelf && !string.IsNullOrEmpty(this.BoosterId) && this.unlockLevel != null)
		{
			BoosterMetaData metaData2 = InventoryManager.Instance.GetMetaData<BoosterMetaData>(this.BoosterId);
			if (metaData2 != null)
			{
				MainLevelDatabase levelDatabase = this.LevelDatabaseCollection.GetLevelDatabase<MainLevelDatabase>("Main");
				int humanNumber = levelDatabase.GetHumanNumber(levelDatabase.GetLevel(metaData2.UnlockAtLevelIndex));
				this.unlockLevel.text = humanNumber.ToString();
			}
		}
		if (this.badgePivot.activeSelf)
		{
			int amount = InventoryManager.Instance.GetAmount(this.boosterId);
			this.amountLabel.text = amount.ToString();
			this.amountPivot.SetActive(amount > 0 && !this.Selected && !this.Unlimited);
			this.plusPivot.SetActive(amount <= 0 && !this.Selected && !this.Unlimited);
		}
		foreach (UIWidget uiwidget in this.dimmedWhenDisabled)
		{
			uiwidget.Color = ((!base.Disabled) ? Color.white : Color.gray);
		}
	}

	private void HandleInventoryChanged(InventoryManager.ItemChangeInfo changeInfo)
	{
		if (changeInfo.Item == this.boosterId)
		{
			this.ReflectState();
		}
	}

	[SerializeField]
	private UISprite icon;

	[SerializeField]
	[OptionalSerializedField]
	private UISprite lockedIcon;

	[SerializeField]
	private InventoryItem boosterId;

	[SerializeField]
	private UIWidget[] dimmedWhenDisabled = new UIWidget[0];

	[SerializeField]
	private GameObject enabledPivot;

	[SerializeField]
	private GameObject amountPivot;

	[SerializeField]
	private GameObject plusPivot;

	[SerializeField]
	private GameObject selectedPivot;

	[SerializeField]
	private GameObject freePivot;

	[SerializeField]
	private GameObject disabledPivot;

	[SerializeField]
	private GameObject lockedPivot;

	[SerializeField]
	private GameObject badgePivot;

	[SerializeField]
	[OptionalSerializedField]
	private GameObject checkMark;

	[SerializeField]
	[OptionalSerializedField]
	private GameObject unlimitedCheckMark;

	[SerializeField]
	[OptionalSerializedField]
	private GameObject unlimitedPivot;

	[SerializeField]
	[OptionalSerializedField]
	private UILabel unlimitedTimerLabel;

	[SerializeField]
	[OptionalSerializedField]
	private UILabel unlockLevel;

	private bool selected;

	private bool locked;

	private bool free;

	private bool unlimited;

	private bool isListeningToInventory;

	private bool forcedAmounts;

	public UILabel amountLabel;
}
