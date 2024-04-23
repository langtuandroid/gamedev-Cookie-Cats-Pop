using System;
using System.Collections;
using TactileModules.FacebookExtras;
using TactileModules.Foundation;
using UnityEngine;

public class FacebookLoginInfoView : UIView
{
	private FacebookLoginManager FacebookLoginManager
	{
		get
		{
			return ManagerRepository.Get<FacebookLoginManager>();
		}
	}

	protected override void ViewLoad(object[] parameters)
	{
		LoginContext context = LoginContext.Cloud;
		if (parameters.Length > 0)
		{
			context = (LoginContext)parameters[0];
		}
		this.Setup(context);
	}

	private void Setup(LoginContext context)
	{
		ScalePulsator scalePulsator = null;
		switch (context)
		{
		case LoginContext.Friends:
			this.dialog.GetInstance<DialogFrame>().Title = L.Get("Play with friends");
			this.inboxObject.name = "0" + this.inboxObject.name;
			this.flowLayout.gameObject.SetActive(false);
			this.flowLayout.gameObject.SetActive(true);
			scalePulsator = this.inboxObjectIcon.AddComponent<ScalePulsator>();
			break;
		case LoginContext.Lives:
			this.dialog.GetInstance<DialogFrame>().Title = L.Get("Ask for Lives");
			this.livesObject.name = "0" + this.livesObject.name;
			this.flowLayout.gameObject.SetActive(false);
			this.flowLayout.gameObject.SetActive(true);
			scalePulsator = this.livesObjectIcon.AddComponent<ScalePulsator>();
			break;
		case LoginContext.Cloud:
			this.dialog.GetInstance<DialogFrame>().Title = L.Get("Connect to Facebook");
			this.cloudObject.name = "0" + this.cloudObject.name;
			this.flowLayout.gameObject.SetActive(false);
			this.flowLayout.gameObject.SetActive(true);
			scalePulsator = this.cloudObjectIcon.AddComponent<ScalePulsator>();
			break;
		case LoginContext.Compete:
			this.dialog.GetInstance<DialogFrame>().Title = L.Get("Compete with your friends!");
			this.competeObject.name = "0" + this.competeObject.name;
			this.flowLayout.gameObject.SetActive(false);
			this.flowLayout.gameObject.SetActive(true);
			scalePulsator = this.competeObjectIcon.AddComponent<ScalePulsator>();
			break;
		case LoginContext.Inbox:
			this.dialog.GetInstance<DialogFrame>().Title = L.Get("Inbox");
			this.inboxObject.name = "0" + this.inboxObject.name;
			this.flowLayout.gameObject.SetActive(false);
			this.flowLayout.gameObject.SetActive(true);
			scalePulsator = this.inboxObjectIcon.AddComponent<ScalePulsator>();
			break;
		}
		scalePulsator.speed = 0.5f;
	}

	private void Dismiss(UIEvent e)
	{
		base.Close(0);
	}

	private void Connect(UIEvent e)
	{
		FiberCtrl.Pool.Run(this.DoFacebookLogin(), false);
	}

	private IEnumerator DoFacebookLogin()
	{
		yield return this.FacebookLoginManager.EnsureLoggedInAndUserRegistered();
		if (this.FacebookLoginManager.IsLoggedInAndUserRegistered)
		{
			base.Close(1);
		}
		else
		{
			string text = L._("It was not possible to complete the request at this time, please try again later");
			UIViewManager instance = UIViewManager.Instance;
			object[] array = new object[4];
			array[0] = L.Get("Error");
			array[1] = text;
			instance.ShowView<MessageBoxView>(array);
		}
		yield break;
	}

	public UIInstantiator dialog;

	public UIFlowLayout flowLayout;

	[Header("Pitch Objects")]
	public GameObject cloudObject;

	public GameObject competeObject;

	public GameObject inboxObject;

	public GameObject livesObject;

	[Header("Pitch Object Icons")]
	public GameObject cloudObjectIcon;

	public GameObject competeObjectIcon;

	public GameObject inboxObjectIcon;

	public GameObject livesObjectIcon;
}
