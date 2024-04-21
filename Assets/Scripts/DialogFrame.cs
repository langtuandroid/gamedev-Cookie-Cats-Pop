using System;
using TactileModules.Validation;
using UnityEngine;

public class DialogFrame : MonoBehaviour
{
	public UILabel TitleUiLabel
	{
		get
		{
			return this.title;
		}
	}

	[Instantiator.SerializeLocalizableProperty]
	public string Title
	{
		get
		{
			return (!(this.title != null)) ? string.Empty : this.title.text;
		}
		set
		{
			if (this.title == null)
			{
				return;
			}
			this.title.text = value;
		}
	}

	[Instantiator.SerializeProperty(true)]
	public bool HasCloseButton
	{
		get
		{
			return this.closeButton.gameObject.activeSelf;
		}
		set
		{
			this.closeButton.gameObject.SetActive(value);
		}
	}

	[Instantiator.SerializeProperty]
	public Instantiator.MethodReference CloseReceiver
	{
		get
		{
			return this.closeButton.Receiver;
		}
		set
		{
			this.closeButton.Receiver = value;
		}
	}

	[Instantiator.SerializeProperty]
	public float TopZOffset
	{
		get
		{
			if (this.topPivot == null)
			{
				return 0f;
			}
			return this.topPivot.localPosition.z;
		}
		set
		{
			if (this.topPivot == null)
			{
				return;
			}
			Vector3 localPosition = this.topPivot.localPosition;
			localPosition.z = value;
			this.topPivot.localPosition = localPosition;
		}
	}

	[SerializeField]
	private UIButton closeButton;

	[SerializeField]
	[OptionalSerializedField]
	private UILabel title;

	[SerializeField]
	[OptionalSerializedField]
	private Transform topPivot;
}
