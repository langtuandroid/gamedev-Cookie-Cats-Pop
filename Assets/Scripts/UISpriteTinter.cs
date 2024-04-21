using System;
using UnityEngine;

[RequireComponent(typeof(UISprite))]
public class UISpriteTinter : MonoBehaviour
{
	protected void OnEnable()
	{
		UISprite component = base.GetComponent<UISprite>();
		component.Color = SingletonAsset<LevelVisuals>.Instance.GameUITintColor;
	}
}
