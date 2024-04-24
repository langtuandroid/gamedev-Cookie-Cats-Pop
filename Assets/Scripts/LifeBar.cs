using System;
using Tactile;
using TactileModules.Foundation;
using UnityEngine;

public class LifeBar : MonoBehaviour
{
	private LivesManager LivesManager
	{
		get
		{
			return LivesManager.Instance;
		}
	}

	private InventoryManager InventoryManager
	{
		get
		{
			return ManagerRepository.Get<InventoryManager>();
		}
	}
	

	public InventoryItem LivesType
	{
		get
		{
			return this.livesType;
		}
		set
		{
			this.livesType = value;
			InventoryItemMetaData metaData = this.InventoryManager.GetMetaData(this.livesType);
			this.heartSprite.SpriteName = metaData.IconSpriteName;
			this.RefreshLives();
		}
	}

	private void OnEnable()
	{
		this.InventoryManager.InventoryChanged += this.OnInventoryChanged;
		this.LivesManager.UnlimitedLivesChanged += this.RefreshLives;
		this.RefreshTimer();
		this.RefreshLives();
	}

	private void OnDisable()
	{
		this.InventoryManager.InventoryChanged -= this.OnInventoryChanged;
		this.LivesManager.UnlimitedLivesChanged -= this.RefreshLives;
	}

	private void Update()
	{
		if (Time.time < this.nextUpdateTimeStamp)
		{
			return;
		}
		this.nextUpdateTimeStamp = Time.time + 1f;
		this.RefreshTimer();
	}

	private void RefreshTimer()
	{
		if (this.LivesManager.HasUnlimitedLives())
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds((double)this.LivesManager.GetTimeLeftForInfiniteLives());
			this.timeCounterLabel.text = string.Format("{0:D2}:{1:D2}:{2:D2}", (int)timeSpan.TotalHours, timeSpan.Minutes, timeSpan.Seconds);
			return;
		}
		int num = 0;
		if (this.livesType != "TournamentLife")
		{
			num = Mathf.RoundToInt((float)this.LivesManager.GetRegenerationTimeLeft());
		}
		
		if (num > 0)
		{
			TimeSpan timeSpan2 = TimeSpan.FromSeconds((double)num);
			this.timeCounterLabel.text = string.Format("{0:D2}:{1:D2}", timeSpan2.Minutes, timeSpan2.Seconds);
		}
		else
		{
			this.timeCounterLabel.text = L.Get("Full");
		}
	}

	private void OnInventoryChanged(InventoryManager.ItemChangeInfo itemChangeInfo)
	{
		if (this.livesType == itemChangeInfo.Item)
		{
			this.RefreshLives();
		}
	}

	public void RefreshLives()
	{
		bool flag = this.LivesManager.HasUnlimitedLives();
		this.unlimitedLivesImage.SetActive(flag);
		this.lifeCounterLabel.gameObject.SetActive(!flag);
		this.lifeCounterLabel.text = this.InventoryManager.GetAmount(this.livesType).ToString();
	}

	[SerializeField]
	public UILabel lifeCounterLabel;

	[SerializeField]
	public GameObject unlimitedLivesImage;

	[SerializeField]
	private UILabel timeCounterLabel;

	[SerializeField]
	private UISprite heartSprite;

	private InventoryItem livesType = "Life";

	private float nextUpdateTimeStamp;
}
