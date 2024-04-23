using System;
using System.Collections;
using UnityEngine;

namespace Tactile.GardenGame.Story
{
	public interface ITaskCompletableButtonExtension
	{
		IEnumerator Animate(Vector3 endPosition, float delay);
	}
}
