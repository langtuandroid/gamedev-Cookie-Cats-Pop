using System;
using System.Collections.Generic;
using Tactile;
using Tactile.XperiaGamesClub;

public class XperiaClubProvider : XperiaGiftPopupManager.IDataProvider
{
	public UIViewManager.UIViewState ShowGiftView()
	{
		return UIViewManager.Instance.ShowView<XperiaGiftRewardView>(new object[]
		{
			this.rewards
		});
	}

	public void GiveRewards()
	{
		foreach (ItemAmount itemAmount in this.rewards)
		{
			InventoryManager.Instance.Add(itemAmount.ItemId, itemAmount.Amount, "xperiaClub");
		}
	}

	private List<ItemAmount> rewards = new List<ItemAmount>
	{
		new ItemAmount
		{
			ItemId = "BoosterSuperAim",
			Amount = 3
		},
		new ItemAmount
		{
			ItemId = "BoosterRainbow",
			Amount = 3
		}
	};
}
