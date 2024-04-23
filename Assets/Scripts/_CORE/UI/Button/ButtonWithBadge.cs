using System;
using UnityEngine;

public class ButtonWithBadge : AnimatedButton
{
	[Instantiator.SerializeProperty]
	public string BadgeText
	{
		get
		{
			return this.badgeLabel.text;
		}
		set
		{
			this.badgeLabel.text = value;
			this.badge.gameObject.SetActive(!string.IsNullOrEmpty(value));
		}
	}

	[Instantiator.SerializeProperty]
	public string IconSpriteName
	{
		get
		{
			return this.icon.SpriteName;
		}
		set
		{
			this.icon.SpriteName = value;
		}
	}

	[SerializeField]
	private GameObject badge;

	[SerializeField]
	private UILabel badgeLabel;

	[SerializeField]
	private UISprite icon;
}
