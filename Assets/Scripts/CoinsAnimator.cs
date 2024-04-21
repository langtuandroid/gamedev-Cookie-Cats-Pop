using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using TactileModules.Validation;
using UnityEngine;

public class CoinsAnimator : MonoBehaviour
{
	private void Update()
	{
		if (this.animationFiber != null)
		{
			this.animationFiber.Step();
		}
	}

	public void GiveCoins(Vector3 position, int visibleCoins, float duration, string customSpriteName = null, Action callback = null)
	{
		if (this.destination == null)
		{
			return;
		}
		Vector3 position2 = this.destination.position;
		position2.z = base.gameObject.transform.position.z;
		this.animationFiber.Start(this.AnimateCoins(position, position2, this.GiveCoinsCurve, visibleCoins, duration, customSpriteName, callback));
	}

	public void UseCoins(Vector3 position, int visibleCoins, float duration)
	{
		if (this.destination == null)
		{
			return;
		}
		if (this.destination == null)
		{
			return;
		}
		Vector3 position2 = this.destination.position;
		this.animationFiber.Start(this.AnimateCoins(position2, position, this.UseCoinsCurve, visibleCoins, duration, null, null));
	}

	public IEnumerator WaitForCoinsAndChangeLayerIfNeeded(UIView uiView)
	{
		bool changedCoinLayer = false;
		while (this.IsAnimating)
		{
			if (uiView == null && !changedCoinLayer)
			{
				foreach (GameObject go in this.coins)
				{
					go.SetLayerRecursively(UIViewManager.Instance.TopLayer.CameraLayer);
					changedCoinLayer = true;
				}
			}
			yield return null;
		}
		yield break;
	}

	public IEnumerator WaitForAnimationToComplete()
	{
		while (this.IsAnimating)
		{
			yield return null;
		}
		yield break;
	}

	public bool IsAnimating
	{
		get
		{
			return !this.animationFiber.IsTerminated;
		}
	}

	private IEnumerator AnimateCoins(Vector3 sourcePosition, Vector3 destination, AnimationCurve curve, int visibleCoins, float duration, string customSpriteName, Action callback = null)
	{
		IEnumerator[] coins = new IEnumerator[visibleCoins];
		float deltaDelay = duration / (float)visibleCoins * 0.5f;
		for (int i = 0; i < coins.Length; i++)
		{
			float delay = deltaDelay * (float)i;
			coins[i] = this.SpawnAndMoveCoin(sourcePosition, destination, curve, duration, delay, customSpriteName);
		}
		yield return FiberHelper.RunParallel(coins);
		if (callback != null)
		{
			callback();
		}
		yield break;
	}

	private IEnumerator SpawnAndMoveCoin(Vector3 sourcePosition, Vector3 destination, AnimationCurve curve, float duration, float delay, string customSpriteName)
	{
		if (delay > 0f)
		{
			yield return FiberHelper.Wait(delay, (FiberHelper.WaitFlag)0);
		}
		GameObject coin = UnityEngine.Object.Instantiate<GameObject>(this.coinPrefab);
		this.coins.Add(coin);
		if (customSpriteName != null)
		{
			coin.GetComponent<UISprite>().SpriteName = customSpriteName;
		}
		yield return new Fiber.OnExit(delegate()
		{
			this.coins.Remove(coin);
			UnityEngine.Object.Destroy(coin);
		});
		yield return FiberAnimation.MoveTransform(coin.transform, sourcePosition, destination, curve, duration);
		yield break;
	}

	private void OnDestroy()
	{
		Fiber.TerminateIfAble(this.animationFiber);
	}

	public GameObject coinPrefab;

	public Transform destination;

	[OptionalSerializedField]
	public UISprite sprite;

	public AnimationCurve UseCoinsCurve;

	public AnimationCurve GiveCoinsCurve;

	private Fiber animationFiber = new Fiber(FiberBucket.Manual);

	private List<GameObject> coins = new List<GameObject>();
}
