using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Fibers;
using JetBrains.Annotations;
using TactileModules.FacebookExtras;
using TactileModules.Foundation;
using UnityEngine;

public class FacebookSelectFriendsAndRequestView : UIView
{
    //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event FacebookSelectFriendsAndRequestView.AskFriendsSelectedEvent SelectionChanged;

    //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event FacebookSelectFriendsAndRequestView.AskFriendsSelectedEvent AskFriendsClicked;

    public FacebookSelectFriendsAndRequestView.RequestType Type { get; private set; }

    private DialogFrame title
    {
        get
        {
            return this.dialogWithRedBar.GetInstance<DialogFrame>();
        }
    }

    private FacebookClient FacebookClient
    {
        get
        {
            return ManagerRepository.Get<FacebookClient>();
        }
    }

    private FacebookLoginManager FacebookLoginManager
    {
        get
        {
            return ManagerRepository.Get<FacebookLoginManager>();
        }
    }

    protected override void ViewLoad(object[] parameters)
    {
        this.Type = (FacebookSelectFriendsAndRequestView.RequestType)parameters[0];
        this.elementPrefab.gameObject.SetActive(false);
        this.elementAskOtherPrefab.gameObject.SetActive(false);
        this.loadingSpinner.SetActive(false);
        this.buttonAskNow.gameObject.SetActive(false);
        this.buttonAskNowDisabled.gameObject.SetActive(true);
        this.showMoreFriends = (parameters.Length < 2 || (bool)parameters[1]);
        this.normalPivot.SetActive(this.FacebookClient.HasFriendListPermissions);
        this.needPermissionPivot.SetActive(!this.FacebookClient.HasFriendListPermissions);
        if (this.FacebookClient.HasFriendListPermissions)
        {
            this.Initialize();
        }
    }

    private void Initialize()
    {
        ButtonWithTitle instance = this.buttonAskNow.GetInstance<ButtonWithTitle>();
        ButtonWithTitle instance2 = this.buttonAskNowDisabled.GetInstance<ButtonWithTitle>();
        this.UpdateTable();
        this.updateFiber.Start(this.UpdateFriendsList());
        switch (this.Type)
        {
            case FacebookSelectFriendsAndRequestView.RequestType.Life:
                this.title.Title = L.Get("Get Free Lives");
                instance.Title = L.Get("Ask");
                instance2.Title = L.Get("Ask");
                break;
            case FacebookSelectFriendsAndRequestView.RequestType.GiftLives:
                this.title.Title = L.Get("Help your Friends!\nSend Lives\t");
                instance.Title = L.Get("Send");
                instance2.Title = L.Get("Send");
                break;
            case FacebookSelectFriendsAndRequestView.RequestType.TournamentLife:
                this.title.Title = string.Empty;
                instance.Title = L.Get("Ask");
                instance2.Title = L.Get("Ask");
                break;
            case FacebookSelectFriendsAndRequestView.RequestType.Key:
                this.title.Title = L.Get("Ask For Keys");
                instance.Title = L.Get("Ask");
                instance2.Title = L.Get("Ask");
                break;
        }
    }

    protected override void ViewWillDisappear()
    {
        this.updateFiber.Terminate();
        this.getFriendListFiber.Terminate();
    }

    protected override void ViewDidAppear()
    {
        this.UpdateTable();
    }

    [UsedImplicitly]
    private void ButtonSelectAll(UIEvent e)
    {
        this.selectAll(true);
    }

    [UsedImplicitly]
    private void ButtonDeSelectAll(UIEvent e)
    {
        this.selectAll(false);
    }

    [UsedImplicitly]
    private void ButtonClosePermission(UIEvent e)
    {
        this.needPermissionPivot.SetActive(false);
        if (!this.FacebookClient.HasFriendListPermissions)
        {
            base.Close(0);
        }
    }

    private void selectAll(bool v)
    {
        this.allSelected = v;
        this.buttonSelectAll.SetActive(!this.allSelected);
        this.buttonDeSelectAll.SetActive(this.allSelected);
        List<FacebookFriendSelectItem> list = new List<FacebookFriendSelectItem>(this.itemList.GetComponentsInChildren<FacebookFriendSelectItem>(true));
        foreach (FacebookFriendSelectItem facebookFriendSelectItem in list)
        {
            facebookFriendSelectItem.SetSelected(this.allSelected);
        }
        if (this.SelectionChanged != null)
        {
            this.SelectionChanged(this, this.selected);
        }
    }

