using System;
using TactileModules.PuzzleGame.SlidesAndLadders.Views;

public class SlidesAndLaddersChestDot : SlidesAndLaddersMapDot
{
	private void Clicked(UIEvent e)
	{
		UIViewManager.Instance.ShowView<SlidesAndLaddersRewardInfoView>(new object[0]);
	}

	public override void UpdateUI()
	{
	}
}
