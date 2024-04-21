using System;
using System.Collections;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	public class MapAcceleratingMovement : MonoBehaviour, IMapMoveableMovement
	{
		public IEnumerator Move(Vector2 start, Vector2 target, Vector2 startDirection, Vector2 targetDirection, float movementSpeed)
		{
			float duration = Vector2.Distance(start, target) / movementSpeed;
			yield return FiberAnimation.MoveTransform(base.transform, start, target, this.movementCurve, duration);
			yield break;
		}

		[SerializeField]
		private AnimationCurve movementCurve;
	}
}
