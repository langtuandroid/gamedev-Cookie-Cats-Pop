using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Fibers;
using Tactile;
using TactileModules.GameCore.Audio;
using TactileModules.GameCore.Rewards;
using UnityEngine;

namespace TactileModules.GameCore.Inventory
{
	public class VisualInventory : IVisualInventory
	{
		public VisualInventory(IRewardAreaModel rewardAreaModel, AudioDatabaseInjector audio, InventoryManager inventoryManager)
		{
			this.rewardAreaModel = rewardAreaModel;
			this.inventoryManager = inventoryManager;
			this.audio = audio;
			inventoryManager.InventoryChanged += this.InventoryChanged;
		}

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<InventoryItem> VisualInventoryChanged;

		public IRewardAreaModel RewardAreaModel
		{
			get
			{
				return this.rewardAreaModel;
			}
		}

		public InventoryManager InventoryManager
		{
			get
			{
				return this.inventoryManager;
			}
		}

		public int GetVisualAmount(InventoryItem item)
		{
			if (this.animatorRefCounter == 0)
			{
				return this.inventoryManager.GetAmount(item);
			}
			int result;
			if (!this.inventorySnapshot.TryGetValue(item, out result))
			{
				result = 0;
			}
			return result;
		}

		public IInventoryItemAnimator CreateAnimator()
		{
			return new VisualInventory.Animator(this, null);
		}

		public IInventoryItemAnimator CreateAnimator(IFlyingItemsAnimator customAnimator)
		{
			return new VisualInventory.Animator(this, customAnimator);
		}

		public IInventoryItemAnimator CreateAnimator(Func<InventoryItem, int, bool> filterFunction)
		{
			return new VisualInventory.Animator(this, null, filterFunction);
		}

		public IInventoryItemAnimator CreateAnimator(Func<InventoryItem, int, bool> filterFunction, IFlyingItemsAnimator customAnimator)
		{
			return new VisualInventory.Animator(this, customAnimator, filterFunction);
		}

		private void ChangeVisualAmount(InventoryItem item, int amount)
		{
			if (!this.inventorySnapshot.ContainsKey(item))
			{
				this.inventorySnapshot.Add(item, amount);
			}
			else
			{
				this.inventorySnapshot[item] = amount;
			}
			if (this.VisualInventoryChanged != null)
			{
				this.VisualInventoryChanged(item);
			}
		}

		private void BeginAnimations()
		{
			if (this.animatorRefCounter == 0)
			{
				this.inventoryManager.InventoryChanged -= this.InventoryChanged;
				this.inventorySnapshot = this.CreateSnapshot();
			}
			this.animatorRefCounter++;
		}

		private void EndAnimations()
		{
			this.animatorRefCounter--;
			if (this.animatorRefCounter == 0)
			{
				this.inventoryManager.InventoryChanged += this.InventoryChanged;
			}
		}

		private Dictionary<InventoryItem, int> CreateSnapshot()
		{
			Dictionary<InventoryItem, int> dictionary = new Dictionary<InventoryItem, int>();
			foreach (KeyValuePair<string, int> keyValuePair in UserSettingsManager.Get<InventoryManager.PersistableState>().items)
			{
				dictionary.Add(keyValuePair.Key, keyValuePair.Value);
			}
			return dictionary;
		}

		private void InventoryChanged(InventoryManager.ItemChangeInfo info)
		{
			if (this.VisualInventoryChanged != null)
			{
				this.VisualInventoryChanged(info.Item);
			}
		}

		private readonly IRewardAreaModel rewardAreaModel;

		private readonly AudioDatabaseInjector audio;

		private readonly InventoryManager inventoryManager;

		private int animatorRefCounter;

		private Dictionary<InventoryItem, int> inventorySnapshot;

		private class Animator : IInventoryItemAnimator, IDisposable
		{
			public Animator(VisualInventory visualInventory, IFlyingItemsAnimator animator) : this(visualInventory, animator, (InventoryItem item, int i) => true)
			{
			}

			public Animator(VisualInventory visualInventory, IFlyingItemsAnimator animator, Func<InventoryItem, int, bool> filterFunction)
			{
				this.visualInventory = visualInventory;
				this.animator = animator;
				this.filterFunction = filterFunction;
				this.visualInventory.BeginAnimations();
				this.inventoryManager.InventoryChanged += this.InventoryChanged;
				this.animations = new List<IEnumerator>();
			}

			//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			public event Action<Transform> ItemCreated;

			//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			public event Action<Transform> ItemDestroyed;

			void IDisposable.Dispose()
			{
				this.visualInventory.EndAnimations();
				this.inventoryManager.InventoryChanged -= this.InventoryChanged;
			}

			private InventoryManager inventoryManager
			{
				get
				{
					return this.visualInventory.inventoryManager;
				}
			}

