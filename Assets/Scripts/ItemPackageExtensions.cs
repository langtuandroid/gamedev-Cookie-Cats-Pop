using System;
using System.Collections.Generic;

public static class ItemPackageExtensions
{
	public static int GetAmount(this List<ItemAmount> list, string itemId)
	{
		foreach (ItemAmount itemAmount in list)
		{
			if (itemAmount.ItemId == itemId)
			{
				return itemAmount.Amount;
			}
		}
		return 0;
	}

	public static List<ItemAmount> GetMergedItems(this List<ItemAmount> list)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		foreach (ItemAmount itemAmount in list)
		{
			if (!dictionary.ContainsKey(itemAmount.ItemId))
			{
				dictionary.Add(itemAmount.ItemId, 0);
			}
			Dictionary<string, int> dictionary2;
			string itemId;
			(dictionary2 = dictionary)[itemId = itemAmount.ItemId] = dictionary2[itemId] + itemAmount.Amount;
		}
		List<ItemAmount> list2 = new List<ItemAmount>();
		foreach (KeyValuePair<string, int> keyValuePair in dictionary)
		{
			list2.Add(new ItemAmount
			{
				ItemId = keyValuePair.Key,
				Amount = keyValuePair.Value
			});
		}
		return list2;
	}
}
