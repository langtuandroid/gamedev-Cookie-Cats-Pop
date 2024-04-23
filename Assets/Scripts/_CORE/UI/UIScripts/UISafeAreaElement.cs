using System;
using UnityEngine;

public class UISafeAreaElement : UIElement
{
	public void SetScreenAndSafeAreaRects(Rect screenRect, Rect safeAreaRect)
	{
		this.ScreenRect = screenRect;
		this.SafeAreaRect = safeAreaRect;
	}

	protected override void Awake()
	{
		Vector2 size = new Vector2((float)Screen.width, (float)Screen.height);
		this.ScreenRect = new Rect(Vector2.zero, size);
		this.SafeAreaRect = new Rect(Vector2.zero, size);
		base.Awake();
	}

	protected override void SetNewSize(Vector2 unused, bool markForLayout = true)
	{
		UIView componentInParent = base.GetComponentInParent<UIView>();
		Vector2 vector = componentInParent.CalculateViewSizeForScreen(this.ScreenRect.size);
		Vector2 b = new Vector2(vector.x / this.ScreenRect.size.x, vector.y / this.ScreenRect.size.y);
		Vector2 newSize = Vector2.Scale(this.SafeAreaRect.size, b);
		Vector2 v = Vector2.Scale(this.SafeAreaRect.center - this.ScreenRect.center, b);
		base.transform.localPosition = v;
		base.SetNewSize(newSize, markForLayout);
	}

	private Rect ScreenRect;

	private Rect SafeAreaRect;
}
