using System;
using UnityEngine;

[RequireComponent(typeof(SkeletonAnimation))]
public class UISpineTinter : MonoBehaviour
{
	protected void OnEnable()
	{
		SkeletonAnimation component = base.GetComponent<SkeletonAnimation>();
		component.skeleton.SetColor(SingletonAsset<LevelVisuals>.Instance.GameSpineTintColor);
	}
}
