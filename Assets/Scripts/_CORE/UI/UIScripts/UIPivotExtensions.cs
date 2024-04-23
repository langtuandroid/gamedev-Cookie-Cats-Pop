using System;

public static class UIPivotExtensions
{
	public static VAlignment GetVAlignment(this UIPivot p)
	{
		if (p == UIPivot.Top || p == UIPivot.TopLeft || p == UIPivot.TopRight)
		{
			return VAlignment.Top;
		}
		if (p == UIPivot.Center || p == UIPivot.Left || p == UIPivot.Right)
		{
			return VAlignment.Center;
		}
		return VAlignment.Bottom;
	}

	public static Alignment GetAlignment(this UIPivot p)
	{
		if (p == UIPivot.Right || p == UIPivot.TopRight || p == UIPivot.BottomRight)
		{
			return Alignment.Right;
		}
		if (p == UIPivot.Top || p == UIPivot.Bottom || p == UIPivot.Center)
		{
			return Alignment.Center;
		}
		return Alignment.Left;
	}
}
