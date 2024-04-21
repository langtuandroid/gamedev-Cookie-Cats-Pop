using System;
using UnityEngine;

public class ExtensibleVisual<T> : MonoBehaviour, IExtensibleVisual
{
	protected T Extension
	{
		get
		{
			return base.GetComponent<T>();
		}
	}
}
