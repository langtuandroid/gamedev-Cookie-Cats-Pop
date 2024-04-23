using System;
using TactileModules.PuzzleGame.ThemeHunt;
using UnityEngine;

public class ThemeHuntMapButton : SideMapButton
{
	protected override void UpdateOncePerSecond()
	{
		int count = ThemeHuntManagerBase.Instance.GetActiveThemeItems.Count;
		this.themeHuntBadgeCount.text = count.ToString();
		this.themeHuntBadge.gameObject.SetActive(count > 0);
		this.themeHuntCount.text = ThemeHuntManagerBase.Instance.GetCollectedItems.ToString() + "/" + ThemeHuntManagerBase.Instance.GetNextThemeHuntReward().ThemeItemsRequired;
	}

	private new void Clicked(UIEvent e)
	{
		UIViewManager.Instance.ShowView<ThemeHuntProgressView>(new object[0]);
	}

	public override SideMapButton.AreaSide Side
	{
		get
		{
			return SideMapButton.AreaSide.Left;
		}
	}

	public override bool VisibilityChecker(object data)
	{
		return ThemeHuntManagerBase.Instance.IsHuntActiveOnClient();
	}

	public override Vector2 Size
	{
		get
		{
			return Vector2.zero;
		}
	}

	public override object Data
	{
		get
		{
			return null;
		}
	}

	[SerializeField]
	private UILabel themeHuntBadgeCount;

	[SerializeField]
	private GameObject themeHuntBadge;

	[SerializeField]
	private UILabel themeHuntCount;
}
