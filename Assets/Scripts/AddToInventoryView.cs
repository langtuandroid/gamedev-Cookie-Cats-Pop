using System;
using System.Collections;
using Fibers;
using Tactile;
using TactileModules.Foundation;
using TactileModules.Inventory;
using TactileModules.PuzzleGame.Inventory;
using UnityEngine;

public class AddToInventoryView : UIView
{
	protected override void ViewLoad(object[] parameters)
	{
		this.defaultTarget.Target.Initialize();
		foreach (AddToInventoryView.InventoryRewardTarget inventoryRewardTarget in this.rewardTargets)
		{
			inventoryRewardTarget.Target.Initialize();
		}
		this.dimmedBackground.gameObject.SetActive(false);
	}

	protected override void ViewWillAppear()
	{
		this.defaultTarget.SetActive(false);
		foreach (AddToInventoryView.InventoryRewardTarget inventoryRewardTarget in this.rewardTargets)
		{
			inventoryRewardTarget.SetActive(false);
		}
	}

	public void StartDimming(float duration, float sourceAlpha, float destAlpha)
	{
		this.dimmedBackgroundFiber = new Fiber(FiberBucket.Manual);
		this.dimmedBackgroundFiber.Start(FiberAnimation.Animate(duration, delegate(float t)
		{
			this.dimmedBackground.Alpha = sourceAlpha + (destAlpha - sourceAlpha) * t;
			this.dimmedBackground.gameObject.SetActive(this.dimmedBackground.Alpha > 0.0001f);
		}));
	}

	private void Update()
	{
		if (this.dimmedBackgroundFiber != null)
		{
			this.dimmedBackgroundFiber.Step();
		}
	}

	public void InitializeViewsWithPreviousAmounts(ItemAmount item, int startAmount)
	{
		if (item.ItemId == "Coin")
		{
			if (this.savedCurrencyOverlay == null)
			{
				CurrencyOverlay x = base.ObtainOverlay<CurrencyOverlay>();
				if (x != null)
				{
					this.savedCurrencyOverlay = x;
					base.ReleaseOverlay<CurrencyOverlay>();
				}
			}
			if (this.savedCurrencyOverlay != null)
			{
				this.savedCurrencyOverlay.coinButton.coins.text = startAmount.ToString();
			}
		}
		else if (item.ItemId == "Life")
		{
			LivesOverlay livesOverlay = base.ObtainOverlay<LivesOverlay>();
			if (livesOverlay != null)
			{
				livesOverlay.lifeBar.lifeCounterLabel.text = startAmount.ToString();
				base.ReleaseOverlay<LivesOverlay>();
			}
		}
		else if (item.ItemId == "UnlimitedLives")
		{
			LivesOverlay livesOverlay2 = base.ObtainOverlay<LivesOverlay>();
			if (livesOverlay2 != null)
			{
				livesOverlay2.lifeBar.lifeCounterLabel.text = startAmount.ToString();
				base.ReleaseOverlay<LivesOverlay>();
			}
		}
		AddToInventoryView.InventoryRewardTarget inventoryRewardTarget = this.TryGetRewardTarget(item.ItemId);
		if (inventoryRewardTarget == null)
		{
			inventoryRewardTarget = this.defaultTarget;
		}
		if (inventoryRewardTarget != null)
		{
			UILabel uilabel = inventoryRewardTarget.Target.TryGetAmountLabel(item.ItemId);
			uilabel.text = startAmount.ToString();
		}
	}

