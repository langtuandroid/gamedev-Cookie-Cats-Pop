using System;
using UnityEngine;

public class ListPanelAnimationOptions
{
	public bool ScaleMovingItem = true;

	public float ScaleAnimationDuration = 0.5f;

	public float TranslateAnimationDuration = 0.5f;

	public float AutoScrollDuration = 0.4f;

	public float PauseDuration = 0.2f;

	public float MovingItemZ = -50f;

	public AnimationCurve ScaleCurve;

	public AnimationCurve TranslateCurve;

	public AnimationCurve AutoScrollCurve;

	public AnimationCurve ListContentTranslateCurve;
}
