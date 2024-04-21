using System;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem.Props
{
	public class ChoosePropSettings : ScriptableObject
	{
		public float BlinkStrength = 0.1f;

		public float BlinkSpeed = 1f;

		public float ZoomToPropDuration;

		public AnimationCurve MoveToPropCurve;

		public AnimationCurve ZoomToPropCurve;
	}
}
