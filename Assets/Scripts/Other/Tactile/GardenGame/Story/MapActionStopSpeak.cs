using System;
using System.Collections;
using Fibers;
using Tactile.GardenGame.MapSystem;
using UnityEngine;

namespace Tactile.GardenGame.Story
{
	public class MapActionStopSpeak : MapAction
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

		public override IEnumerator Logic(IStoryMapController map)
		{
			MapSpeaker speaker = map.GetMapComponent<MapSpeaker>(this.speakerId);
			if (this.speakerId == null)
			{
				yield break;
			}
			yield return new Fiber.OnExit(new Fiber.OnExitHandler(speaker.HideSpeechBubbleInstantly));
			yield return speaker.HideSpeechBubble();
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
		private string speakerId;
	}
}
