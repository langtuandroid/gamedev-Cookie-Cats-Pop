using System;
using System.Collections.Generic;

public class ThresholdChecker<T>
{
	public ThresholdChecker(List<T> sourceList, bool reverse, Func<T, float> valueGetter)
	{
		ThresholdChecker<T> _0024this = this;
		this.valueGetter = valueGetter;
		this.reverse = ((!reverse) ? 1 : -1);
		this.sortedList = new List<T>(sourceList);
		this.sortedList.Sort(delegate(T a, T b)
		{
			float num = valueGetter(a);
			float num2 = valueGetter(b);
			if (num < num2)
			{
				return -1 * _0024this.reverse;
			}
			if (num > num2)
			{
				return _0024this.reverse;
			}
			return 0;
		});
	}

	public void Clear()
	{
		this.closestInsideIndex = -1;
	}

	public bool Check(float threshold, Action<T, bool> didChange)
	{
		if (this.closestInsideIndex >= 0)
		{
			float num = this.valueGetter(this.sortedList[this.closestInsideIndex]);
			if (num * (float)this.reverse > threshold * (float)this.reverse)
			{
				didChange(this.sortedList[this.closestInsideIndex], false);
				this.closestInsideIndex--;
				return true;
			}
		}
		int num2 = this.closestInsideIndex + 1;
		if (num2 < this.sortedList.Count)
		{
			float num3 = this.valueGetter(this.sortedList[num2]);
			if (num3 * (float)this.reverse < threshold * (float)this.reverse)
			{
				didChange(this.sortedList[num2], true);
				this.closestInsideIndex++;
				return true;
			}
		}
		return false;
	}

	private int closestInsideIndex = -1;

	private List<T> sortedList;

	private int reverse;

	private Func<T, float> valueGetter;
}
