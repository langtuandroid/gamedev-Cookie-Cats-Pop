using System;
using System.Diagnostics;
using TactileModules.SideMapButtons;
using UnityEngine;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	[RequireComponent(typeof(UIButton))]
	public class StoryMapSideButton : SideMapButton, ISideMapButton, IStoryMapSideButton
	{
		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public new event Action Clicked;



		private new void Start()
		{
			this.GetButton().Clicked += delegate(UIButton button)
			{
				this.Clicked();
			};
		}

		public void SetBadgeText(string text)
		{
			this.badgeLabel.text = text;
			this.badgePivot.gameObject.SetActive(!string.IsNullOrEmpty(text));
		}

		public override bool VisibilityChecker(object data)
		{
			return base.VisibilityChecker(data);
		}

		public void SetTimeLeft(int secondsLeft)
		{
			string text = L.FormatSecondsAsColumnSeparated(secondsLeft, L.Get("Ended"), TimeFormatOptions.None);
			this.timeLeftLabel.text = text;
		}

		[SerializeField]
		private UILabel badgeLabel;

		[SerializeField]
		private Transform badgePivot;

		[SerializeField]
		private UILabel timeLeftLabel;
	}
}
