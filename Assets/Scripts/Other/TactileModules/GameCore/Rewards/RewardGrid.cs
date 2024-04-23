using System;
using System.Collections;
using System.Collections.Generic;
using Tactile;
using UnityEngine;

namespace TactileModules.GameCore.Rewards
{
	[RequireComponent(typeof(UIElement))]
	public class RewardGrid : MonoBehaviour
	{
		public UIGridLayout ItemArea
		{
			get
			{
				return this.itemArea;
			}
		}

		public Vector3 GetSlotPosition(InventoryItem item)
		{
			for (int i = 0; i < this.slots.Count; i++)
			{
				if (this.slots[i].item.ItemId == item)
				{
					return this.slots[i].rewardItem.transform.position;
				}
			}
			return Vector3.zero;
		}

		public void RemoveSlotItem(InventoryItem item)
		{
			for (int i = this.slots.Count - 1; i >= 0; i--)
			{
				if (this.slots[i].item.ItemId == item)
				{
					UnityEngine.Object.Destroy(this.slots[i].rewardItem);
				}
			}
		}

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
		}

		public void Initialize(InventoryManager inventoryManager, List<ItemAmount> rewards, bool overridePresetColumns = true)
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
				GameObject gameObject = this.ConstructSprite(inventoryManager, itemAmount.ItemId, itemAmount.Amount);
				gameObject.transform.parent = this.itemArea.transform;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.gameObject.name = num++.ToString();
				this.slots.Add(new RewardGrid.Slot
				{
					rewardItem = gameObject,
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

		private GameObject ConstructSprite(InventoryManager inventoryManager, InventoryItem item, int amount)
		{
			RewardItem rewardItem = UnityEngine.Object.Instantiate<RewardItem>(this.rewardItemPrefab);
			UIElement component = rewardItem.gameObject.GetComponent<UIElement>();
			component.SetSizeAndDoLayout(Vector2.one * this.PreferredItemWidth);
			InventoryItemMetaData metaData = inventoryManager.GetMetaData(item);
			rewardItem.icon.SpriteName = metaData.IconSpriteName;
			rewardItem.icon.KeepAspect = true;
			if (rewardItem.label != null)
			{
				rewardItem.label.text = metaData.FormattedQuantity(amount);
			}
			rewardItem.gameObject.SetLayerRecursively(base.gameObject.layer);
			return rewardItem.gameObject;
		}

		public void EmptySlots()
		{
			while (this.slots.Count > 0)
			{
				RewardGrid.Slot slot = this.slots[0];
				this.slots.RemoveAt(0);
				slot.rewardItem.transform.parent = null;
			}
		}

		[SerializeField]
		private UIGridLayout itemArea;

		[SerializeField]
		private RewardItem rewardItemPrefab;

		private readonly List<RewardGrid.Slot> slots = new List<RewardGrid.Slot>();

		private class Slot
		{
			public GameObject rewardItem;

			public ItemAmount item;
		}
	}
}
