using System;
using UnityEngine;

public interface IRewardTarget
{
	Transform TryGetTransformTarget(InventoryItem item);

	UILabel TryGetAmountLabel(InventoryItem item);

	void Initialize();

	void DisableInventoryListeners();

	void ReEnableInventoryListeners();
}
