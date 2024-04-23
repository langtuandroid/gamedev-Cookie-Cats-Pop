using System;
using UnityEngine;

namespace TactileModules.UserSupport.View
{
	public class LoadingDialogueView : UIView
	{
		protected override void ViewLoad(object[] parameters)
		{
			if (parameters.Length > 0)
			{
				this.message.text = parameters[0].ToString();
			}
		}

		[SerializeField]
		protected UILabel message;
	}
}
