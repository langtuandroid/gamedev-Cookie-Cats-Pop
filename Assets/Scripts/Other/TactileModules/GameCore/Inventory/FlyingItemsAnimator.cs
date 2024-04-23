using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Fibers;
using UnityEngine;

namespace TactileModules.GameCore.Inventory
{
	public class FlyingItemsAnimator : IFlyingItemsAnimator
	{
		public FlyingItemsAnimator(string spriteName, float size, int objectLayer)
		{
			this.itemPool = new List<GameObject>();
			for (int i = 0; i < 5; i++)
			{
				GameObject gameObject = this.CreateItem(spriteName);
				gameObject.transform.localScale = new Vector3(size, size, 1f);
				gameObject.SetLayerRecursively(objectLayer);
				gameObject.SetActive(false);
				this.itemPool.Add(gameObject);
			}
			this.curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
		}

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<Transform> ItemCreated;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<Transform> ItemDestroyed;

		public float Progress { get; private set; }

		public IEnumerator AnimateFlyingItems(InventoryItem inventoryItem, int layer, Vector3 sourcePos, Vector3 destPos, int visibleItems)
		{
			visibleItems = Mathf.Clamp(visibleItems, 1, 5);
			if (inventoryItem == "UnlimitedLives")
			{
				visibleItems = 1;
			}
			IEnumerator[] itemAnimations = new IEnumerator[visibleItems];
			float duration = (this.curve == null) ? 0.5f : this.curve.Duration();
			float deltaDelay = duration / (float)visibleItems * 0.5f;
			for (int i = 0; i < itemAnimations.Length; i++)
			{
				float delay = deltaDelay * (float)i;
				itemAnimations[i] = this.SpawnAndMoveSprite(i, sourcePos, destPos, this.curve, duration, delay, (float)(i + 1) / (float)itemAnimations.Length);
			}
			yield return FiberHelper.RunParallel(itemAnimations);
			yield break;
		}

		private IEnumerator SpawnAndMoveSprite(int index, Vector3 sourcePos, Vector3 destPos, AnimationCurve curve, float duration, float delay, float targetProgress)
		{
			if (delay > 0f)
			{
				yield return FiberHelper.Wait(delay, (FiberHelper.WaitFlag)0);
			}
			GameObject gameObject = this.itemPool[index];
			yield return new Fiber.OnExit(delegate()
			{
				if (this.ItemDestroyed != null)
				{
					this.ItemDestroyed(gameObject.transform);
				}
				UnityEngine.Object.Destroy(gameObject);
				this.Progress = targetProgress;
			});
			gameObject.SetActive(true);
			if (this.ItemCreated != null)
			{
				this.ItemCreated(gameObject.transform);
			}
			yield return FiberAnimation.MoveTransform(gameObject.transform, sourcePos, destPos, curve, duration);
			yield break;
		}

		private Vector3 VectorBezierInterpolation(Vector3 from, Vector3 to, float weight, float direction, float t)
		{
			Vector3 tangent = this.GetTangent(from, to, weight, direction);
			return this.CalculateBezierPoint(from, from + tangent, to + tangent, to, this.curve.Evaluate(t));
		}

		private Vector3 GetTangent(Vector3 from, Vector3 to, float weight, float direction)
		{
			return Vector3.Cross((to - from).normalized, Vector3.forward).normalized * weight * direction;
		}

		private Vector3 CalculateBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
		{
			float num = 1f - t;
			float num2 = t * t;
			float num3 = num * num;
			float d = num3 * num;
			float d2 = num2 * t;
			Vector3 a = d * p0;
			a += 3f * num3 * t * p1;
			a += 3f * num * num2 * p2;
			return a + d2 * p3;
		}

		private GameObject CreateItem(string spriteName)
		{
			GameObject gameObject = new GameObject("flyingSprite");
			UISprite uisprite = gameObject.AddComponent<UISprite>();
			uisprite.Atlas = UIProjectSettings.Get().defaultAtlas;
			uisprite.SpriteName = spriteName;
			uisprite.KeepAspect = true;
			return gameObject;
		}

		private const int MAX_ITEMS = 5;

		private readonly List<GameObject> itemPool;

		private readonly AnimationCurve curve;
	}
}
