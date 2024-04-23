using System;
using UnityEngine;

namespace Tactile.GardenGame
{
	[Serializable]
	public class PathFindingSetUp
	{
		public float GetAICardinalDirectionThreshold(float distance)
		{
			if (distance < this.CardinalDirectionThresholdAIBlendMinDistance)
			{
				return this.CardinalDirectionThresholdAIMax;
			}
			if (distance > this.CardinalDirectionThresholdAIBlendMaxDistance)
			{
				return this.CardinalDirectionThresholdAIMin;
			}
			float num = this.CardinalDirectionThresholdAIBlendMaxDistance - this.CardinalDirectionThresholdAIBlendMinDistance;
			float t = (distance - this.CardinalDirectionThresholdAIBlendMinDistance) / num;
			return Mathf.Lerp(this.CardinalDirectionThresholdAIMax, this.CardinalDirectionThresholdAIMin, t);
		}

		[SerializeField]
		public bool onlyFollowIsometricDirections;

		[Tooltip("Threshold in degrees that edited paths can deviate from isometric direction before a waypoint is added")]
		[Range(0.1f, 30f)]
		[SerializeField]
		public float CardinalDirectionThresholdDirectPaths = 2f;

		[Tooltip("Min threshold in degrees that AI paths can deviate from isometric directions")]
		[Range(0.1f, 30f)]
		[SerializeField]
		public float CardinalDirectionThresholdAIMin = 1f;

		[Tooltip("Max threshold in degrees that AI paths can deviate from isometric directions")]
		[Range(0.1f, 30f)]
		[SerializeField]
		public float CardinalDirectionThresholdAIMax = 13f;

		[Tooltip("Blend distance start for AI Cadinal Threshold")]
		[Range(20f, 1000f)]
		[SerializeField]
		public float CardinalDirectionThresholdAIBlendMinDistance = 50f;

		[Tooltip("Blend distance end for AI Cadinal Threshold")]
		[Range(100f, 2000f)]
		[SerializeField]
		public float CardinalDirectionThresholdAIBlendMaxDistance = 400f;
	}
}
