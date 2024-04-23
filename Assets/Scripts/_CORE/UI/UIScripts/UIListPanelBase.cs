using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIListPanelBase : UIScrollablePanel
{
	public void BeginAdding()
	{
		if (this.scrollAxis == UIScrollablePanel.ScrollAxis.Both)
		{
			this.scrollAxis = UIScrollablePanel.ScrollAxis.Vertical;
		}
		base.DestroyAllContent();
		this.elapsed = Vector3.zero;
	}

	public abstract void AddToContent(UIElement e);

	public IEnumerator AnimateScrollToItem(int index, float duration, AnimationCurve curve = null)
	{
		if (index < 0 || index >= base.ScrollRoot.childCount)
		{
			throw new Exception("Trying to scroll to a cell that doesn't exist. Index: " + index);
		}
		Transform item = base.ScrollRoot.GetChild(index);
		Vector2 location = new Vector2(-item.localPosition.x, -item.localPosition.y);
		base.SetScrollAnimated(location, duration, curve);
		yield return base.WaitForScrollAnimation();
		yield break;
	}

	public void SetScrollToItem(int index)
	{
		if (index < 0 || index >= base.ScrollRoot.childCount)
		{
			throw new Exception("Trying to scroll to a cell that doesn't exist. Index: " + index);
		}
		Transform child = base.ScrollRoot.GetChild(index);
		Vector2 scroll = new Vector2(child.localPosition.x, -child.localPosition.y);
		base.SetScroll(scroll);
	}

	public IEnumerator AnimateCellToPosition(int from, int to, float scaleAnimationDuration = 0.5f, float translationAnimationDuration = 0.5f, AnimationCurve scaleCurve = null, AnimationCurve translateCurve = null)
	{
		ListPanelAnimationOptions options = new ListPanelAnimationOptions
		{
			ScaleAnimationDuration = scaleAnimationDuration,
			TranslateAnimationDuration = translationAnimationDuration,
			ScaleCurve = scaleCurve,
			TranslateCurve = translateCurve
		};
		yield return this.AnimateCellToPosition(from, to, options);
		yield break;
	}

	public IEnumerator AnimateCellToPosition(int from, int to, ListPanelAnimationOptions options)
	{
		if (from < 0 || from >= base.ScrollRoot.childCount || to < 0 || to >= base.ScrollRoot.childCount)
		{
			throw new Exception(string.Concat(new object[]
			{
				"Trying to animate a cell that doesn't exist. From: ",
				from,
				" to: ",
				to
			}));
		}
		Transform fromCell = base.ScrollRoot.GetChild(from);
		if (!fromCell.gameObject.activeSelf)
		{
			yield return this.AnimateScrollToItem(from, options.AutoScrollDuration, options.AutoScrollCurve);
		}
		Transform toCell = base.ScrollRoot.GetChild(to);
		UIElement fromElement = fromCell.GetComponent<UIElement>();
		bool scrollNeeded = !toCell.gameObject.activeSelf;
		fromCell.transform.localPosition = new Vector3(fromCell.localPosition.x, fromCell.localPosition.y, options.MovingItemZ);
		fromCell.transform.SetParent(base.transform);
		if (options.ScaleMovingItem)
		{
			yield return FiberAnimation.ScaleTransform(fromCell, fromCell.localScale, fromCell.localScale + new Vector3(0.2f, 0.2f, 0.2f), options.ScaleCurve, options.ScaleAnimationDuration);
			yield return FiberHelper.Wait(options.PauseDuration, (FiberHelper.WaitFlag)0);
		}
		List<IEnumerator> cellAnimations = new List<IEnumerator>();
		Vector3 destOffset = (to <= from) ? new Vector3(0f, -fromElement.Size.y, 0f) : new Vector3(0f, fromElement.Size.y, 0f);
		int start = (to <= from) ? to : from;
		int end = (to <= from) ? from : to;
		for (int i = start; i < end; i++)
		{
			UIElement component = base.ScrollRoot.GetChild(i).GetComponent<UIElement>();
			Vector3 destPosition = component.transform.localPosition + destOffset;
			IEnumerator item = FiberAnimation.MoveLocalTransform(component.transform, component.transform.localPosition, destPosition, options.ListContentTranslateCurve, options.TranslateAnimationDuration);
			cellAnimations.Add(item);
		}
		cellAnimations.Add(this.ForceUpdateCellVisibility(options.TranslateAnimationDuration));
		yield return FiberHelper.RunParallel(cellAnimations);
		yield return FiberHelper.Wait(options.PauseDuration, (FiberHelper.WaitFlag)0);
		if (scrollNeeded)
		{
			yield return this.AnimateScrollToItem(Mathf.Max(to - 1, 0), options.AutoScrollDuration, options.AutoScrollCurve);
		}
		Vector3 toPosition;
		if (to > from)
		{
			toPosition = new Vector3(toCell.position.x, toCell.position.y - fromElement.Size.y, fromCell.position.z);
		}
		else
		{
			toPosition = new Vector3(toCell.position.x, toCell.position.y + fromElement.Size.y, fromCell.position.z);
		}
		yield return FiberAnimation.MoveTransform(fromCell, fromCell.position, toPosition, options.TranslateCurve, options.TranslateAnimationDuration);
		yield return FiberHelper.Wait(options.PauseDuration, (FiberHelper.WaitFlag)0);
		fromCell.transform.SetParent(base.ScrollRoot);
		if (options.ScaleMovingItem)
		{
			yield return FiberAnimation.ScaleTransform(fromCell, fromCell.localScale, Vector3.one, options.ScaleCurve, options.ScaleAnimationDuration);
		}
		fromCell.transform.position = new Vector3(fromCell.position.x, fromCell.position.y, toCell.position.z);
		yield break;
	}

	private IEnumerator ForceUpdateCellVisibility(float duration)
	{
		while (duration > 0f)
		{
			base.UpdateCellVisibility(false);
			duration -= Time.deltaTime;
			yield return null;
		}
		yield break;
	}

	public void EndAdding()
	{
		base.CalculateContentSize();
		if (this.startFromTop)
		{
			base.SetScroll(-base.TotalContentSize);
		}
		else
		{
			base.SetScroll(Vector2.zero);
		}
	}

	public float cellZPosition;

	protected Vector3 elapsed;

	public float paddingAmount;
}
