using System;
using TactileModules.PuzzleGames.TreasureHunt;

public class TreasureHuntChestDot : TreasureHuntMapDot
{
	private void Clicked(UIEvent e)
	{
		UIViewManager.Instance.ShowView<TreasureHuntRewardInfoView>(new object[0]);
	}

	public override void UpdateUI()
	{
	}
}
