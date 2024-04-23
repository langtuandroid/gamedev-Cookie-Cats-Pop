using System;
using System.Collections;
using UnityEngine;

namespace TactileModules.GameCore.Inventory
{
	public interface IInventoryItemAnimator : IDisposable
	{
		IEnumerator Animate(Vector3 startPoint);

		IEnumerator Animate(Func<InventoryItem, Vector3> startPointFunction);

		event Action<Transform> ItemCreated;

		event Action<Transform> ItemDestroyed;

		IEnumerator AnimateInventoryItem(Vector3 startPoint, InventoryItem item, int startAmount, int endAmount, Action<int> amountChanged);
	}
}
