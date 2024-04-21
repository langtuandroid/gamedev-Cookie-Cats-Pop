using System;
using System.Collections.Generic;

namespace ConfigSchema
{
	[AttributeUsage(AttributeTargets.Property)]
	public class InventoryItemsAttribute : Attribute
	{
		public static List<string> InventoryItems()
		{
			return CollectionExtensions.GetNonEmptyConstStringValues(typeof(InventoryItem));
		}
	}
}
