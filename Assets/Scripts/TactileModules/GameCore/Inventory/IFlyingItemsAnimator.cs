using System;
using System.Collections;
using UnityEngine;

namespace TactileModules.GameCore.Inventory
{
	public interface IFlyingItemsAnimator
	{
		event Action<Transform> ItemCreated;

		event Action<Transform> ItemDestroyed;

		float Progress { get; }

		IEnumerator AnimateFlyingItems(InventoryItem inventoryItem, int layer, Vector3 sourcePos, Vector3 destPos, int visibleItems);
	}
}
