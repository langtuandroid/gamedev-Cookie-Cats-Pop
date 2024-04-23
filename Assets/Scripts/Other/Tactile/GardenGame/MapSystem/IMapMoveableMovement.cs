using System;
using System.Collections;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	public interface IMapMoveableMovement
	{
		IEnumerator Move(Vector2 start, Vector2 target, Vector2 startDirection, Vector2 targetDirection, float speed);
	}
}
