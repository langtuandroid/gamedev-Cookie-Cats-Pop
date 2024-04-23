using System;
using System.Collections.Generic;
using Fibers;
using TactileModules.PuzzleGame.ThemeHunt;
using UnityEngine;

public class ThemeHuntProgressView : UIView
{
	protected override void ViewWillAppear()
	{
		this.spawnInfoPluralisText = this.spawnInfoPluralis.text;
		this.UpdateUI();
		this.UpdateUIText();
		ThemeHuntManagerBase.Instance.OnItemSpawn += this.UpdateUIText;
		ThemeHuntManagerBase.Instance.CheckForRewards();
	}

	protected override void ViewWillDisappear()
	{
		this.timerFiber.Terminate();
		ThemeHuntManagerBase.Instance.OnItemSpawn -= this.UpdateUIText;
	}

	private void UpdateUI()
	{
		this.bar.FillAmount = Mathf.Lerp(0f, 1f, ThemeHuntManagerBase.Instance.HuntProgress);
		List<ThemeHuntRewardItem> rewards = ThemeHuntManagerBase.Instance.Rewards;
		for (int i = 0; i < rewards.Count; i++)
		{
			ThemeHuntRewardItem themeHuntRewardItem = rewards[i];
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.themeHuntProgressItem);
			gameObject.transform.parent = this.bar.transform.parent;
			gameObject.transform.localScale = Vector3.one;
			gameObject.SetLayerRecursively(base.gameObject.layer);
			gameObject.transform.position = this.bar.transform.position - Vector3.right * this.bar.GetElementSize().x / 2f + Vector3.right * this.bar.GetElementSize().x * ((float)themeHuntRewardItem.ThemeItemsRequired / (float)ThemeHuntManagerBase.Instance.TotalItemsRequiredForHunt);
			gameObject.GetComponent<ThemeHuntProgressItem>().Init(themeHuntRewardItem, i < ThemeHuntManagerBase.Instance.GetClaimedRewards, i == rewards.Count - 1);
		}
	}

	private void UpdateUIText()
	{
		this.spawnInfoZero.gameObject.SetActive(false);
		this.spawnInfoSingular.gameObject.SetActive(false);
		this.spawnInfoPluralis.gameObject.SetActive(false);
		if (ThemeHuntManagerBase.Instance.GetActiveThemeItems.Count > 1)
		{
			this.spawnInfoPluralis.gameObject.SetActive(true);
			this.spawnInfoPluralis.text = string.Format(this.spawnInfoPluralisText, ThemeHuntManagerBase.Instance.GetActiveThemeItems.Count);
		}
		else if (ThemeHuntManagerBase.Instance.GetActiveThemeItems.Count == 1)
		{
			this.spawnInfoSingular.gameObject.SetActive(true);
		}
		else
		{
			this.spawnInfoZero.gameObject.SetActive(true);
		}
	}

	private void CloseClicked(UIEvent e)
	{
		base.Close(0);
	}

	[SerializeField]
	private UILabel spawnInfoZero;

	[SerializeField]
	private UILabel spawnInfoSingular;

	[SerializeField]
	private UILabel spawnInfoPluralis;

	[SerializeField]
	private UIFillModifier bar;

	[SerializeField]
	private GameObject themeHuntProgressItem;

	private string spawnInfoPluralisText;

	private Fiber timerFiber = new Fiber();
}
