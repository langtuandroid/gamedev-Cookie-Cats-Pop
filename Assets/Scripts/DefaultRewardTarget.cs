using System;
using UnityEngine;

public class DefaultRewardTarget : MonoBehaviour, IRewardTarget
{
	Transform IRewardTarget.TryGetTransformTarget(InventoryItem item)
	{
		return this.target;
	}

	UILabel IRewardTarget.TryGetAmountLabel(InventoryItem item)
	{
		return this.amount;
	}

	void IRewardTarget.Initialize()
	{
	}

	void IRewardTarget.DisableInventoryListeners()
	{
	}

	void IRewardTarget.ReEnableInventoryListeners()
	{
	}

	[SerializeField]
	public Transform target;

	[SerializeField]
	public UILabel amount;
}
