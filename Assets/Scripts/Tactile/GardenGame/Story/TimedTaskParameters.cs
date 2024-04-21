using System;
using UnityEngine;

namespace Tactile.GardenGame.Story
{
	public class TimedTaskParameters : MonoBehaviour
	{
		[LocalizedStringField]
		public string title;

		[LocalizedStringField]
		public string description;

		public bool timerEnabled = true;

		public int coinSkipCost;

		public int waitTimeSeconds;
	}
}
