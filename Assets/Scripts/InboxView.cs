using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using Tactile;
using TactileModules.FacebookExtras;
using TactileModules.Foundation;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.SagaCore;
using UnityEngine;

public class InboxView : UIView
{
    private FacebookClient FacebookClient
    {
        get
        {
            return ManagerRepository.Get<FacebookClient>();
        }
    }

    private FacebookRequestManager FacebookRequestManager
    {
        get
        {
            return ManagerRepository.Get<FacebookRequestManager>();
        }
    }

    private FacebookLoginManager FacebookLoginManager
    {
        get
        {
            return ManagerRepository.Get<FacebookLoginManager>();
        }
    }

    private LevelDatabaseCollection LevelDatabaseCollection
    {
        get
        {
            return ManagerRepository.Get<LevelDatabaseCollection>();
        }
    }

    protected override void ViewWillAppear()
    {
        this.elementPrefab.gameObject.SetActive(false);
        this.EvaluateAndHandleRequests();
    }

    protected override void ViewWillDisappear()
    {
        this.handleRequestFiber.Terminate();
    }

    private void ButtonFacebookLoginClicked(UIEvent e)
    {
        FiberCtrl.Pool.Run(this.DoFacebookLogin(), false);
    }

    private IEnumerator DoFacebookLogin()
    {
        yield return this.FacebookLoginManager.EnsureLoggedInAndUserRegistered();
        this.EvaluateAndHandleRequests();
        yield break;
    }

    private void EvaluateAndHandleRequests()
    {
        if (this.FacebookClient.IsSessionValid)
        {
            this.notConnectedPivot.SetActive(false);
            this.UpdateUI();
        }
        else
        {
            this.itemList.gameObject.SetActive(false);
            this.notConnectedPivot.SetActive(true);
            this.noMessagesPivot.SetActive(false);
        }
    }

    private void DeleteRequest(FacebookRequestData request)
    {
        FiberCtrl.Pool.Run(this.FacebookRequestManager.DeleteRequest(request, delegate (object err)
        {
            if (err != null)
            {
            }
        }), true);
    }

