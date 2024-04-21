using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UIElement))]
public abstract class UILayout : MonoBehaviour
{
	public abstract void Layout();

	public virtual List<UIElement> GetChildElements(UILayout.QueryFlags flags = (UILayout.QueryFlags)0)
	{
		List<UIElement> list = new List<UIElement>();
		Transform transform = base.transform;
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			UIElement component = child.GetComponent<UIElement>();
			if (component != null)
			{
				if ((flags & UILayout.QueryFlags.NoDisabled) == (UILayout.QueryFlags)0 || component.gameObject.activeInHierarchy)
				{
					list.Add(component);
				}
			}
		}
		if ((flags & UILayout.QueryFlags.SortByName) != (UILayout.QueryFlags)0)
		{
			list.Sort((UIElement x, UIElement y) => x.name.CompareTo(y.name));
		}
		return list;
	}

	[Flags]
	public enum QueryFlags
	{
		SortByName = 1,
		NoDisabled = 2
	}
}
