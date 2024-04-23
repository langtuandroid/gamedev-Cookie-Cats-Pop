using System;
using System.Collections;
using Tactile;
using UnityEngine;

public class DailyQuestFriendAvatar : MonoBehaviour
{
	private void OnEnable()
	{
		this.portrait.progress.gameObject.SetActive(false);
	}

	public IEnumerator MoveAvatar(Vector3 start, Vector3 end, CurrencyOverlay currencyOverlay)
	{
		yield return FiberAnimation.MoveTransform(base.transform, start, end, AnimationCurve.EaseInOut(0f, 0f, 1f, 1f), 0.4f);
		yield return FiberHelper.Wait(0.8f, (FiberHelper.WaitFlag)0);
		currencyOverlay.coinAnimator.GiveCoins(this.portrait.transform.position, 1, 0.7f, null, null);
		InventoryManager.Instance.AddCoins(1, "FriendMovedOnQuest");
		yield break;
	}

	public FacebookPortraitWithProgress portrait;
}
