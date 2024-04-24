using System.Collections.Generic;
using Fibers;
using TactileModules.Foundation;
using UnityEngine;

public class InboxView : UIView
{
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
    

    private void EvaluateAndHandleRequests()
    {
        if (false)
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
    
   
    private void UpdateUI()
    {
       
    }


    public UIListPanel itemList;

    public GameObject elementPrefab;

    public GameObject notConnectedPivot;

    public GameObject noMessagesPivot;

    private Fiber handleRequestFiber = new Fiber();

    private bool keyAddedSuccessfully;
}
