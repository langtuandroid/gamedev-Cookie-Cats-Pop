using System;
using TactileModules.ComponentLifecycle;
using UnityEngine;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	public class StoryMapEventCountdown : LifecycleBroadcaster
	{
		public void SetCountdown(int secondsLeft)
		{
			string text = string.Empty;
			if (secondsLeft > 0)
			{
				string str = L.FormatSecondsAsColumnSeparated(secondsLeft, L.Get("Ended"), TimeFormatOptions.None);
				text = L.Get("Event Ends in ") + str;
			}
			else
			{
				text = L.Get("Event is Ended");
			}
			this.countdownLabel.text = text;
		}

		private const string STORY_EVENT_LABEL = "Event Ends in ";

		private const string STORY_EVENT_ENDED_LABEL = "Event is Ended";

		[SerializeField]
		private UILabel countdownLabel;
	}
}
