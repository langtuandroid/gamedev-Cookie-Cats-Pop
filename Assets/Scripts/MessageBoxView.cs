using System;
using UnityEngine;

public class MessageBoxView : UIView
{
	protected override void ViewLoad(object[] parameters)
	{
		if (parameters.Length > 0)
		{
			this.title.text = parameters[0].ToString();
		}
		if (parameters.Length > 1)
		{
			this.message.text = parameters[1].ToString();
		}
		if (parameters.Length > 2 && parameters[2] != null)
		{
			this.button0.GetInstance<ButtonWithTitle>().Title = parameters[2].ToString();
			this.button0.gameObject.SetActive(true);
		}
		else
		{
			this.button0.gameObject.SetActive(true);
		}
		if (parameters.Length > 3 && parameters[3] != null)
		{
			this.button1.GetInstance<ButtonWithTitle>().Title = parameters[3].ToString();
			this.button1.gameObject.SetActive(true);
		}
		else
		{
			this.button1.gameObject.SetActive(false);
		}
		if (parameters.Length > 4 && parameters[4] != null)
		{
			this.icon.SpriteName = parameters[4].ToString();
			this.icon.gameObject.SetActive(true);
			this.icon.CorrectAspect(AspectCorrection.Fit);
		}
		else
		{
			this.icon.gameObject.SetActive(false);
		}
	}

	private void Button1Clicked(UIEvent e)
	{
		base.Close(1);
	}

	private void Button0Clicked(UIEvent e)
	{
		base.Close(0);
	}

	[Header("Message Box View")]
	public UIInstantiator dialogInstantiator;

	public UILabel message;

	public UISprite icon;

	public UIInstantiator button0;

	public UIInstantiator button1;

	public UILabel title;
}
