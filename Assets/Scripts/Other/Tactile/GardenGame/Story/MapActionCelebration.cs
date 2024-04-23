using System;
using System.Collections;
using UnityEngine;

namespace Tactile.GardenGame.Story
{
	public class MapActionCelebration : MapAction, IMapActionLocalizable
	{
		public string Message
		{
			get
			{
				return this.message;
			}
			set
			{
				this.message = value;
			}
		}

		public override IEnumerator Logic(IStoryMapController map)
		{
			yield return map.ShowCelebration(L.Get(this.Message));
			yield break;
		}

		string IMapActionLocalizable.GetLocalizableText()
		{
			return this.Message;
		}

		string IMapActionLocalizable.GetContext()
		{
			return "Celebration";
		}

		[SerializeField]
		private string message;
	}
}
