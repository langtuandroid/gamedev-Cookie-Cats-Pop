using System;
using System.Collections;
using Tactile.GardenGame.MapSystem;
using UnityEngine;

namespace Tactile.GardenGame.Story
{
	public class MapActionSpeak : MapAction, IMapActionLocalizable
	{
		public string SpeakerId
		{
			get
			{
				return this.speakerId;
			}
			set
			{
				this.speakerId = value;
			}
		}

		public string Text
		{
			get
			{
				return this.text;
			}
			set
			{
				this.text = value;
			}
		}

		public override IEnumerator Logic(IStoryMapController map)
		{
			MapSpeaker speaker = map.GetMapComponent<MapSpeaker>(this.speakerId);
			if (this.speakerId == null)
			{
				yield break;
			}
			yield return speaker.Speak(L.Get(this.text));
			yield break;
		}

		string IMapActionLocalizable.GetLocalizableText()
		{
			return this.Text;
		}

		string IMapActionLocalizable.GetContext()
		{
			GameObject defaultObjectsPrefab = GardenGameSetup.Get.defaultObjectsPrefab;
			foreach (MapSpeaker mapSpeaker in defaultObjectsPrefab.GetComponentsInChildren<MapSpeaker>())
			{
				if (mapSpeaker.GetComponent<MapObjectID>().Id == this.speakerId)
				{
					return mapSpeaker.gameObject.name;
				}
			}
			return "?";
		}

		public override bool IsAllowedWhenSkipping
		{
			get
			{
				return true;
			}
		}

		[SerializeField]
		private string speakerId;

		[SerializeField]
		private string text;
	}
}
