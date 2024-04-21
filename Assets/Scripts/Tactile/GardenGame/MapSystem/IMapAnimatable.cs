using System;
using System.Collections;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	public abstract class IMapAnimatable : MapComponent
	{
		public abstract IEnumerator PlayAnimation(string animationID, Vector2 direction, string transitionAnimation, int loops);

		public abstract string[] GetAvailableAnimations();

		public abstract bool SupportsTransitionAnimations(string animName);
	}
}