	public IEnumerator AnimateItem(ItemAmount item, ItemAmount prevItem, ItemAmount nextItem, int startAmount, int finalAmount, Vector3 sourcePosition, Vector2 size, Transform space, Action movingStarted)
	{
		IUnlimitedItems unlimitedItems = ManagerRepository.Get<InventorySystem>().UnlimitedItems;
		bool isUnlimited = unlimitedItems.IsUnlimitedType(item.ItemId);
		string inventoryItem = AddToInventoryView.GetInventoryItem(item, unlimitedItems);
		string previousInventoryItem = AddToInventoryView.GetInventoryItem(prevItem, unlimitedItems);
		string nextInventoryItem = AddToInventoryView.GetInventoryItem(nextItem, unlimitedItems);
		InventoryItemMetaData meta = InventoryManager.Instance.GetMetaData(item.ItemId);
		int previous = isUnlimited ? Mathf.Max(0, InventoryManager.Instance.GetAmount(inventoryItem)) : startAmount;
		if (item.ItemId == "Coin")
		{
			CurrencyOverlay currencyOverlay = base.ObtainOverlay<CurrencyOverlay>();
			if (currencyOverlay != null)
			{
				movingStarted();
				yield return this.AnimateCoins(currencyOverlay, sourcePosition, previous, finalAmount, item.Amount);
				yield break;
			}
		}
		else if (inventoryItem == "Life")
		{
			LivesOverlay livesOverlay = base.ObtainOverlay<LivesOverlay>();
			if (livesOverlay != null)
			{
				movingStarted();
				yield return this.AnimateLives(livesOverlay, sourcePosition, previous, finalAmount, item.Amount);
				yield break;
			}
		}
		else if (inventoryItem == "UnlimitedLives")
		{
			LivesOverlay livesOverlay2 = base.ObtainOverlay<LivesOverlay>();
			if (livesOverlay2 != null)
			{
				movingStarted();
				yield return this.AnimateUnlimitedLives(livesOverlay2, sourcePosition, previous);
				yield break;
			}
		}
		AddToInventoryView.InventoryRewardTarget rewardTarget = this.TryGetRewardTarget(inventoryItem);
		AddToInventoryView.InventoryRewardTarget rewardTargetNext = (nextInventoryItem == null) ? null : this.TryGetRewardTarget(nextInventoryItem);
		AddToInventoryView.InventoryRewardTarget rewardTargetPrev = (previousInventoryItem == null) ? null : this.TryGetRewardTarget(previousInventoryItem);
		if (rewardTarget == null)
		{
			rewardTarget = this.defaultTarget;
		}
		if (rewardTarget != null)
		{
			if (!rewardTarget.IsActive())
			{
				rewardTarget.SetActive(true);
			}
			UILabel amountLabel = rewardTarget.Target.TryGetAmountLabel(inventoryItem);
			amountLabel.gameObject.SetActive(true);
			amountLabel.text = previous.ToString();
			if (rewardTarget != rewardTargetPrev)
			{
				yield return rewardTarget.SlideIn(base.GetElementSize(), SingletonAsset<CommonCurves>.Instance.easeInOut, this.slidingDuration);
				yield return FiberHelper.Wait(0.2f, (FiberHelper.WaitFlag)0);
			}
			movingStarted();
			Transform targetTransform = rewardTarget.Target.TryGetTransformTarget(inventoryItem);
			if (isUnlimited)
			{
				yield return this.AnimateUnlimited(sourcePosition, size, space, targetTransform, meta.IconSpriteName);
			}
			else
			{
				yield return this.AnimateNormal(item, sourcePosition, size, space, targetTransform, meta, amountLabel, previous, finalAmount);
			}
			if (rewardTarget != rewardTargetNext)
			{
				yield return FiberHelper.Wait(0.2f, (FiberHelper.WaitFlag)0);
				yield return rewardTarget.SlideOut(base.GetElementSize(), SingletonAsset<CommonCurves>.Instance.easeInOut, this.slidingDuration);
			}
			if (rewardTarget != rewardTargetNext || rewardTargetNext == null)
			{
				rewardTarget.SetActive(false);
			}
		}
		yield break;
	}

	private static string GetInventoryItem(ItemAmount itemAmount, IUnlimitedItems unlimitedItems)
	{
		string text = (itemAmount == null) ? null : itemAmount.ItemId;
		if (text != null && unlimitedItems.IsUnlimitedType(itemAmount.ItemId))
		{
			text = unlimitedItems.GetCorrespondingNonUnlimitedItem(itemAmount.ItemId);
		}
		return text;
	}

