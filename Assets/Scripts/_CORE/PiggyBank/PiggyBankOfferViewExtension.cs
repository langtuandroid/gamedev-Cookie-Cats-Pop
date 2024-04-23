using System;
using System.Collections;
using System.Collections.Generic;
using TactileModules.PuzzleGame.PiggyBank.Interfaces;
using UnityEngine;

public class PiggyBankOfferViewExtension : MonoBehaviour, IPiggyBankOfferViewExtension
{
	public void Initialize(List<ItemAmount> offerItems)
	{
		this.rewardGrid.Initialize(offerItems, false);
	}

	public IEnumerator AnimateItemsToInventory(List<ItemAmount> items)
	{
		yield return this.rewardGrid.Animate(false, true);
		yield break;
	}

	[SerializeField]
	private RewardGrid rewardGrid;
}
