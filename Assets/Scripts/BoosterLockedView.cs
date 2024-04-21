using System;
using UnityEngine;

public class BoosterLockedView : UIView
{
	protected override void ViewLoad(object[] parameters)
	{
		if (parameters.Length > 0)
		{
			this.boosterIcon.SpriteName = (string)parameters[0];
		}
	}

	private void DismissClicked(UIEvent e)
	{
		base.Close(0);
	}

	[SerializeField]
	private UISprite boosterIcon;
}
