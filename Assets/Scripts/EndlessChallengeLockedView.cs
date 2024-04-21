using System;
using JetBrains.Annotations;
using UnityEngine;

public class EndlessChallengeLockedView : UIView
{
	protected override void ViewLoad(object[] parameters)
	{
		if (parameters != null && parameters.Length > 0)
		{
			this.lvlRequired = (int)parameters[0];
		}
		this.description.text = string.Format(L.Get("Endless Challenge is locked until level {0}."), this.lvlRequired);
	}

	[UsedImplicitly]
	private void OnCloseClicked(UIEvent e)
	{
		base.Close(0);
	}

	[SerializeField]
	private UILabel description;

	private int lvlRequired;
}
