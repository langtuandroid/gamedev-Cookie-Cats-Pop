using System;
using System.Diagnostics;
using JetBrains.Annotations;
using TactileModules.Validation;
using UnityEngine;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	public class StoryMapEventAnnounceView : UIView, IStoryMapEventAnnounceView, IUIView
	{
		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action CallToActionClicked;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action DismissClicked;



		[UsedImplicitly]
		private void CallToAction(UIEvent e)
		{
			this.CallToActionClicked();
		}

		[UsedImplicitly]
		private void Dismiss(UIEvent e)
		{
			this.DismissClicked();
		}

		public void SetTimeLeft(int seconds)
		{
			if (this.timeLeftLabel == null)
			{
				return;
			}
			string text = L.FormatSecondsAsColumnSeparated(seconds, L.Get("Ended"), TimeFormatOptions.None);
			this.timeLeftLabel.text = text;
		}

		[SerializeField]
		[OptionalSerializedField]
		private UILabel timeLeftLabel;
	}
}
