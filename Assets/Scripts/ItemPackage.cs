using System;
using System.Collections.Generic;
using ConfigSchema;

[RequireAll]
[ObsoleteJsonName("ID")]
public class ItemPackage : IEquatable<ItemPackage>
{
	public ItemPackage()
	{
		this.Items = new List<ItemAmount>();
	}

	[ArrayFormat(ArrayFormatAttribute.ArrayFormat.table)]
	[JsonSerializable("Items", typeof(ItemAmount))]
	public List<ItemAmount> Items { get; set; }

	public bool Equals(ItemPackage other)
	{
		if (this.Items.Count != other.Items.Count)
		{
			return false;
		}
		List<ItemAmount> list = (this.Items.Count >= other.Items.Count) ? other.Items : this.Items;
		List<ItemAmount> list2 = (this.Items.Count <= other.Items.Count) ? other.Items : this.Items;
		for (int i = 0; i < list2.Count; i++)
		{
			for (int j = 0; j < list.Count; j++)
			{
				if (list2[i].ItemId != list[j].ItemId || list2[i].Amount != list[j].Amount)
				{
					return false;
				}
			}
		}
		return true;
	}
}