			public IEnumerator Animate(Vector3 startPoint)
			{
				return this.Animate((InventoryItem item) => startPoint);
			}

			public IEnumerator Animate(Func<InventoryItem, Vector3> startPointFunction)
			{
				this.startPointFunction = startPointFunction;
				yield return FiberHelper.RunSerial(this.animations);
				yield break;
			}

			private void InventoryChanged(InventoryManager.ItemChangeInfo info)
			{
				int amount = this.inventoryManager.GetAmount(info.Item);
				if (this.filterFunction(info.Item, info.ChangeByAmount))
				{
					this.animations.Add(this.AnimateInventoryItem(info.Item, amount - info.ChangeByAmount, amount, delegate(int a)
					{
						this.visualInventory.ChangeVisualAmount(info.Item, a);
					}));
				}
			}

			public IEnumerator AnimateInventoryItem(Vector3 startPoint, InventoryItem item, int startAmount, int endAmount, Action<int> amountChanged)
			{
				this.startPointFunction = ((InventoryItem i) => startPoint);
				return this.AnimateInventoryItem(item, startAmount, endAmount, amountChanged);
			}

			private IEnumerator AnimateInventoryItem(InventoryItem item, int startAmount, int endAmount, Action<int> amountChanged)
			{
				UICamera.DisableInput();
				yield return new Fiber.OnExit(delegate()
				{
					UICamera.EnableInput();
					amountChanged(endAmount);
				});
				this.PlaySound(item, startAmount, endAmount, true);
				InventoryCollectTarget collectTarget = this.visualInventory.RewardAreaModel.GetTargetPosition(item);
				if (collectTarget == null)
				{
					yield break;
				}
				Vector3 startPoint = (!(this.rewardGrid != null)) ? this.startPointFunction(item) : this.rewardGrid.GetSlotPosition(item);
				InventoryItemMetaData meta = this.visualInventory.inventoryManager.GetMetaData(item);
				int layer = collectTarget.gameObject.layer;
				IFlyingItemsAnimator flyingItemsAnimator = (this.animator == null) ? new FlyingItemsAnimator(meta.IconSpriteName, 80f, layer) : this.animator;
				flyingItemsAnimator.ItemCreated += delegate(Transform t)
				{
					if (this.ItemCreated != null)
					{
						this.ItemCreated(t);
					}
				};
				flyingItemsAnimator.ItemDestroyed += delegate(Transform t)
				{
					if (this.ItemDestroyed != null)
					{
						this.ItemDestroyed(t);
					}
				};
				Vector3 sourcePosition = (startAmount >= endAmount) ? collectTarget.transform.position : startPoint;
				Vector3 destPosition = (startAmount >= endAmount) ? startPoint : collectTarget.transform.position;
				float minZ = Mathf.Min(sourcePosition.z, destPosition.z) - 20f;
				sourcePosition.z = minZ;
				destPosition.z = minZ;
				Fiber fiber = new Fiber(flyingItemsAnimator.AnimateFlyingItems(item, layer, sourcePosition, destPosition, Math.Abs(endAmount - startAmount)), FiberBucket.Manual);
				while (fiber.Step())
				{
					amountChanged(Mathf.FloorToInt(Mathf.Lerp((float)startAmount, (float)endAmount, flyingItemsAnimator.Progress)));
					yield return null;
				}
				this.PlaySound(item, startAmount, endAmount, false);
				yield break;
			}

			private void PlaySound(InventoryItem item, int startAmount, int endAmount, bool atAnimationStart)
			{
				AudioDatabase.InventoryItemSoundType type;
				if (atAnimationStart)
				{
					type = ((endAmount >= startAmount) ? AudioDatabase.InventoryItemSoundType.ReceivedItemStarted : AudioDatabase.InventoryItemSoundType.UsedItemStarted);
				}
				else
				{
					type = ((endAmount >= startAmount) ? AudioDatabase.InventoryItemSoundType.ReceivedItemCompleted : AudioDatabase.InventoryItemSoundType.UsedItemCompleted);
				}
				AudioDatabase audioDatabase = this.visualInventory.audio.Assets.AudioDatabase;
				if (audioDatabase != null)
				{
					ISoundDefinition inventoryAudio = audioDatabase.GetInventoryAudio(item, type);
					inventoryAudio.PlaySound();
				}
			}

			private readonly VisualInventory visualInventory;

			private readonly List<IEnumerator> animations;

			private readonly IFlyingItemsAnimator animator;

			private bool isActive;

			private Func<InventoryItem, Vector3> startPointFunction;

			private Func<InventoryItem, int, bool> filterFunction;

			private TactileModules.GameCore.Rewards.RewardGrid rewardGrid;
		}
	}
}
