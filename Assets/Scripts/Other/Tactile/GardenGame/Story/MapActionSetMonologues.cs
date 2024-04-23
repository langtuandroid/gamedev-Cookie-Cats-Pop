using System;
using System.Collections;
using Tactile.GardenGame.Story.Monologue;
using UnityEngine;

namespace Tactile.GardenGame.Story
{
	public class MapActionSetMonologues : MapAction
	{
		public MonologueDatabase Database
		{
			get
			{
				return this.database;
			}
			set
			{
				this.database = value;
			}
		}

		public override IEnumerator Logic(IStoryMapController map)
		{
			map.Monologues.SetDatabase(this.database);
			yield break;
		}

		public override bool IsAllowedWhenSkipping
		{
			get
			{
				return true;
			}
		}

		[SerializeField]
		private MonologueDatabase database;
	}
}
