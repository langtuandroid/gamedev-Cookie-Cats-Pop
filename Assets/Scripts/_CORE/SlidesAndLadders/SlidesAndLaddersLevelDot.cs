using System;
using UnityEngine;

public class SlidesAndLaddersLevelDot : SlidesAndLaddersMapDot
{
	public override void Initialize()
	{
		this.UpdateUI();
	}

	public override void UpdateUI()
	{
		this.enabledRoot.SetActive(this.IsUnlocked && !this.IsCompleted && !base.IsCompletedLevel());
		this.disabledRoot.SetActive(!this.IsUnlocked || base.IsCompletedLevel());
		this.rewardRoot.SetActive(base.IsActiveTreasureLevel());
		this.normalLevel.SetActive(base.IsNeitherSlideOrLadderLevel());
		this.slideLevel.SetActive(base.IsSlideLevel() && !base.IsLadderLevel());
		this.ladderLevel.SetActive(base.IsLadderLevel() && !base.IsSlideLevel());
		this.slideAndLadderLevel.SetActive(base.IsSlideLevel() && base.IsLadderLevel());
		this.completedLevel.SetActive(base.IsCompletedLevel());
	}

	public GameObject enabledRoot;

	public GameObject disabledRoot;

	public GameObject rewardRoot;

	[Header("Level Dot Sprites")]
	public GameObject normalLevel;

	public GameObject slideLevel;

	public GameObject ladderLevel;

	public GameObject slideAndLadderLevel;

	public GameObject completedLevel;
}
