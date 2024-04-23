using System;
using UnityEngine;

public static class UIButtonExtensions
{
	public static UIButton GetButton(this Component self)
	{
		return self.GetComponent<UIButton>();
	}
}
