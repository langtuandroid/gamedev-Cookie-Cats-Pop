using System;
using UnityEngine;

public class ThemeHuntProgressItem : MonoBehaviour
{
	public void Init(ThemeHuntRewardItem reward, bool hasBeenClaimed, bool hideBar = false)
	{
		this.rewardGrid.Initialize(reward.Rewards, true);
		this.itemsRequired.text = reward.ThemeItemsRequired.ToString();
		this.checkMark.gameObject.SetActive(hasBeenClaimed);
		this.bar.gameObject.SetActive(!hideBar);
	}

	[SerializeField]
	private RewardGrid rewardGrid;

	[SerializeField]
	private UILabel itemsRequired;

	[SerializeField]
	private UISprite checkMark;

	[SerializeField]
	private UISprite bar;
}
