using System;
using JetBrains.Annotations;
using UnityEngine;

namespace TactileModules.UserSupport.View
{
	public class AlertView : UIView, IAlertView
	{
		public void Show(string message)
		{
			this.label.text = message;
		}

		[UsedImplicitly]
		public void Dismiss(UIEvent e)
		{
			base.Close(0);
		}

		[SerializeField]
		private UILabel label;
	}
}
