using System;
using TactileModules.Validation;
using UnityEngine;

public class ButtonToggle : ButtonWithTitle
{
	[Instantiator.SerializeProperty]
	public string Icon
	{
		get
		{
			return (!(this.icon != null)) ? string.Empty : this.icon.SpriteName;
		}
		set
		{
			if (this.icon != null)
			{
				this.icon.SpriteName = value;
			}
			if (this.offIcon != null)
			{
				this.offIcon.SpriteName = value;
			}
		}
	}

	[Instantiator.SerializeProperty]
	public bool IsOn
	{
		get
		{
			return this.isOn;
		}
		set
		{
			this.isOn = value;
			this.ReflectState();
		}
	}

	[Instantiator.SerializeProperty]
	public override string Title
	{
		get
		{
			return base.Title;
		}
		set
		{
			this.offLabel.text = value;
			base.Title = value;
		}
	}

	[Instantiator.SerializeProperty]
	public override float TitleOffsetX
	{
		get
		{
			return base.TitleOffsetX;
		}
		set
		{
			base.TitleOffsetX = value;
			this.offLabel.transform.position = this.title.transform.position;
		}
	}

	protected override void ReflectState()
	{
		base.ReflectState();
		this.onPivot.SetActive(!base.Disabled && this.isOn);
		this.offPivot.SetActive(base.Disabled || !this.isOn);
		this.offLabel.text = this.Title;
	}

	protected override void HandleButtonClicked(UIButton b)
	{
		this.IsOn = !this.IsOn;
		base.HandleButtonClicked(b);
	}

	private bool isOn;

	[SerializeField]
	[OptionalSerializedField]
	private UISprite icon;

	[SerializeField]
	private GameObject onPivot;

	[SerializeField]
	private GameObject offPivot;

	[SerializeField]
	private UILabel offLabel;

	[SerializeField]
	[OptionalSerializedField]
	private UISprite offIcon;
}
