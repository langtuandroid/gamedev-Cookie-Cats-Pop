using System;
using System.Collections.Generic;
using UnityEngine;

public class Overlap2DChecker<T>
{
	public Overlap2DChecker(List<T> sourceList, Func<T, Rect> rectFunction)
	{
		this.elements = new List<Overlap2DChecker<T>.VisibilityInfo>();
		foreach (T element in sourceList)
		{
			this.elements.Add(new Overlap2DChecker<T>.VisibilityInfo
			{
				element = element
			});
		}
		this.topEdges = new ThresholdChecker<Overlap2DChecker<T>.VisibilityInfo>(this.elements, true, (Overlap2DChecker<T>.VisibilityInfo e) => rectFunction(e.element).TopLeft().y);
		this.leftEdges = new ThresholdChecker<Overlap2DChecker<T>.VisibilityInfo>(this.elements, false, (Overlap2DChecker<T>.VisibilityInfo e) => rectFunction(e.element).TopLeft().x);
		this.rightEdges = new ThresholdChecker<Overlap2DChecker<T>.VisibilityInfo>(this.elements, true, (Overlap2DChecker<T>.VisibilityInfo e) => rectFunction(e.element).BottomRight().x);
		this.bottomEdges = new ThresholdChecker<Overlap2DChecker<T>.VisibilityInfo>(this.elements, false, (Overlap2DChecker<T>.VisibilityInfo e) => rectFunction(e.element).BottomRight().y);
	}

	public void StartJump()
	{
		this.ignoreCallbacks = true;
		this.snapshot = this.TakeSnapshot();
	}

	public void EndJump(Action<T, bool> isInsideCallback)
	{
		for (int i = 0; i < this.elements.Count; i++)
		{
			Overlap2DChecker<T>.VisibilityInfo visibilityInfo = this.elements[i];
			Overlap2DChecker<T>.VisibilityInfo visibilityInfo2 = this.snapshot[i];
			if (visibilityInfo.IsInside != visibilityInfo2.IsInside)
			{
				isInsideCallback(visibilityInfo.element, visibilityInfo.IsInside);
			}
		}
		this.snapshot.Clear();
		this.ignoreCallbacks = false;
	}

	private List<Overlap2DChecker<T>.VisibilityInfo> TakeSnapshot()
	{
		List<Overlap2DChecker<T>.VisibilityInfo> list = new List<Overlap2DChecker<T>.VisibilityInfo>();
		foreach (Overlap2DChecker<T>.VisibilityInfo visibilityInfo in this.elements)
		{
			list.Add(new Overlap2DChecker<T>.VisibilityInfo
			{
				element = visibilityInfo.element,
				bottomInside = visibilityInfo.bottomInside,
				leftInside = visibilityInfo.leftInside,
				rightInside = visibilityInfo.rightInside,
				topInside = visibilityInfo.topInside
			});
		}
		return list;
	}

	public void Check(Rect insideThisRect, Action<T, bool> isInsideCallback)
	{
		bool flag;
		do
		{
			flag = this.bottomEdges.Check(insideThisRect.TopLeft().y, delegate(Overlap2DChecker<T>.VisibilityInfo e, bool isInside)
			{
				e.bottomInside = isInside;
				if (!this.ignoreCallbacks)
				{
					isInsideCallback(e.element, e.IsInside);
				}
			});
			flag |= this.topEdges.Check(insideThisRect.BottomRight().y, delegate(Overlap2DChecker<T>.VisibilityInfo e, bool isInside)
			{
				e.topInside = isInside;
				if (!this.ignoreCallbacks)
				{
					isInsideCallback(e.element, e.IsInside);
				}
			});
			flag |= this.leftEdges.Check(insideThisRect.BottomRight().x, delegate(Overlap2DChecker<T>.VisibilityInfo e, bool isInside)
			{
				e.leftInside = isInside;
				if (!this.ignoreCallbacks)
				{
					isInsideCallback(e.element, e.IsInside);
				}
			});
		}
		while (flag | this.rightEdges.Check(insideThisRect.TopLeft().x, delegate(Overlap2DChecker<T>.VisibilityInfo e, bool isInside)
		{
			e.rightInside = isInside;
			if (!this.ignoreCallbacks)
			{
				isInsideCallback(e.element, e.IsInside);
			}
		}));
	}

	private bool ignoreCallbacks;

	private List<Overlap2DChecker<T>.VisibilityInfo> snapshot;

	private ThresholdChecker<Overlap2DChecker<T>.VisibilityInfo> topEdges;

	private ThresholdChecker<Overlap2DChecker<T>.VisibilityInfo> leftEdges;

	private ThresholdChecker<Overlap2DChecker<T>.VisibilityInfo> rightEdges;

	private ThresholdChecker<Overlap2DChecker<T>.VisibilityInfo> bottomEdges;

	private List<Overlap2DChecker<T>.VisibilityInfo> elements;

	private class VisibilityInfo
	{
		public bool IsInside
		{
			get
			{
				return this.bottomInside && this.topInside && this.leftInside && this.rightInside;
			}
		}

		public T element;

		public bool topInside;

		public bool bottomInside;

		public bool leftInside;

		public bool rightInside;
	}
}
