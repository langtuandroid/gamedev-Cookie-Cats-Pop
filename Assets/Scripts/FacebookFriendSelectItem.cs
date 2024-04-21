using System;
using System.Diagnostics;
using JetBrains.Annotations;
using TactileModules.Foundation;
using UnityEngine;

public class FacebookFriendSelectItem : MonoBehaviour
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event FacebookFriendSelectItem.OnFriendSelected FriendSelected;

	private FacebookClient FacebookClient
	{
		get
		{
			return ManagerRepository.Get<FacebookClient>();
		}
	}

	public void Initialize(FacebookUser user)
	{
		this.user = user;
		this.UpdateUI();
	}

	public void SetSelected(bool v)
	{
		this.Select(v);
	}

	private void Select(bool v)
	{
		this.userSelected = v;
		this.UpdateUI();
		if (this.FriendSelected != null && this.user != null)
		{
			this.FriendSelected(this.user, this.userSelected);
		}
	}

	[UsedImplicitly]
	private void Clicked(UIEvent e)
	{
		this.Select(!this.userSelected);
	}

	private void UpdateUI()
	{
		this.selected.SetActive(this.userSelected);
		this.unselected.SetActive(!this.userSelected);
	}

	public void AppearInPanel()
	{
		if (this.user != null)
		{
			this.firstName.text = this.user.FirstName;
			this.lastName.text = this.user.LastName;
			this.portrait.Load(this.FacebookClient, this.user.Id, null);
		}
	}

	public FacebookPortraitWithProgress portrait;

	public UILabel firstName;

	public UILabel lastName;

	public GameObject selected;

	public GameObject unselected;

	private FacebookUser user;

	private bool userSelected;

	public delegate void OnFriendSelected(FacebookUser user, bool selected);
}
