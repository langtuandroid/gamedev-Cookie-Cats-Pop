using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TournamentUnlockedViewExtension : MonoBehaviour, TournamentUnlockedView.IExtension
{
	private void Awake()
	{
		this.celebrationState.SetActive(false);
	}

	public void SetRewardVisuals(List<ItemAmount> rewards)
	{
		this.celebrationState.SetActive(true);
		this.rewardGrid.Initialize(rewards, true);
	}

	public IEnumerator AnimateGivingRewards()
	{
		yield return this.rewardGrid.Animate(true, true);
		yield break;
	}

	[SerializeField]
	private RewardGrid rewardGrid;

	[SerializeField]
	private GameObject celebrationState;
}
