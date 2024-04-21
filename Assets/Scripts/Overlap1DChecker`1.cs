using System;
using System.Collections.Generic;
using UnityEngine;

public class Overlap1DChecker<T>
{
	public Overlap1DChecker(List<T> sourceList, Func<T, Vector2> spanFunction)
	{
		this.elements = new List<Overlap1DChecker<T>.VisibilityInfo>();
		foreach (T element in sourceList)
		{
			this.elements.Add(new Overlap1DChecker<T>.VisibilityInfo
			{
				element = element
			});
		}
		this.leftEdges = new ThresholdChecker<Overlap1DChecker<T>.VisibilityInfo>(this.elements, false, (Overlap1DChecker<T>.VisibilityInfo e) => spanFunction(e.element).x);
		this.rightEdges = new ThresholdChecker<Overlap1DChecker<T>.VisibilityInfo>(this.elements, true, (Overlap1DChecker<T>.VisibilityInfo e) => spanFunction(e.element).y);
	}

	public void Clear()
	{
		this.leftEdges.Clear();
		this.rightEdges.Clear();
	}

	public bool Check(Vector2 insideThisSpan, Action<T, bool> isInsideCallback)
	{
		bool flag = this.leftEdges.Check(insideThisSpan.y, delegate(Overlap1DChecker<T>.VisibilityInfo e, bool isInside)
		{
			e.leftInside = isInside;
			isInsideCallback(e.element, e.IsInside);
		});
		return flag | this.rightEdges.Check(insideThisSpan.x, delegate(Overlap1DChecker<T>.VisibilityInfo e, bool isInside)
		{
			e.rightInside = isInside;
			isInsideCallback(e.element, e.IsInside);
		});
	}

	private ThresholdChecker<Overlap1DChecker<T>.VisibilityInfo> leftEdges;

	private ThresholdChecker<Overlap1DChecker<T>.VisibilityInfo> rightEdges;

	private List<Overlap1DChecker<T>.VisibilityInfo> elements;

	private class VisibilityInfo
	{
		public bool IsInside
		{
			get
			{
				return this.leftInside && this.rightInside;
			}
		}

		public T element;

		public bool leftInside;

		public bool rightInside;
	}
}