    private void UpdateTable()
    {
        List<FacebookRequestData> pendingRequestData = this.FacebookRequestManager.PendingRequestData;
        if (pendingRequestData.Count == 0)
        {
            this.noMessagesPivot.SetActive(true);
            this.itemList.gameObject.SetActive(false);
        }
        else
        {
            this.noMessagesPivot.SetActive(false);
            this.itemList.gameObject.SetActive(true);
            this.itemList.DestroyAllContent();
            this.itemList.BeginAdding();
            foreach (FacebookRequestData facebookRequestData in pendingRequestData)
            {
                if (!facebookRequestData.IsRequestConsumed)
                {
                    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.elementPrefab);
                    gameObject.SetActive(true);
                    this.itemList.AddToContent(gameObject.GetComponent<UIElement>());
                    InboxItem component = gameObject.GetComponent<InboxItem>();
                    component.Initialize(facebookRequestData);
                    component.AppearInPanel();
                    component.Selected += this.HandleSelectedChanged;
                }
            }
            this.itemList.EndAdding();
        }
    }

    private void UpdateUI()
    {
        this.UpdateTable();
    }

    private void HandleSelectedChanged(FacebookRequestData request)
    {
        this.handleRequestFiber.Start(this.HandleRequestCr(request));
        Fiber fiber = new Fiber();
        fiber.Start(this.ShowConfirmMessage(request));
    }

    private IEnumerator ShowConfirmMessage(FacebookRequestData request)
    {
        if (request.RequestType == "askforlives")
        {
            UIViewManager.Instance.ShowView<MessageBoxView>(new object[]
            {
                L.Get("Life sent"),
                L.Get("You sent a life!"),
                L.Get("Ok"),
                null,
                "heart"
            });
        }
        else if (request.RequestType == "life")
        {
            UICamera.DisableInput();
            UIViewManager.UIViewStateGeneric<MessageBoxView> vs = UIViewManager.Instance.ShowView<MessageBoxView>(new object[]
            {
                L.Get("Life received!"),
                L.Get("You received a life!"),
                L.Get("Ok"),
                null,
                "heart"
            });
            yield return FiberHelper.Wait(0.5f, (FiberHelper.WaitFlag)0);
            LivesOverlay life = UIViewManager.Instance.ObtainOverlay<LivesOverlay>(vs.View);
            Vector3 pos = new Vector3(0f, 0f, 0f);
            Vector3 iconPos = vs.View.icon.transform.position;
            pos.x = iconPos.x;
            pos.y = iconPos.y;
            pos.z = life.transform.position.z - 5f;
            life.GiveLife(pos, 1, false, 1f, null);
            yield return FiberHelper.Wait(1.5f, (FiberHelper.WaitFlag)0);
            UIViewManager.Instance.ReleaseOverlay<LivesOverlay>();
            UICamera.EnableInput();
            yield return vs.WaitForClose();
        }
        else if (request.RequestType == "askforTournamentlives")
        {
            UIViewManager.Instance.ShowView<MessageBoxView>(new object[]
            {
                L.Get("Tournament life sent"),
                L.Get("You sent a tournament life!"),
                L.Get("Ok"),
                null,
                "heartTournament"
            });
        }
        else if (request.RequestType == "tournamentLife")
        {
            UICamera.DisableInput();
            UIViewManager.UIViewStateGeneric<MessageBoxView> vs2 = UIViewManager.Instance.ShowView<MessageBoxView>(new object[]
            {
                L.Get("Tournament life received!"),
                L.Get("You received a tournament life!"),
                L.Get("Ok"),
                null,
                "heartTournament"
            });
            yield return FiberHelper.Wait(0.5f, (FiberHelper.WaitFlag)0);
            LivesOverlay life2 = UIViewManager.Instance.ObtainOverlay<LivesOverlay>(vs2.View);
            life2.ChangeLivesType("TournamentLife");
            Vector3 pos2 = new Vector3(0f, 0f, 0f);
            Vector3 iconPos2 = vs2.View.icon.transform.position;
            pos2.x = iconPos2.x;
            pos2.y = iconPos2.y;
            pos2.z = life2.transform.position.z - 5f;
            life2.GiveLife(pos2, 1, false, 1f, null);
            yield return FiberHelper.Wait(1.5f, (FiberHelper.WaitFlag)0);
            UIViewManager.Instance.ReleaseOverlay<LivesOverlay>();
            life2.ChangeLivesType("Life");
            UICamera.EnableInput();
        }
        else if (request.RequestType == "askforkey")
        {
            UIViewManager.Instance.ShowView<MessageBoxView>(new object[]
            {
                L.Get("Key sent!"),
                L.Get("You sent a key!"),
                L.Get("Ok"),
                null,
                "key"
            });
        }
        else if (request.RequestType == "key")
        {
            if (this.keyAddedSuccessfully)
            {
                UIViewManager.UIViewStateGeneric<MessageBoxView> vs3 = UIViewManager.Instance.ShowView<MessageBoxView>(new object[]
                {
                    L.Get("Key received!"),
                    L.Get("You received a key!"),
                    L.Get("Ok"),
                    null,
                    "key"
                });
                yield return vs3.WaitForClose();
                base.Close(0);
                FlowStack flowStack = ManagerRepository.Get<FlowStack>();
                flowStack.Find<MainMapFlow>().MapContentController.Refresh();
                flowStack.Push(new InboxView.MapFlowInvokerDummyFlow());
                UIViewManager.UIViewState gateview = UIViewManager.Instance.ShowView<SagaGateView>(new object[0]);
                yield return gateview.WaitForClose();
                if (gateview.ClosingResult != null && (int)gateview.ClosingResult == 2)
                {
                    yield return this.ShowUnlockViewAndMoveAvatar();
                }
            }
            else
            {
                UIViewManager.Instance.ShowView<MessageBoxView>(new object[]
                {
                    L.Get("Key already received!"),
                    L.Get("You already received a key from this person!"),
                    L.Get("Ok"),
                    null,
                    "key"
                });
            }
        }
        yield break;
    }

    private IEnumerator ShowUnlockViewAndMoveAvatar()
    {
        this.FacebookRequestManager.RefreshRequestFilter();
        this.LevelDatabaseCollection.GetLevelDatabase<MainLevelDatabase>("Main").Save();
        UIViewManager.UIViewStateGeneric<GateUnlockedView> vs = UIViewManager.Instance.ShowView<GateUnlockedView>(new object[0]);
        yield return vs.WaitForClose();
        yield break;
    }

    private IEnumerator HandleRequestCr(FacebookRequestData request)
    {
        this.FacebookRequestManager.MakeSureRequestIsMarkedConsumed(request);
        if (request.RequestType == "askforlives")
        {
            FiberCtrl.FiberPool pool = FiberCtrl.Pool;
            IEnumerator[] array = new IEnumerator[2];

            array[1] = this.FacebookRequestManager.DeleteRequest(request, delegate (object err)
            {
                if (err != null)
                {
                }
            });
            pool.Run(FiberHelper.RunSerial(array), true);
        }
        else if (request.RequestType == "life")
        {
            InventoryManager.Instance.Add("Life", 1, "receiveLife");
            FiberCtrl.Pool.Run(this.FacebookRequestManager.DeleteRequest(request, delegate (object err)
            {
                if (err != null)
                {
                }
            }), true);
        }
        else if (request.RequestType == "askforTournamentlives")
        {
            FiberCtrl.FiberPool pool2 = FiberCtrl.Pool;
            IEnumerator[] array2 = new IEnumerator[2];

            array2[1] = this.FacebookRequestManager.DeleteRequest(request, delegate (object err)
            {
                if (err != null)
                {
                }
            });
            pool2.Run(FiberHelper.RunSerial(array2), true);
        }
        else if (request.RequestType == "tournamentLife")
        {
            InventoryManager.Instance.Add("TournamentLife", 1, "receiveTournamentLife");
            FiberCtrl.Pool.Run(this.FacebookRequestManager.DeleteRequest(request, delegate (object err)
            {
                if (err != null)
                {
                }
            }), true);
        }
        else if (request.RequestType == "askforkey")
        {
            FiberCtrl.FiberPool pool3 = FiberCtrl.Pool;
            IEnumerator[] array3 = new IEnumerator[2];

            array3[1] = this.FacebookRequestManager.DeleteRequest(request, delegate (object err)
            {
                if (err != null)
                {
                }
            });
            pool3.Run(FiberHelper.RunSerial(array3), true);
        }
        else if (request.RequestType == "key")
        {
            this.keyAddedSuccessfully = GateManager.Instance.AddKey(request.RequestSenderFacebookId);
            FiberCtrl.Pool.Run(this.FacebookRequestManager.DeleteRequest(request, delegate (object err)
            {
                if (err != null)
                {
                }
            }), true);
        }
        this.EvaluateAndHandleRequests();
        yield return null;
        yield break;
    }

    private void ButtonCloseClicked(UIEvent e)
    {
        base.Close(0);
    }

    public UIListPanel itemList;

    public GameObject elementPrefab;

    public GameObject notConnectedPivot;

    public GameObject noMessagesPivot;

    private Fiber handleRequestFiber = new Fiber();

    private bool keyAddedSuccessfully;

    private class MapFlowInvokerDummyFlow : IFlow, IFiberRunnable
    {
        public IEnumerator Run()
        {
            yield break;
        }

        public void OnExit()
        {
        }
    }
}
