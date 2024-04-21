using System;
using System.Collections;
using System.Collections.Generic;
using Tactile;
using TactileModules.PuzzleGame.SlidesAndLadders.Views;
using UnityEngine;

public class SlidesAndLaddersSlidesRewardAnimation : MonoBehaviour, ISlidesRewardAnimation
{
	public IEnumerator Animate(List<ItemAmount> rewards, MapStreamer mapStreamer, int preSlideIndex, int currentIndex, int chestIndex)
	{
		this.view.StartDimming(0.3f, 0f, 0.8f);
		yield return mapStreamer.FocusCameraBetweenTwoLevels(currentIndex, chestIndex, 0.3f, null);
		yield return FiberHelper.Wait(0.4f, (FiberHelper.WaitFlag)0);
		this.AddRewardsItem(rewards, mapStreamer);
		yield return this.AnimateRewardScale(0.5f);
		yield return FiberHelper.Wait(0.3f, (FiberHelper.WaitFlag)0);
		this.view.StartDimming(0.8f, 0.5f, 0f);
		yield return this.AnimateRewardsToEndPosition(mapStreamer, currentIndex, chestIndex);
		yield return FiberHelper.Wait(0.3f, (FiberHelper.WaitFlag)0);
		yield return mapStreamer.FocusCameraOnLevel(currentIndex, 0.3f, null);
		yield break;
	}

	private void AddRewardsItem(List<ItemAmount> rewards, MapStreamer mapStreamer)
	{
		for (int i = 0; i < rewards.Count; i++)
		{
			Vector2 position = this.TransformRewardToViewLocalPosition(mapStreamer, this.view.transform, this.view.RewardsStartPosition);
			this.rewardItems.Add(this.ConstructRewardItem(rewards[i].ItemId, rewards[i].Amount, position));
		}
	}

	private IEnumerator AnimateRewardScale(float duration)
	{
		float time = 0f;
		float timeScale = 1f / duration;
		while (time < duration)
		{
			float evaluationTime = time * timeScale;
			foreach (RewardItem rewardItem in this.rewardItems)
			{
				this.ScaleTransformBasedOnAnimationCurve(evaluationTime, this.scaleCurve, rewardItem.transform);
			}
			yield return FiberHelper.Wait(Time.deltaTime, (FiberHelper.WaitFlag)0);
			time += Time.deltaTime;
		}
		yield break;
	}

	private IEnumerator AnimateRewardsToEndPosition(MapStreamer mapStreamer, int currentIndex, int chestIndex)
	{
		Vector2 end = this.TransformLevelToViewLocalPosition(mapStreamer, this.view.transform, chestIndex);
		for (int i = 0; i < this.rewardItems.Count; i++)
		{
			yield return this.AnimateToEndPosition(this.rewardItems[i].GetElement(), 0.3f, end);
			this.rewardItems[i].gameObject.SetActive(false);
		}
		yield break;
	}

	private IEnumerator AnimateToEndPosition(UIElement item, float duration, Vector2 endPosition)
	{
		Vector2 direction = (item.LocalPosition.x >= endPosition.x) ? Vector2.left : Vector2.right;
		yield return this.AnimateToPosition(item, duration, endPosition, this.toEnd, direction, this.directionCurve, this.directionWeight, this.scaleTo);
		yield break;
	}

	public IEnumerator AnimateToPosition(UIElement animationItem, float duration, Vector2 endPosition, AnimationCurve movementCurve, Vector2 direction, AnimationCurve directionCurve = null, float directionWeight = 0f, AnimationCurve scaleCurve = null)
	{
		float time = 0f;
		float timeScale = 1f / duration;
		Vector2 start = animationItem.GetElement().LocalPosition;
		Vector2 endDirection = endPosition - start;
		while (time < duration)
		{
			float evaluationTime = time * timeScale;
			animationItem.GetElement().LocalPosition = start + endDirection * movementCurve.Evaluate(evaluationTime);
			if (directionCurve != null)
			{
				animationItem.GetElement().LocalPosition += this.DirectionBasedOnAnimationCurve(evaluationTime, directionCurve, direction, directionWeight);
			}
			if (scaleCurve != null)
			{
				this.ScaleTransformBasedOnAnimationCurve(evaluationTime, scaleCurve, animationItem.transform);
			}
			time += Time.deltaTime;
			yield return FiberHelper.Wait(Time.deltaTime, (FiberHelper.WaitFlag)0);
		}
		yield break;
	}

	private Vector2 DirectionBasedOnAnimationCurve(float evaluationTime, AnimationCurve directionCurve, Vector2 direction, float weight)
	{
		return direction * directionCurve.Evaluate(evaluationTime) * weight;
	}

	private void ScaleTransformBasedOnAnimationCurve(float evaluationTime, AnimationCurve scaleCurve, Transform item)
	{
		item.localScale = scaleCurve.Evaluate(evaluationTime) * Vector3.one;
	}

	public Vector2 TransformLevelToViewLocalPosition(MapStreamer mapStreamer, Transform localView, int index)
	{
		Vector3 position;
		mapStreamer.TryGetDotPosition(index, out position);
		Vector3 position2 = mapStreamer.SpawnedContentRoot.TransformPoint(position);
		return localView.InverseTransformPoint(position2);
	}

	public Vector2 TransformRewardToViewLocalPosition(MapStreamer mapStreamer, Transform localView, Vector3 rewardLocalPosition)
	{
		Vector3 position = mapStreamer.SpawnedContentRoot.TransformPoint(rewardLocalPosition);
		return localView.InverseTransformPoint(position);
	}

	private RewardItem ConstructRewardItem(InventoryItem item, int amount, Vector2 position)
	{
		RewardItem rewardItem = UnityEngine.Object.Instantiate<RewardItem>(this.rewardPrefab);
		rewardItem.transform.parent = this.view.transform;
		rewardItem.transform.localPosition = new Vector3(position.x, position.y, -100f);
		rewardItem.gameObject.name = item.ToString();
		if (rewardItem.icon == null)
		{
			rewardItem.icon = new GameObject("spr").AddComponent<UISprite>();
			UIElement uielement = rewardItem.icon.gameObject.AddComponent<UIElement>();
			uielement.SetSizeAndDoLayout(Vector2.one * this.rewardItemWidth);
		}
		InventoryItemMetaData metaData = InventoryManager.Instance.GetMetaData(item);
		rewardItem.icon.SpriteName = metaData.IconSpriteName;
		rewardItem.icon.KeepAspect = true;
		rewardItem.icon.Atlas = UIProjectSettings.Get().defaultAtlas;
		if (rewardItem.label != null)
		{
			rewardItem.label.text = metaData.FormattedQuantity(amount);
		}
		rewardItem.gameObject.SetLayerRecursively(this.view.gameObject.layer);
		return rewardItem;
	}

	public void PlaySound()
	{
		SingletonAsset<UISetup>.Instance.itemPutIntoInventory.Play();
	}

	[SerializeField]
	private SlidesAndLaddersSlidesRewardView view;

	[SerializeField]
	private RewardItem rewardPrefab;

	[SerializeField]
	private float rewardItemWidth;

	[Header("Animation curves")]
	[SerializeField]
	private AnimationCurve toCenter;

	[SerializeField]
	private AnimationCurve toEnd;

	[SerializeField]
	private AnimationCurve scaleTo;

	[SerializeField]
	private AnimationCurve directionCurve;

	[SerializeField]
	private AnimationCurve scaleCurve;

	[SerializeField]
	private float directionWeight;

	private List<RewardItem> rewardItems = new List<RewardItem>();
}
