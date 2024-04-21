using System;
using System.Diagnostics;
using TactileModules.Foundation;
using UnityEngine;

public class InboxItem : MonoBehaviour
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event InboxItem.OnSelected Selected;

	private FacebookClient FacebookClient
	{
		get
		{
			return ManagerRepository.Get<FacebookClient>();
		}
	}

	private void Awake()
	{
	}

	private void OnDestroy()
	{
	}

	public void Initialize(FacebookRequestData data)
	{
		if (data == null)
		{
			return;
		}
		this.heartIcon.SetActive(false);
		this.keyIcon.SetActive(false);
		this.tournamentHeartIcon.SetActive(false);
		this.data = data;
		if (data.RequestType == "askforlives")
		{
			this.label1.text = string.Format(L.Get("{0} asks for a life"), data.FromName);
			this.heartIcon.SetActive(true);
		}
		else if (data.RequestType == "life")
		{
			this.label1.text = string.Format(L.Get("{0} sends you a life"), data.FromName);
			this.heartIcon.SetActive(true);
		}
		else if (data.RequestType == "askforTournamentlives")
		{
			this.label1.text = string.Format(L.Get("{0} asks for a tournament life"), data.FromName);
			this.tournamentHeartIcon.SetActive(true);
		}
		else if (data.RequestType == "tournamentLife")
		{
			this.label1.text = string.Format(L.Get("{0} sends you a tournament life"), data.FromName);
			this.tournamentHeartIcon.SetActive(true);
		}
		else if (data.RequestType == "askforkey")
		{
			this.label1.text = string.Format(L.Get("{0} asks for a key"), data.FromName);
			this.keyIcon.SetActive(true);
		}
		else if (data.RequestType == "key")
		{
			this.label1.text = string.Format(L.Get("{0} sends you a key"), data.FromName);
			this.keyIcon.SetActive(true);
		}
		else
		{
			this.label1.text = string.Format(L.Get("{0} requests a {1}"), data.FromName, data.RequestType);
		}
	}

	private void Clicked(UIEvent e)
	{
		if (this.Selected != null)
		{
			this.Selected(this.data);
		}
	}

	public void AppearInPanel()
	{
		if (this.data == null)
		{
			return;
		}
		this.portrait.Load(this.FacebookClient, this.data.RequestSenderFacebookId, null);
	}

	public FacebookPortraitWithProgress portrait;

	public UILabel label1;

	private FacebookRequestData data;

	public GameObject heartIcon;

	public GameObject keyIcon;

	public GameObject tournamentHeartIcon;

	public delegate void OnSelected(FacebookRequestData data);
}
