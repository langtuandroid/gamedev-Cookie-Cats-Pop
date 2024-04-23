using System;
using UnityEngine;

public class UIFontStyle : ScriptableObject
{
	public bool italic;

	public bool gradient;

	public Color startColor = Color.white;

	public Color endColor = Color.yellow;

	public UIFontStyle.StyleEffect effect;

	public bool applyEffectsToEmbeddedSprites = true;

	public Color shadowColor = Color.black;

	public Vector2 shadowSize = new Vector2(1f, 1f);

	public Color strokeColor = Color.red;

	public Vector2 strokeSize = new Vector2(1f, 1f);

	public UIFontStyle.Sizing sizing;

	public float fontSize;

	public enum Sizing
	{
		None,
		LocalSpace,
		WorldSpace
	}

	public enum StyleEffect
	{
		None,
		Shadow,
		Outline,
		ShadowAndOutline
	}
}
