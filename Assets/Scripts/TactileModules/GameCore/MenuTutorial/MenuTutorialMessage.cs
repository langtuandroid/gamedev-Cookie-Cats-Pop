using System;
using UnityEngine;

namespace TactileModules.GameCore.MenuTutorial
{
	public class MenuTutorialMessage : MonoBehaviour
	{
		[Instantiator.SerializeProperty]
		public string Message
		{
			get
			{
				return this.messageLabel.text;
			}
			set
			{
				this.messageLabel.text = value;
			}
		}

		[SerializeField]
		private UILabel messageLabel;
	}
}
