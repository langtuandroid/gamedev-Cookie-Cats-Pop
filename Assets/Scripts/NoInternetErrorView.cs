using System;
using UnityEngine;

public class NoInternetErrorView : UIView
{
	protected override void ViewLoad(object[] parameters)
	{
		if (parameters.Length > 0)
		{
			this.dialog.GetInstance<DialogFrame>().TitleUiLabel.multiLine = true;
			this.dialog.GetInstance<DialogFrame>().Title = parameters[0].ToString();
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
	}

	private void Dismiss(UIEvent e)
	{
		base.Close(0);
	}

	[SerializeField]
	private UIInstantiator dialog;

	[SerializeField]
	private UILabel message;

	[SerializeField]
	private UIInstantiator button0;
}
