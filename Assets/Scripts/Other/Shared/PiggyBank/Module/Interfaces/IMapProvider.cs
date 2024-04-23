using System;
using UnityEngine;

namespace Shared.PiggyBank.Module.Interfaces
{
	public interface IMapProvider
	{
		AnimationCurve CameraFocusCurve();

		AnimationCurve ItemDropCurve();

		AnimationCurve ItemDropScaleCurve();

		GameObject GetLevelDotIndicatorPrefab();

		void PlayDropGameObjectSound();
	}
}
