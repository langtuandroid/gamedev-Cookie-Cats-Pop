using System;
using System.Collections;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	public class MapCameraMovement : MonoBehaviour, IMapMoveableMovement
	{
		public IEnumerator Move(Vector2 start, Vector2 target, Vector2 startDirection, Vector2 targetDirection, float movementSpeed)
		{
			float duration = (movementSpeed <= 0f) ? 1f : (1f / movementSpeed);
			yield return FiberAnimation.MoveTransform(base.transform, base.transform.position, target, this.curve, duration);
			yield break;
		}

		[SerializeField]
		private AnimationCurve curve;
	}
}
