using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using TactileModules.Foundation;
using UnityEngine;

public class FacebookSelectFriendsAndRequest : MonoBehaviour
{
    private FacebookRequestManager FacebookRequestManager
    {
        get
        {
            return ManagerRepository.Get<FacebookRequestManager>();
        }
    }

    private void OnEnable()
    {
        this.askFriendsView.AskFriendsClicked += this.HandleAskFriendsViewAskFriendsClicked;
    }

    private void OnDisable()
    {
        this.askFriendsView.AskFriendsClicked -= this.HandleAskFriendsViewAskFriendsClicked;
    }

    private void HandleAskFriendsViewAskFriendsClicked(FacebookSelectFriendsAndRequestView askFriendsView, List<FacebookUser> selectedUsers)
    {
        if (selectedUsers.Count == 0)
        {
            return;
        }
        new Fiber(this.HandleAskFriendsViewAskFriendsClickedCr(askFriendsView, selectedUsers));
    }

    private IEnumerator HandleAskFriendsViewAskFriendsClickedCr(FacebookSelectFriendsAndRequestView askFriendsView, List<FacebookUser> selectedUsers)
    {
        if (this.processingAskFriends)
        {
            yield break;
        }
        yield return new Fiber.OnExit(delegate ()
        {
            this.processingAskFriends = false;
        });
        this.processingAskFriends = true;
        List<string> facebookIds = new List<string>();
        foreach (FacebookUser facebookUser in selectedUsers)
        {
            facebookIds.Add(facebookUser.Id);
        }
        object error = null;
        switch (askFriendsView.Type)
        {
            case FacebookSelectFriendsAndRequestView.RequestType.Life:

                break;
            case FacebookSelectFriendsAndRequestView.RequestType.GiftLives:

                break;
            case FacebookSelectFriendsAndRequestView.RequestType.TournamentLife:

                break;
            case FacebookSelectFriendsAndRequestView.RequestType.Key:

                break;
        }
        if (error != null && error.ToString() != "cancelled")
        {
            string title = L._("Could not send at this time.");
            string description = L._("It was not possible to complete the request at this time, please try again later");
            if (error.ToString().Contains("FacebookDialogException"))
            {
                if (error.ToString().Contains("ERR_INTERNET_DISCONNECTED"))
                {
                    description = L.Get("The Internet connection appears to be offline.");
                }
            }
            else
            {
                description = error.ToString();
            }
            UIViewManager.UIViewStateGeneric<NoInternetErrorView> vs = UIViewManager.Instance.ShowView<NoInternetErrorView>(new object[]
            {
                title,
                description,
                L._("Ok")
            });
            yield return vs.WaitForClose();
            if ((int)vs.ClosingResult != 0)
            {
                yield break;
            }
        }
        yield break;
    }

    public FacebookSelectFriendsAndRequestView askFriendsView;

    private bool processingAskFriends;
}
