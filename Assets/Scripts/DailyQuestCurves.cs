using System;
using UnityEngine;

[SingletonAssetPath("Assets/[Database]/Resources/DailyQuestCurves.asset")]
public class DailyQuestCurves : SingletonAsset<DailyQuestCurves>
{
	public AnimationCurve rewardPopInCurve;

	public AnimationCurve rewardHideCurve;
}
