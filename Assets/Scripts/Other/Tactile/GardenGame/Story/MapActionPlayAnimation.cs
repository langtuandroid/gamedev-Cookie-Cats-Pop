using System;
using System.Collections;
using Tactile.GardenGame.MapSystem;
using UnityEngine;

namespace Tactile.GardenGame.Story
{
	public class MapActionPlayAnimation : MapAction
	{
		public int Loops
		{
			get
			{
				return this.loops;
			}
			set
			{
				this.loops = value;
			}
		}

		public string MapAnimatableID
		{
			get
			{
				return this.map3dModelId;
			}
			set
			{
				this.map3dModelId = value;
			}
		}

		public string Animation
		{
			get
			{
				return this.animation;
			}
			set
			{
				this.animation = value;
			}
		}

		public string TransitionAnimation
		{
			get
			{
				return this.transitionAnimation;
			}
			set
			{
				this.transitionAnimation = value;
			}
		}

		public Vector2 Direction
		{
			get
			{
				return this.direction;
			}
			set
			{
				this.direction = value;
			}
		}

		public override IEnumerator Logic(IStoryMapController map)
		{
			IMapAnimatable mapAnimatable = map.GetMapComponent<IMapAnimatable>(this.MapAnimatableID);
			if (mapAnimatable == null)
			{
				yield break;
			}
			yield return mapAnimatable.PlayAnimation(this.Animation, this.direction, this.transitionAnimation, this.loops);
			yield break;
		}

		[SerializeField]
		private string map3dModelId;

		[SerializeField]
		private string animation;

		[SerializeField]
		private string transitionAnimation;

		[SerializeField]
		private Vector2 direction;

		[SerializeField]
		private int loops = 1;
	}
}