	private IEnumerator AnimateUnlimited(Vector3 sourcePosition, Vector2 size, Transform space, Transform targetTransform, string iconSpriteName)
	{
		yield return this.SpawnAndMoveSingle(sourcePosition, targetTransform.position, this.flyCurve, 0.5f, 0f, iconSpriteName, size, space);
		yield break;
	}

	private IEnumerator AnimateNormal(ItemAmount item, Vector3 sourcePosition, Vector2 size, Transform space, Transform targetTransform, InventoryItemMetaData meta, UILabel amountLabel, int previous, int current)
	{
		yield return FiberHelper.RunParallel(new IEnumerator[]
		{
			this.SpawnAndMove(sourcePosition, targetTransform.position + Vector3.back * 10f, this.flyCurve, 0.5f, 0f, meta.IconSpriteName, size, space, item.Amount),
			FiberHelper.RunSerial(new IEnumerator[]
			{
				FiberHelper.Wait(0.1f, (FiberHelper.WaitFlag)0),
				FiberAnimation.ScaleTransform(amountLabel.transform.parent, Vector3.zero, amountLabel.transform.parent.localScale, this.itemHitCurve, 0.9f)
			}),
			FiberHelper.RunSerial(new IEnumerator[]
			{
				FiberHelper.Wait(0.1f, (FiberHelper.WaitFlag)0),
				this.AnimateLabel(amountLabel, previous, current, 0.4f)
			})
		});
		yield break;
	}

	private IEnumerator AnimateLabel(UILabel label, int startAmount, int endAmount, float duration)
	{
		if (Mathf.Abs(startAmount - endAmount) > 1)
		{
			float delta = (float)(endAmount - startAmount);
			yield return FiberAnimation.Animate(duration, delegate(float t)
			{
				int num = Mathf.RoundToInt((float)startAmount + delta * t);
				label.text = num.ToString();
			});
		}
		label.text = endAmount.ToString();
		yield break;
	}

	private AddToInventoryView.InventoryRewardTarget TryGetRewardTarget(string inventoryItemId)
	{
		foreach (AddToInventoryView.InventoryRewardTarget inventoryRewardTarget in this.rewardTargets)
		{
			if (inventoryRewardTarget.Target.TryGetTransformTarget(inventoryItemId) != null)
			{
				return inventoryRewardTarget;
			}
		}
		return null;
	}

	private IEnumerator SpawnAndMove(Vector3 sourcePosition, Vector3 destination, AnimationCurve curve, float duration, float delay, string customSpriteName, Vector2 size, Transform space, int amount)
	{
		IEnumerator[] enums = new IEnumerator[amount];
		float maxDelay = duration * 0.5f;
		float dDelay = maxDelay / (float)amount;
		for (int i = 0; i < amount; i++)
		{
			float delay2 = (float)i * dDelay;
			enums[i] = this.SpawnAndMoveSingle(sourcePosition, destination, curve, duration, delay2, customSpriteName, size, space);
		}
		yield return FiberHelper.RunParallel(enums);
		SingletonAsset<UISetup>.Instance.itemPutIntoInventory.Play();
		yield break;
	}

	private IEnumerator SpawnAndMoveSingle(Vector3 sourcePosition, Vector3 destination, AnimationCurve curve, float duration, float delay, string customSpriteName, Vector2 size, Transform space)
	{
		if (delay > 0f)
		{
			yield return FiberHelper.Wait(delay, (FiberHelper.WaitFlag)0);
		}
		GameObject coin = UnityEngine.Object.Instantiate<GameObject>(this.flyingItemPrefab);
		coin.SetActive(true);
		Transform oldTransform = coin.transform.parent;
		coin.transform.parent = space;
		coin.transform.localScale = Vector3.one;
		coin.transform.parent = oldTransform;
		UISprite sprite = coin.GetComponent<UISprite>();
		sprite.SpriteName = customSpriteName;
		coin.SetLayerRecursively(base.gameObject.layer);
		yield return new Fiber.OnExit(delegate()
		{
			UnityEngine.Object.Destroy(coin);
		});
		yield return FiberHelper.RunParallel(new IEnumerator[]
		{
			FiberAnimation.MoveTransform(coin.transform, sourcePosition, destination, curve, duration),
			FiberAnimation.ScaleTransform(coin.transform, coin.transform.localScale, Vector3.one, null, duration),
			this.AnimateSize(sprite, size, sprite.Size, duration)
		});
		yield break;
	}

