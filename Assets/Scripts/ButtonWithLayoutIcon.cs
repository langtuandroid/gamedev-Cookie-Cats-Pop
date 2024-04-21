using System;

public class ButtonWithLayoutIcon : AnimatedButton
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

	public UISprite icon;
}
