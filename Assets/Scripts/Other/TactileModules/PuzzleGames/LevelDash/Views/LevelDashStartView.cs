using System;
using UnityEngine;

namespace TactileModules.PuzzleGames.LevelDash.Views
{
	public class LevelDashStartView : UIView
	{
		public void Init(bool isReminder, Func<string> getTimeRemainingFunc, int levelDashStartLevel)
		{
			this.isReminder = isReminder;
			this.getTimeRemainingFunc = getTimeRemainingFunc;
			this.ConfigureTitleText(isReminder);
			this.descriptionLabel.text = string.Format(L.Get("Complete levels and compete with other players who also started at level {0}!"), levelDashStartLevel);
		}

		private void ConfigureTitleText(bool isReminder)
		{
			if (this.buttonInstantiator == null)
			{
				return;
			}
			if (isReminder)
			{
				UILabel componentInChildren = this.buttonInstantiator.GetInstance<UIElement>().GetComponentInChildren<UILabel>();
				componentInChildren.text = L.Get("Continue");
			}
		}

		protected override void ViewDidAppear()
		{
		}

		private void Update()
		{
			this.timerLabel.text = this.getTimeRemainingFunc();
		}

		private void BeginClicked(UIEvent e)
		{
			if (this.isReminder)
			{
				base.Close(0);
			}
			else
			{
				base.Close(1);
			}
		}

		private void Dismiss(UIEvent e)
		{
			base.Close(0);
		}

		[SerializeField]
		private UILabel timerLabel;

		[SerializeField]
		private UILabel descriptionLabel;

		[SerializeField]
		private UIInstantiator buttonInstantiator;

		private bool isReminder;

		private Func<string> getTimeRemainingFunc;
	}
}