	private IEnumerator AnimateCoins(CurrencyOverlay currencyOverlay, Vector3 sourcePosition, int previous, int current, int amount)
	{
		currencyOverlay.coinButton.coins.text = previous.ToString();
		currencyOverlay.coinAnimator.GiveCoins(sourcePosition, Mathf.Min(amount, 10), 0.5f, null, null);
		yield return FiberAnimation.Animate(0.5f, delegate(float t)
		{
			int num = Mathf.RoundToInt(Mathf.Lerp((float)previous, (float)current, t));
			currencyOverlay.coinButton.coins.text = num.ToString();
		});
		SingletonAsset<UISetup>.Instance.itemPutIntoInventory.Play();
		yield return FiberHelper.Wait(0.3f, (FiberHelper.WaitFlag)0);
		currencyOverlay.coinButton.coins.text = current.ToString();
		yield break;
	}

	private IEnumerator AnimateLives(LivesOverlay currencyOverlay, Vector3 sourcePosition, int previous, int current, int amount)
	{
		base.ObtainOverlay<LivesOverlay>();
		currencyOverlay.lifeBar.lifeCounterLabel.text = previous.ToString();
		currencyOverlay.heartAnimator.GiveCoins(sourcePosition, Mathf.Min(amount, 10), 0.5f, null, null);
		yield return FiberAnimation.Animate(0.5f, delegate(float t)
		{
			int num = Mathf.RoundToInt(Mathf.Lerp((float)previous, (float)current, t));
			currencyOverlay.lifeBar.lifeCounterLabel.text = num.ToString();
		});
		SingletonAsset<UISetup>.Instance.itemPutIntoInventory.Play();
		yield return FiberHelper.Wait(0.3f, (FiberHelper.WaitFlag)0);
		currencyOverlay.lifeBar.lifeCounterLabel.text = current.ToString();
		base.ReleaseOverlay<LivesOverlay>();
		yield break;
	}

	private IEnumerator AnimateUnlimitedLives(LivesOverlay livesOverlay, Vector3 sourcePosition, int previous)
	{
		base.ObtainOverlay<LivesOverlay>();
		string spriteName = InventoryManager.Instance.GetMetaData<InventoryItemMetaData>("UnlimitedLives").IconSpriteName;
		livesOverlay.lifeBar.lifeCounterLabel.text = previous.ToString();
		livesOverlay.heartAnimator.GiveCoins(sourcePosition, 1, 0.5f, spriteName, null);
		yield return FiberAnimation.Animate(0.5f, delegate(float t)
		{
		});
		SingletonAsset<UISetup>.Instance.itemPutIntoInventory.Play();
		yield return FiberHelper.Wait(0.3f, (FiberHelper.WaitFlag)0);
		livesOverlay.lifeBar.unlimitedLivesImage.SetActive(true);
		livesOverlay.lifeBar.lifeCounterLabel.gameObject.SetActive(false);
		base.ReleaseOverlay<LivesOverlay>();
		yield break;
	}

	private IEnumerator AnimateSize(UISprite sprite, Vector2 source, Vector2 dest, float duration)
	{
		yield return FiberAnimation.Animate(duration, delegate(float t)
		{
			sprite.Size = Vector2.Lerp(source, dest, t);
		});
		yield break;
	}

	public void PauseInventoryListeners()
	{
		this.savedCurrencyOverlay = base.ObtainOverlay<CurrencyOverlay>();
		if (this.savedCurrencyOverlay != null)
		{
			this.savedCurrencyOverlay.coinButton.PauseRefreshingCoins(true);
			base.ReleaseOverlay<CurrencyOverlay>();
		}
		foreach (AddToInventoryView.InventoryRewardTarget inventoryRewardTarget in this.rewardTargets)
		{
			inventoryRewardTarget.Target.DisableInventoryListeners();
		}
	}

