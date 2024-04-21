using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using Tactile;
using TactileModules.Validation;
using UnityEngine;

[RequireComponent(typeof(UIElement))]
public class RewardGrid : MonoBehaviour
{
	public void Clear()
	{
		List<GameObject> list = new List<GameObject>();
		IEnumerator enumerator = this.itemArea.transform.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				Transform transform = (Transform)obj;
				list.Add(transform.gameObject);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		foreach (GameObject gameObject in list)
		{
			gameObject.SetActive(false);
			UnityEngine.Object.Destroy(gameObject.gameObject);
		}
		this.slots.Clear();
	}

	public void Initialize(List<ItemAmount> rewards, bool overridePresetColumns = true)
	{
		Vector2 elementSize = this.itemArea.GetElementSize();
		this.itemArea.GetElement().SetSizeAndDoLayout(elementSize);
		if (overridePresetColumns)
		{
			this.itemArea.numColums = rewards.Count;
		}
		int num = 0;
		foreach (ItemAmount itemAmount in rewards)
		{
			UISprite uisprite = this.ConstructSprite(itemAmount.ItemId, itemAmount.Amount);
			uisprite.transform.parent = this.itemArea.transform;
			uisprite.transform.localPosition = Vector3.zero;
			uisprite.gameObject.name = num++.ToString();
			this.slots.Add(new RewardGrid.Slot
			{
				sprite = uisprite,
				item = itemAmount
			});
		}
		this.itemArea.Layout();
	}

	private float PreferredItemWidth
	{
		get
		{
			return this.itemArea.GetElementSize().y;
		}
	}

	private UISprite ConstructSprite(InventoryItem item, int amount)
	{
		UISprite uisprite = null;
		UILabel uilabel = null;
		if (this.customRewardItemPrefab != null)
		{
			RewardItem component = UnityEngine.Object.Instantiate<RewardItem>(this.customRewardItemPrefab).GetComponent<RewardItem>();
			uisprite = component.icon;
			uilabel = component.label;
		}
		else
		{
			foreach (UIView uiview in UIProjectSettings.Get().viewPrefabs)
			{
				if (uiview is AddToInventoryView)
				{
					RewardItem rewardItemPrefab = ((AddToInventoryView)uiview).rewardItemPrefab;
					if (rewardItemPrefab != null)
					{
						RewardItem component2 = UnityEngine.Object.Instantiate<RewardItem>(rewardItemPrefab).GetComponent<RewardItem>();
						uisprite = component2.icon;
						uilabel = component2.label;
					}
				}
			}
		}
		if (uisprite == null)
		{
			uisprite = new GameObject("spr").AddComponent<UISprite>();
			UIElement uielement = uisprite.gameObject.AddComponent<UIElement>();
			uielement.SetSizeAndDoLayout(Vector2.one * this.PreferredItemWidth);
		}
		InventoryItemMetaData metaData = InventoryManager.Instance.GetMetaData(item);
		uisprite.SpriteName = metaData.IconSpriteName;
		uisprite.KeepAspect = true;
		uisprite.Atlas = UIProjectSettings.Get().defaultAtlas;
		if (uilabel != null)
		{
			uilabel.text = metaData.FormattedQuantity(amount);
		}
		uisprite.gameObject.SetLayerRecursively(base.gameObject.layer);
		return uisprite;
	}

	public void ShowRewards()
	{
		for (int i = 0; i < this.slots.Count; i++)
		{
			RewardGrid.Slot slot = this.slots[i];
			if (slot != null && slot.sprite != null && slot.sprite.gameObject != null)
			{
				slot.sprite.gameObject.SetActive(true);
			}
		}
	}

