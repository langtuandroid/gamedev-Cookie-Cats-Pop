using System;
using System.Collections;
using UnityEngine;

public interface IUIView
{
	object[] Parameters { get; }

	void LoadedFromPool(object[] parameters);

	void Close(object result);

	bool UseBottomViewZone { get; set; }

	bool IgnoreSafeArea { get; }

	Color BackgroundColor { get; }

	int PoolingAmount { get; set; }

	bool BlockOtherViews { get; set; }

	bool IsClosing { get; }

	bool IgnoreFocus { get; set; }

	Vector2 OriginalSize { get; }

	bool UseDefaultViewAnimator { get; set; }

	UIViewLayerAnimation OverrideLayerAnimation { get; set; }

	bool DisableViewAnalytics { get; }

	Action OnClosed { get; set; }

	object ClosingResult { get; }

	string name { get; }

	GameObject gameObject { get; }

	Transform transform { get; }

	bool ShowBackfillQuad { get; }

	UIElement GetElement();

	Vector2 GetElementSize();

	IEnumerator AnimateIn();

	IEnumerator AnimateOut();

	void ReleasedToPool();

	UIViewLayerAnimation GetViewLayerAnimation();

	Vector2 CalculateViewSizeForScreen(Vector2 screenSize);
}
