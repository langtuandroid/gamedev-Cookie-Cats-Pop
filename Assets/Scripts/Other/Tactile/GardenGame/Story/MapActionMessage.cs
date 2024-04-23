using System;
using System.Collections;
using UnityEngine;

namespace Tactile.GardenGame.Story
{
	public class MapActionMessage : MapAction, IMapActionLocalizable
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

		public float Duration
		{
			get
			{
				return this.duration;
			}
			set
			{
				this.duration = value;
			}
		}

		public override IEnumerator Logic(IStoryMapController map)
		{
			yield return map.ShowFullScreenMessage(L.Get(this.Message), this.duration);
			yield break;
		}

		string IMapActionLocalizable.GetLocalizableText()
		{
			return this.Message;
		}

		string IMapActionLocalizable.GetContext()
		{
			return "FullscreenMessage";
		}

		[SerializeField]
		private string message;

		[SerializeField]
		private float duration = 2f;
	}
}
