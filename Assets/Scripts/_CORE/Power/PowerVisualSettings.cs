using System;
using UnityEngine;

[SingletonAssetPath("Assets/[Database]/Resources/PowerVisualSettings.asset")]
public class PowerVisualSettings : SingletonAsset<PowerVisualSettings>
{
	public AnimationCurve pieceGrowCurve;

	public AnimationCurve pieceMoveCurve;

	public AnimationCurve flashScreenCurve;

	public AnimationCurve notesExpandCurve;

	public AnimationCurve bluebubbleExpandCurve;

	public AnimationCurve frogHitEffectScaleCurve;

	public Material rainbowHitEffectMaterial;
}
