using System;
using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class UIButton : MonoBehaviour
{
	public static UIButton LastActivatedButton { get; private set; }

	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<UIButton> PressedDown = delegate (UIButton A_0)
    {
    };



    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action<UIButton> Clicked = delegate (UIButton A_0)
    {
    };



    [Instantiator.SerializeProperty]
	public Instantiator.MethodReference Receiver
	{
		get
		{
			return new Instantiator.MethodReference
			{
				message = this.methodName,
				receiver = this.receiver
			};
		}
		set
		{
			this.receiver = value.receiver;
			this.methodName = value.message;
		}
	}

	[Instantiator.SerializeProperty]
	public int PayloadAsInt
	{
		get
		{
			return this.ID;
		}
		set
		{
			this.ID = value;
			this.payload = value;
		}
	}

	[Instantiator.SerializeProperty]
	public bool ClickOnEscapeKey
	{
		get
		{
			return this.clickOnEscapeKey;
		}
		set
		{
			this.clickOnEscapeKey = value;
		}
	}

	public object Payload
	{
		get
		{
			if (this.ID >= 0)
			{
				return this.ID;
			}
			return this.payload;
		}
		set
		{
			this.payload = value;
			if (value is int)
			{
				this.ID = (int)value;
			}
			else
			{
				this.ID = -1;
			}
		}
	}

	private void OnClick()
	{
		if (!base.enabled)
		{
			return;
		}
		if (this.clickType == UIButton.ClickType.Click)
		{
			this.FireEvent();
		}
	}

	private void OnPress(object value)
	{
		if (!base.enabled)
		{
			return;
		}
		bool flag = (bool)value;
		if (flag)
		{
			this.PressedDown(this);
			if (this.clickType == UIButton.ClickType.Press)
			{
				this.FireEvent();
			}
		}
	}

	private void Update()
	{
		if (this.clickOnEscapeKey && UIViewManager.Instance.IsEscapeKeyDownAndAvailable(base.gameObject.layer))
		{
			this.FireEvent();
		}
	}

	private void FireEvent()
	{
		this.Clicked(this);
		if (this.receiver != null && !string.IsNullOrEmpty(this.methodName))
		{
			UIButton.LastActivatedButton = this;
			UIEvent uievent = default(UIEvent);
			uievent.sender = base.gameObject;
			uievent.payload = this.Payload;
			string name = base.gameObject.name;
			if (name.Contains("_temp_") && base.gameObject.transform.parent != null)
			{
				name = base.gameObject.transform.parent.name;
			}
			NinjaUIDebugData.LogAction(string.Format("Button:{0},RecievingMethod:{1},Payload:{2}", name, this.methodName, (uievent.payload == null) ? "null" : uievent.payload.ToString()));
			this.receiver.SendMessage(this.methodName, uievent, SendMessageOptions.RequireReceiver);
		}
	}

	[SerializeField]
	[HideInInspector]
	private int ID = -1;

	[HideInInspector]
	public GameObject receiver;

	[HideInInspector]
	public string methodName;

	public object payload;

	public bool clickOnEscapeKey;

	public UIButton.ClickType clickType;

	public UIButton.ButtonMood mood;

	public enum ClickType
	{
		Click,
		Press
	}

	public enum ButtonMood
	{
		Neutral,
		Confirm,
		Dismiss
	}
}
