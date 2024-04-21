using System;
using UnityEngine;

namespace TactileModules.PuzzleGames.LevelDash.Views
{
	public class LevelDashMapButton : SideMapButton
	{
		public void Init(Func<string> getTimeRemainingFunc, Func<bool> shouldShowButtonFunc)
		{
			this.getTimeRemainingFunc = getTimeRemainingFunc;
			this.shouldShowButtonFunc = shouldShowButtonFunc;
		}

		protected override void UpdateOncePerSecond()
		{
			if (this.VisibilityChecker(null))
			{
				this.timerLabel.text = this.getTimeRemainingFunc();
			}
		}

		public override SideMapButton.AreaSide Side
		{
			get
			{
				return SideMapButton.AreaSide.Left;
			}
		}

		public override bool VisibilityChecker(object data)
		{
			return this.shouldShowButtonFunc();
		}

		public override Vector2 Size
		{
			get
			{
				return base.GetComponent<UIElement>().Size;
			}
		}

		public override object Data
		{
			get
			{
				return null;
			}
		}

		[SerializeField]
		private UILabel timerLabel;

		private Func<string> getTimeRemainingFunc;

		private Func<bool> shouldShowButtonFunc;
	}
}
