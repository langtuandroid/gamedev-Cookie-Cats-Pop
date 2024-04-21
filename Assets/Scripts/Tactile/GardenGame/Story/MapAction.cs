using System;
using System.Collections;
using UnityEngine;

namespace Tactile.GardenGame.Story
{
	public abstract class MapAction : MonoBehaviour
	{
		public Vector2 Position
		{
			get
			{
				return this.position;
			}
			set
			{
				this.position = value;
			}
		}

		public MapAction.ActionType Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		public bool DontWait
		{
			get
			{
				return this.dontWait;
			}
			set
			{
				this.dontWait = value;
			}
		}

		public abstract IEnumerator Logic(IStoryMapController map);

		public virtual bool IsAllowedWhenSkipping
		{
			get
			{
				return false;
			}
		}

		[SerializeField]
		private Vector2 position;

		[SerializeField]
		private MapAction.ActionType type;

		[SerializeField]
		private bool dontWait;

		public enum ActionType
		{
			Default,
			Intro,
			Idle,
			EnterMap
		}
	}
}
