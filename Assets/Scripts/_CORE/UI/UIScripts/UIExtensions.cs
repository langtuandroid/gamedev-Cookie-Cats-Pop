using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIExtensions
{
	public static void SortByDistance(this RaycastHit[] hits)
	{
		Array.Sort<RaycastHit>(hits, delegate(RaycastHit x, RaycastHit y)
		{
			if (x.distance < y.distance)
			{
				return -1;
			}
			if (x.distance > y.distance)
			{
				return 1;
			}
			return 0;
		});
	}

	public static Vector2 TopLeft(this Rect r)
	{
		return new Vector2(r.x, r.y + r.height);
	}

	public static Vector2 BottomRight(this Rect r)
	{
		return new Vector2(r.x + r.width, r.y);
	}

	public static Vector2 TopRight(this Rect r)
	{
		return new Vector2(r.x + r.width, r.y + r.height);
	}

	public static Vector2 BottomLeft(this Rect r)
	{
		return new Vector2(r.x, r.y);
	}

	public static Vector2 Size(this Rect r)
	{
		return new Vector2(r.width, r.height);
	}

	public static float Aspect(this Vector2 v)
	{
		if (v.y != 0f)
		{
			return v.x / v.y;
		}
		return 0f;
	}

	public static Vector2 GetNormal(this Vector2 v)
	{
		Vector2 vector = new Vector2(-v.y, v.x);
		return vector.normalized;
	}

	public static string GetHierarchyPath(this GameObject obj)
	{
		string text = obj.name;
		while (obj.transform.parent != null)
		{
			obj = obj.transform.parent.gameObject;
			text = obj.name + "/" + text;
		}
		return "\"" + text + "\"";
	}

	public static void SetLayerRecursively(this GameObject go, int layer)
	{
		go.layer = layer;
		Transform transform = go.transform;
		int i = 0;
		int childCount = transform.childCount;
		while (i < childCount)
		{
			Transform child = transform.GetChild(i);
			child.gameObject.SetLayerRecursively(layer);
			i++;
		}
	}

	public static void SetHideFlagsRecursively(this GameObject root, HideFlags flags)
	{
		root.hideFlags = flags;
		root.transform.hideFlags = flags;
		IEnumerator enumerator = root.transform.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				Transform transform = (Transform)obj;
				transform.gameObject.SetHideFlagsRecursively(flags);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}

	public static void MoveUp<T>(this List<T> list, T objToMove)
	{
		int num = list.IndexOf(objToMove);
		if (num <= 0)
		{
			return;
		}
		list.RemoveAt(num);
		list.Insert(num - 1, objToMove);
	}

	public static void MoveDown<T>(this List<T> list, T objToMove)
	{
		int num = list.IndexOf(objToMove);
		if (num >= list.Count - 1)
		{
			return;
		}
		list.RemoveAt(num);
		list.Insert(num + 1, objToMove);
	}

	public static string ToConcatenatedString(this object[] parameters, int maxLength = 0)
	{
		if (parameters != null && parameters.Length > 0)
		{
			string text = string.Empty;
			for (int i = 0; i < parameters.Length; i++)
			{
				text = text + parameters[i] + ",";
			}
			text = text.Substring(0, text.Length - 1);
			if (maxLength > 0 && text.Length > maxLength)
			{
				text = text.Substring(0, maxLength - 4) + "...";
			}
			return text;
		}
		return string.Empty;
	}
}
