using System;
using TactileModules.Validation;
using UnityEngine;

public class ButtonWithIcon : AnimatedButton
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
		}
	}

	[Instantiator.SerializeProperty]
	public float IconOffsetX
	{
		get
		{
			return (!(this.icon != null)) ? 0f : this.icon.transform.localPosition.x;
		}
		set
		{
			if (this.icon != null)
			{
				Vector3 localPosition = this.icon.transform.localPosition;
				localPosition.x = value;
				this.icon.transform.localPosition = localPosition;
			}
		}
	}

	[Instantiator.SerializeProperty]
	public float IconOffsetY
	{
		get
		{
			return (!(this.icon != null)) ? 0f : this.icon.transform.localPosition.y;
		}
		set
		{
			if (this.icon != null)
			{
				Vector3 localPosition = this.icon.transform.localPosition;
				localPosition.y = value;
				this.icon.transform.localPosition = localPosition;
			}
		}
	}

	[SerializeField]
	[OptionalSerializedField]
	private UISprite icon;
}
