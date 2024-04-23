using System;
using UnityEngine;

public static class UIElementExtensions
{
	public static UIElement GetElement(this GameObject obj)
	{
		return obj.GetComponent<UIElement>();
	}

	public static Vector2 GetElementSize(this GameObject obj)
	{
		UIElement component = obj.GetComponent<UIElement>();
		return (!(component != null)) ? Vector2.zero : component.Size;
	}

	public static UIElement GetElement(this Component component)
	{
		return component.GetComponent<UIElement>();
	}

	public static Vector2 GetElementSize(this Component component)
	{
		UIElement component2 = component.GetComponent<UIElement>();
		return (!(component2 != null)) ? Vector2.zero : component2.Size;
	}
}
