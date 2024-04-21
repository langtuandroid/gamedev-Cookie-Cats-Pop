using System;
using System.Collections;
using UnityEngine;

public abstract class UIViewLayerAnimation : ScriptableObject
{
	public virtual IEnumerator AnimateIn(IUIView view)
	{
		yield break;
	}

	public virtual IEnumerator AnimateOut(IUIView view)
	{
		yield break;
	}
}