	public IEnumerator Animate(bool detach = false, bool dimBackground = true)
	{
		UICamera.DisableInput();
		yield return new Fiber.OnExit(delegate()
		{
			UICamera.EnableInput();
		});
		Dictionary<string, int> tempInventoryCounts = new Dictionary<string, int>();
		UIViewManager.UIViewStateGeneric<AddToInventoryView> vs = UIViewManager.Instance.ShowView<AddToInventoryView>(new object[0]);
		AddToInventoryView view = vs.View;
		view.PauseInventoryListeners();
		if (detach)
		{
			base.transform.parent = view.gameObject.transform;
			base.gameObject.SetLayerRecursively(view.gameObject.layer);
		}
		if (dimBackground)
		{
			view.StartDimming(0.4f, 0f, 0.5f);
			yield return FiberHelper.Wait(0.4f, (FiberHelper.WaitFlag)0);
		}
		else
		{
			yield return FiberHelper.Wait(0.1f, (FiberHelper.WaitFlag)0);
		}
		this.itemArea.enabled = false;
		this.slots.Sort((RewardGrid.Slot a, RewardGrid.Slot b) => view.CompareItems(a.item, b.item));
		for (int j = 0; j < this.slots.Count; j++)
		{
			RewardGrid.Slot slot2 = this.slots[j];
			if (!tempInventoryCounts.ContainsKey(slot2.item.ItemId))
			{
				tempInventoryCounts.Add(slot2.item.ItemId, InventoryManager.Instance.GetAmount(slot2.item.ItemId));
			}
			tempInventoryCounts[slot2.item.ItemId] = tempInventoryCounts[slot2.item.ItemId] - slot2.item.Amount;
			int startAmount2 = tempInventoryCounts[slot2.item.ItemId];
			view.InitializeViewsWithPreviousAmounts(slot2.item, startAmount2);
		}
		for (int i = 0; i < this.slots.Count; i++)
		{
			RewardGrid.Slot slot = this.slots[i];
			ItemAmount itemNext = (i >= this.slots.Count - 1) ? null : this.slots[i + 1].item;
			ItemAmount itemPrev = (i <= 0) ? null : this.slots[i - 1].item;
			int startAmount = tempInventoryCounts[slot.item.ItemId];
			Dictionary<string, int> dictionary;
			string itemId;
			(dictionary = tempInventoryCounts)[itemId = slot.item.ItemId] = dictionary[itemId] + slot.item.Amount;
			yield return view.AnimateItem(slot.item, itemPrev, itemNext, startAmount, tempInventoryCounts[slot.item.ItemId], this.GetSourcePosition(view, slot.sprite.gameObject), slot.sprite.Size, slot.sprite.transform, delegate
			{
				slot.sprite.gameObject.SetActive(false);
			});
		}
		view.ResumeInventoryListeners();
		if (dimBackground)
		{
			view.StartDimming(0.4f, 0.5f, 0f);
			yield return FiberHelper.Wait(0.4f, (FiberHelper.WaitFlag)0);
		}
		view.Close(0);
		yield break;
	}

	private Vector3 GetSourcePosition(UIView view, GameObject sourceObject)
	{
		UIViewLayer viewLayerWithView = UIViewManager.Instance.GetViewLayerWithView(view);
		Camera viewCamera = viewLayerWithView.ViewCamera;
		Camera cachedCamera = UIViewManager.Instance.FindCameraFromObjectLayer(sourceObject.layer).cachedCamera;
		float d = viewCamera.orthographicSize / cachedCamera.orthographicSize;
		return sourceObject.transform.position * d;
	}

	public void EmptySlots()
	{
		while (this.slots.Count > 0)
		{
			RewardGrid.Slot slot = this.slots[0];
			this.slots.RemoveAt(0);
			slot.sprite.transform.parent = null;
		}
	}

	[SerializeField]
	private UIGridLayout itemArea;

	[SerializeField]
	[OptionalSerializedField]
	private RewardItem customRewardItemPrefab;

	private List<RewardGrid.Slot> slots = new List<RewardGrid.Slot>();

	private class Slot
	{
		public UISprite sprite;

		public ItemAmount item;
	}
}
