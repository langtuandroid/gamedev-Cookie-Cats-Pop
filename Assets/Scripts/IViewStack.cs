using System;
using UnityEngine;

public interface IViewStack
{
	int StackCount { get; }

	event Action<IUIView> OnViewStackPush;

	event Action<IUIView> OnViewStackPop;

	IUIView GetAtIndex(int index);

	int GetLayerIndexFromGameObjectLayer(GameObject gameObject);
}
