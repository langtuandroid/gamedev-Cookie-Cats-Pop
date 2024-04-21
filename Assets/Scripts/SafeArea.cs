using System;
using UnityEngine;

public static class SafeArea
{
	public static Rect GetSafeArea()
	{
		float x = 0f;
		float y = 0f;
		float width = (float)Screen.width;
		float height = (float)Screen.height;
		return new Rect(x, y, width, height);
	}

	private static void EmulateSafeArea(out float x, out float y, out float w, out float h)
	{
		float num = (float)Screen.width / (float)Screen.height;
		if (num < 1f)
		{
			float num2 = (float)Screen.height / 2436f;
			x = 0f * num2;
			y = 102f * num2;
			w = (float)Screen.width - 0f * num2;
			h = (float)Screen.height - 234f * num2;
		}
		else
		{
			float num3 = (float)Screen.width / 2436f;
			x = 132f * num3;
			y = 63f * num3;
			w = (float)Screen.width - 264f * num3;
			h = (float)Screen.height - 63f * num3;
		}
	}
}
