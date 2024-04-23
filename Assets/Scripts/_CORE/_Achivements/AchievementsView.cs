using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using Tactile;
using TactileModules.Foundation;
using UnityEngine;

public class AchievementsView : UIView
{
	private AchievementsManager AchievementsManager
	{
		get
		{
			return ManagerRepository.Get<AchievementsManager>();
		}
	}

	protected override void ViewWillDisappear()
	{
		this.animCoinsFiber.Terminate();
	}

	protected override void ViewWillAppear()
	{
		base.ObtainOverlay<CurrencyOverlay>();
		this.UpdateTable();
	}

	protected override void ViewDidDisappear()
	{
		base.ReleaseOverlay<CurrencyOverlay>();
	}

	private void OnDestroy()
	{
		this.claimFiber.Terminate();
	}

	private void UpdateTable()
	{
		List<AchievementAsset> list = new List<AchievementAsset>();
		List<AchievementAsset> list2 = new List<AchievementAsset>();
		List<AchievementAsset> list3 = new List<AchievementAsset>();
		foreach (AchievementAsset achievementAsset in this.AchievementsManager.GetAllMetaData())
		{
			bool flag = this.AchievementsManager.IsAchievementCompleted(achievementAsset);
			bool flag2 = this.AchievementsManager.IsAchievementClaimed(achievementAsset);
			if (flag && flag2)
			{
				list.Add(achievementAsset);
			}
			else if (flag && !flag2)
			{
				list2.Add(achievementAsset);
			}
			else
			{
				list3.Add(achievementAsset);
			}
		}
		list3.Sort(delegate(AchievementAsset a, AchievementAsset b)
		{
			float num = (a.objectiveThreshold <= 0) ? 0f : ((float)this.AchievementsManager.GetAchievementProgress(a) / (float)a.objectiveThreshold);
			float num2 = (b.objectiveThreshold <= 0) ? 0f : ((float)this.AchievementsManager.GetAchievementProgress(b) / (float)b.objectiveThreshold);
			if (a.thresholdType == AchievementAsset.ThresholdType.ValueInSingleEventExact)
			{
				num = (float)((num < (float)a.objectiveThreshold) ? 0 : 1);
			}
			if (b.thresholdType == AchievementAsset.ThresholdType.ValueInSingleEventExact)
			{
				num2 = (float)((num2 < (float)b.objectiveThreshold) ? 0 : 1);
			}
			if (num < num2)
			{
				return 1;
			}
			if (num > num2)
			{
				return -1;
			}
			return 0;
		});
		List<AchievementAsset> list4 = list2;
		list4.AddRange(list3);
		list4.AddRange(list);
		this.itemList.DestroyAllContent();
		this.itemList.BeginAdding();
		foreach (AchievementAsset achievement in list4)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.elementPrefab);
			gameObject.SetActive(true);
			this.itemList.AddToContent(gameObject.GetComponent<UIElement>());
			AchievementItem component = gameObject.GetComponent<AchievementItem>();
			component.OnClaimClick += this.ClaimClicked;
			component.Initialize(achievement);
		}
		this.itemList.EndAdding();
		this.itemList.SetScroll(-(this.itemList.TotalContentSize + Vector2.up * 180f));
	}

	private void ButtonExitClicked(UIEvent e)
	{
		base.Close(0);
	}

	private void ClaimClicked(AchievementItem item)
	{
		this.claimFiber.Start(this.ClaimReward(item));
	}

	private IEnumerator ClaimReward(AchievementItem item)
	{
		UICamera.DisableInput();
		foreach (ItemAmount itemAmount in item.Reward.Rewards)
		{
			InventoryManager.Instance.Add(itemAmount.ItemId, itemAmount.Amount, "achievements");
		}
		this.AchievementsManager.ClaimAchievement(item.Achievement);
		UserSettingsManager.Instance.SyncUserSettings();
		yield return item.AnimateRewards();
		UICamera.EnableInput();
		yield break;
	}

	public GameObject elementPrefab;

	public UIListPanel itemList;

	private Fiber animCoinsFiber = new Fiber();

	private Fiber claimFiber = new Fiber();
}