    [UsedImplicitly]
    private void ButtonAsk(UIEvent e)
    {
        if (this.AskFriendsClicked != null && this.selected.Count > 0)
        {
            this.AskFriendsClicked(this, this.selected);
        }
    }

    [UsedImplicitly]
    private void ButtonGetPermission(UIEvent e)
    {
        Fiber fiber = new Fiber();
        fiber.Start(this.GetPermissionCr());
    }

    private IEnumerator GetPermissionCr()
    {
        if (this.FacebookClient.HasFriendListPermissions)
        {
            yield break;
        }
        yield return this.FacebookLoginManager.EnsureFriendListPermissions();
        this.normalPivot.SetActive(this.FacebookClient.HasFriendListPermissions);
        this.needPermissionPivot.SetActive(!this.FacebookClient.HasFriendListPermissions);
        if (this.FacebookClient.HasFriendListPermissions)
        {
            this.Initialize();
        }
        yield break;
    }

    [UsedImplicitly]
    private void ButtonAskOther(UIEvent e)
    {

    }

    [UsedImplicitly]
    private void ButtonExitClicked(UIEvent e)
    {
        base.Close(0);
    }

    private void HandleFriendSelectedChanged(FacebookUser user, bool isChecked)
    {
        if (isChecked)
        {
            if (!this.selected.Contains(user))
            {
                this.selected.Add(user);
            }
        }
        else if (this.selected.Contains(user))
        {
            this.selected.Remove(user);
        }
        if (this.SelectionChanged != null)
        {
            this.SelectionChanged(this, this.selected);
        }
        this.UpdateAskButton();
    }

    private void UpdateAskButton()
    {
        bool flag = this.selected.Count > 0;
        this.buttonAskNow.gameObject.SetActive(flag);
        this.buttonAskNowDisabled.gameObject.SetActive(!flag);
    }

    private IEnumerator UpdateFriendsList()
    {
        if (this.FacebookClient.IsSessionValid)
        {
            this.users.Clear();
            this.selected.Clear();
            this.loadingSpinner.SetActive(true);
            IEnumerator e = this.FacebookClient.UpdateCachedFriends(delegate (object err)
            {
            });
            while (e.MoveNext())
            {
                object obj = e.Current;
                yield return obj;
            }
            if (this.FacebookClient.CachedFriends != null)
            {
                this.users.AddRange(this.FacebookClient.CachedFriends);
            }
            foreach (FacebookUser facebookUser in this.users)
            {
            }
            this.loadingSpinner.SetActive(false);
            this.UpdateTable();
            yield return null;
        }
        yield break;
    }

    private void UpdateTable()
    {
        this.itemList.DestroyAllContent();
        this.itemList.BeginAdding();
        for (int i = 0; i < this.users.Count; i++)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.elementPrefab);
            gameObject.SetActive(true);
            this.itemList.AddToContent(gameObject.GetComponent<UIElement>());
            FacebookFriendSelectItem component = gameObject.GetComponent<FacebookFriendSelectItem>();
            FacebookUser user = this.users[i];
            component.Initialize(user);
            component.AppearInPanel();
            component.FriendSelected += this.HandleFriendSelectedChanged;
        }
        if (this.showMoreFriends)
        {
            GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.elementAskOtherPrefab);
            gameObject2.SetActive(true);
            this.itemList.AddToContent(gameObject2.GetComponent<UIElement>());
        }
        this.itemList.EndAdding();
        this.UpdateAskButton();
        this.selectAll(true);
    }

    public GameObject normalPivot;

    public GameObject needPermissionPivot;

    public GameObject buttonSelectAll;

    public GameObject buttonDeSelectAll;

    public UIInstantiator buttonAskNow;

    public UIInstantiator buttonAskNowDisabled;

    public GameObject buttonClose;

    public GameObject loadingSpinner;

    public GameObject elementPrefab;

    public GameObject elementAskOtherPrefab;

    public UIInstantiator dialogWithRedBar;

    public UIListPanel itemList;

    private bool showMoreFriends;

    private List<FacebookUser> users = new List<FacebookUser>();

    private List<FacebookUser> selected = new List<FacebookUser>();

    protected Fiber updateFiber = new Fiber();

    protected Fiber getFriendListFiber = new Fiber();

    private bool allSelected;

    public enum RequestType
    {
        Life,
        GiftLives,
        TournamentLife,
        Key
    }

    public delegate void AskFriendsSelectedEvent(FacebookSelectFriendsAndRequestView askFriendsView, List<FacebookUser> users);
}
