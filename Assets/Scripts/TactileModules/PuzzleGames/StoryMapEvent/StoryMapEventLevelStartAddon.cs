using System;
using System.Diagnostics;
using JetBrains.Annotations;
using TactileModules.ComponentLifecycle;
using UnityEngine;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	public class StoryMapEventLevelStartAddon : LifecycleBroadcaster
	{
		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnButtonClicked;



		public void SetVisible(bool show)
		{
			base.gameObject.SetActive(show);
		}

		public void SetPagesLeft(int currencyLeftToEarn)
		{
			this.pagesLeftLabel.text = string.Format(L.Get("{0} left"), currencyLeftToEarn);
		}

		public void SetTimeLeft(int secondsLeft)
		{
			string text = L.FormatSecondsAsColumnSeparated(secondsLeft, L.Get("Ended"), TimeFormatOptions.None);
			this.timeLeftLabel.text = text;
		}

		[UsedImplicitly]
		public void ButtonClicked(UIEvent e)
		{
			this.OnButtonClicked();
		}

		[SerializeField]
		private UILabel pagesLeftLabel;

		[SerializeField]
		private UILabel timeLeftLabel;
	}
}
