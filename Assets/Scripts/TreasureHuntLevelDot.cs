using System;
using TactileModules.PuzzleGames.TreasureHunt;
using UnityEngine;

public class TreasureHuntLevelDot : TreasureHuntMapDot
{
	public override void Initialize()
	{
		this.UpdateUI();
	}

	public override void UpdateUI()
	{
		this.lvlNumber.text = base.LevelProxy.HumanNumber.ToString();
		this.lvlNumber.gameObject.SetActive(!this.IsCompleted);
		this.enabledRoot.SetActive(this.IsUnlocked && !this.IsCompleted);
		this.completedRoot.SetActive(this.IsCompleted);
		this.disabledRoot.SetActive(!this.IsUnlocked);
	}

	public UILabel lvlNumber;

	public GameObject enabledRoot;

	public GameObject disabledRoot;

	public GameObject completedRoot;
}
