using System;
using UnityEngine;

[SingletonAssetPath("Assets/[Database]/Resources/MapVisualSettings.asset")]
public class MapVisualSettings : SingletonAsset<MapVisualSettings>
{
	public AnimationCurve avatarMoveCurve;
}
