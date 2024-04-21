using System;
using TactileModules.ComponentLifecycle;
using UnityEngine;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	public class StoryMapEventCurrencyOverlay : LifecycleBroadcaster
	{
		public void AmountChanged(int value)
		{
			this.amountLabel.text = value.ToString();
		}

		[SerializeField]
		private UILabel amountLabel;
	}
}
