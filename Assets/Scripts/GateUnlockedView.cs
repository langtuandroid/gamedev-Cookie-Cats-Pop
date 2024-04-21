using System;
using TactileModules.PuzzleGame.MainLevels;
using UnityEngine;

public class GateUnlockedView : UIView
{
    protected override void ViewLoad(object[] parameters)
    {
        this.Title.text = L.Get("Gate Unlocked");
        this.Description.text = L.Get("Congratulations! You have unlocked the gate!");
        this.OKButton.gameObject.SetActive(true);
        this.OKButton.GetInstance<ButtonWithTitle>().Title = L.Get("Ok");
        LevelProxy farthestCompletedLevelProxy = MainProgressionManager.Instance.GetFarthestCompletedLevelProxy();
        if (farthestCompletedLevelProxy.LevelMetaData is GateMetaData)
        {
            GateMetaData gateMetaData = farthestCompletedLevelProxy.LevelMetaData as GateMetaData;
            if (gateMetaData != null && gateMetaData.gateIndex == 0)
            {

            }
        }
    }

    protected override void ViewDidAppear()
    {
    }

    protected override void ViewDidDisappear()
    {
    }

    private void OkClicked(UIEvent e)
    {
        base.Close(0);
    }

    private void CloseClicked(UIEvent e)
    {
        base.Close(1);
    }

    [Header("Gate Unlocked View")]
    [SerializeField]
    private UIInstantiator OKButton;

    [SerializeField]
    private UILabel Title;

    [SerializeField]
    private UILabel Description;
}
