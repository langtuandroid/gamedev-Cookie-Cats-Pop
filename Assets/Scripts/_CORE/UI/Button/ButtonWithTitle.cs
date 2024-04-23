using System;
using TactileModules.Validation;
using UnityEngine;

public class ButtonWithTitle : AnimatedButton
{
	[Instantiator.SerializeLocalizableProperty]
	public virtual string Title
	{
		get
		{
			return (!(this.title != null)) ? string.Empty : this.title.text;
		}
		set
		{
			if (this.title != null)
			{
				this.title.text = value;
			}
			this.ReflectState();
		}
	}

	[Instantiator.SerializeLocalizableProperty]
	public virtual float TitleOffsetX
	{
		get
		{
			return (!(this.title != null)) ? 0f : this.title.transform.localPosition.x;
		}
		set
		{
			if (this.title != null)
			{
				Vector3 localPosition = this.title.transform.localPosition;
				localPosition.x = value;
				this.title.transform.localPosition = localPosition;
			}
		}
	}

	private void Start()
	{
		if (Application.isPlaying)
		{
			this.Title = L.Get(this.Title);
		}
	}

	[SerializeField]
	[OptionalSerializedField]
	protected UILabel title;
}