	public void ResumeInventoryListeners()
	{
		if (this.savedCurrencyOverlay != null)
		{
			this.savedCurrencyOverlay.coinButton.PauseRefreshingCoins(false);
		}
		foreach (AddToInventoryView.InventoryRewardTarget inventoryRewardTarget in this.rewardTargets)
		{
			inventoryRewardTarget.Target.ReEnableInventoryListeners();
		}
	}

	public int CompareItems(ItemAmount a, ItemAmount b)
	{
		AddToInventoryView.InventoryRewardTarget inventoryRewardTarget = this.TryGetRewardTarget(a.ItemId);
		AddToInventoryView.InventoryRewardTarget inventoryRewardTarget2 = this.TryGetRewardTarget(b.ItemId);
		if (inventoryRewardTarget == null)
		{
			inventoryRewardTarget = this.defaultTarget;
		}
		if (inventoryRewardTarget2 == null)
		{
			inventoryRewardTarget2 = this.defaultTarget;
		}
		if (inventoryRewardTarget == inventoryRewardTarget2)
		{
			return 0;
		}
		return inventoryRewardTarget.GetHashCode().CompareTo(inventoryRewardTarget2.GetHashCode());
	}

	public GameObject flyingItemPrefab;

	public AnimationCurve flyCurve;

	public AnimationCurve itemHitCurve;

	public UISprite dimmedBackground;

	public AddToInventoryView.InventoryRewardTarget defaultTarget;

	public AddToInventoryView.InventoryRewardTarget[] rewardTargets;

	public float slidingDuration = 0.1f;

	public RewardItem rewardItemPrefab;

	private Fiber dimmedBackgroundFiber;

	private CurrencyOverlay savedCurrencyOverlay;

	public enum ScreenLocation
	{
		Top,
		Bottom
	}

	[Serializable]
	public class InventoryRewardTarget
	{
		public IRewardTarget Target
		{
			get
			{
				return this.instantiator.GetInstance().GetComponent<IRewardTarget>();
			}
		}

		public void SetActive(bool active)
		{
			this.instantiator.GetInstance().SetActive(active);
		}

		public bool IsActive()
		{
			return this.instantiator.GetInstance().activeInHierarchy;
		}

		public IEnumerator SlideIn(Vector2 viewSize, AnimationCurve curve, float slidingDuration)
		{
			Vector3 outPos;
			Vector3 inPos;
			this.GetInOutPositions(viewSize, out outPos, out inPos);
			yield return FiberAnimation.MoveTransform(this.instantiator.transform, outPos, inPos, curve, slidingDuration);
			yield break;
		}

		public IEnumerator SlideOut(Vector2 viewSize, AnimationCurve curve, float slidingDuration)
		{
			Vector3 outPos;
			Vector3 inPos;
			this.GetInOutPositions(viewSize, out outPos, out inPos);
			yield return FiberAnimation.MoveTransform(this.instantiator.transform, inPos, outPos, curve, slidingDuration);
			yield break;
		}

		private void GetInOutPositions(Vector2 viewSize, out Vector3 outPos, out Vector3 inPos)
		{
			if (this.screenLocation == AddToInventoryView.ScreenLocation.Bottom)
			{
				outPos = (-viewSize.y - this.instantiator.GetElementSize().y) * 0.5f * Vector3.up;
				inPos = (-viewSize.y + this.instantiator.GetElementSize().y) * 0.5f * Vector3.up;
			}
			else
			{
				outPos = (viewSize.y + this.instantiator.GetElementSize().y) * 0.5f * Vector3.up;
				inPos = (viewSize.y - this.instantiator.GetElementSize().y) * 0.5f * Vector3.up;
			}
		}

		public UIInstantiator instantiator;

		public AddToInventoryView.ScreenLocation screenLocation;
	}
}
