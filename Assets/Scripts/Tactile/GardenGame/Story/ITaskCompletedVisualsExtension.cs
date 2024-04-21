using System;
using System.Collections;
using UnityEngine;

namespace Tactile.GardenGame.Story
{
	public interface ITaskCompletedVisualsExtension
	{
		IEnumerator AnimateCompleted(Vector3 endPosition);
	}
}
