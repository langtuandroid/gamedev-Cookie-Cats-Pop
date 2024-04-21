using System;
using UnityEngine;

[SingletonAssetPath("Assets/[Database]/Resources/CommonCurves.asset")]
public class CommonCurves : SingletonAsset<CommonCurves>
{
	public AnimationCurve easeInOut;

	public AnimationCurve quickPopSlowOut;

	public AnimationCurve itemDropCurve;

	public AnimationCurve itemDropScaleCurve;
}
