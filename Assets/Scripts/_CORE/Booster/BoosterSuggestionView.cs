using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class BoosterSuggestionView : UIView
{
	protected override void ViewLoad(object[] parameters)
	{
		if (parameters.Length > 1)
		{
			this.boosterItems = (List<InventoryItem>)parameters[1];
		}
		List<ItemAmount> list = new List<ItemAmount>();
		foreach (InventoryItem t in this.boosterItems)
		{
			ItemAmount item = new ItemAmount
			{
				ItemId = t,
				Amount = 0
			};
			list.Add(item);
		}
		RewardGrid instance = this.boosterGridInstantiator.GetInstance<RewardGrid>();
		instance.Initialize(list, true);
	}

	[UsedImplicitly]
	private void DismissButton(UIEvent e)
	{
		base.Close(0);
	}

	[UsedImplicitly]
	private void UseBoosterButton(UIEvent e)
	{
		if (base.IsClosing)
		{
			return;
		}
		base.Close(this.boosterItems);
	}

	[SerializeField]
	private UIInstantiator boosterGridInstantiator;

	private List<InventoryItem> boosterItems;
}
