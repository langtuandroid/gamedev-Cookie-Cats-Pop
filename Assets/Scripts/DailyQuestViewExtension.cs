using System;
using System.Collections;
using UnityEngine;

public class DailyQuestViewExtension : MonoBehaviour, DailyQuestSagaView.IExtension
{
	private void Awake()
	{
		this.view = base.GetComponent<DailyQuestSagaView>();
	}

	protected void ViewWillAppear()
	{
		this.currencyOverlay = UIViewManager.Instance.ObtainOverlay<CurrencyOverlay>(this.view);
	}

	protected void ViewDidDisappear()
	{
		UIViewManager.Instance.ReleaseOverlay<CurrencyOverlay>();
	}

	public void UpdateCloseButtonVisibility(bool isVisible)
	{
	}

	public IEnumerator AnimateCoinsForFriend(Vector3 fromWorldPos, int amount)
	{
		this.currencyOverlay.coinAnimator.GiveCoins(fromWorldPos, 1, 0.7f, null, null);
		yield break;
	}

	private DailyQuestSagaView view;

	private CurrencyOverlay currencyOverlay;
}
