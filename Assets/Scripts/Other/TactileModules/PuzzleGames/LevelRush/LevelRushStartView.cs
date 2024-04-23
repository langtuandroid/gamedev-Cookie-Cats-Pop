using System;
using UnityEngine;

namespace TactileModules.PuzzleGames.LevelRush
{
	public class LevelRushStartView : UIView
	{
		protected override void ViewLoad(object[] parameters)
		{
			bool flag = false;
			if (parameters != null && parameters.Length > 0)
			{
				flag = (bool)parameters[0];
			}
			if (flag)
			{
				this.descriptionLabel.text = L.Get(this.reminderDescriptionLocalized);
				UILabel componentInChildren = this.buttonInstantiator.GetInstance<UIElement>().GetComponentInChildren<UILabel>();
				componentInChildren.text = L.Get(this.reminderButtonText);
			}
		}

		private void Dismiss(UIEvent e)
		{
			base.Close(0);
		}

		[SerializeField]
		private UILabel descriptionLabel;

		[SerializeField]
		private UIInstantiator buttonInstantiator;

		[SerializeField]
		[LocalizedStringField]
		private string reminderDescriptionLocalized = "The LEVEL RUSH is still active! Complete the levels to earn buzztastic rewards!";

		[SerializeField]
		[LocalizedStringField]
		private string reminderButtonText = "Continue";
	}
}
