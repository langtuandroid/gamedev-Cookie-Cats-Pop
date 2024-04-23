using System;
using UnityEngine;

[RequireComponent(typeof(UIResourceQuad))]
public class UIResourceQuadTinter : MonoBehaviour
{
	protected void OnEnable()
	{
		UIResourceQuad component = base.GetComponent<UIResourceQuad>();
		component.Color = SingletonAsset<LevelVisuals>.Instance.GameUITintColor;
	}
}
