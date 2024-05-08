using System;
using System.Collections;
using System.Collections.Generic;
using Tactile;
using TactileModules.FeatureManager;
using TactileModules.Foundation;
using TactileModules.Inventory;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGame.HotStreak;
using TactileModules.PuzzleGame.Inventory;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGame.ScheduledBooster;
using TactileModules.PuzzleGame.ScheduledBooster.Data;
using TactileModules.PuzzleGame.ScheduledBooster.Model;
using UnityEngine;

public class CCPLevelStartView : LevelStartView
{
    public override void Initialize(LevelProxy levelToPlay, bool isRetrying, IPlayFlowContext context)
    {
        this.highScorePanel.GetInstance<HighscorePanel>().Initialize(levelToPlay);
        this.unlimitedItems = ManagerRepository.Get<InventorySystem>().UnlimitedItems;
        this.levelStartDialog = this.CreateAppropriateLevelStartDialog(levelToPlay);
        this.InitializeLevelStartDialog(this.levelStartDialog, levelToPlay);
        this.ShowHotStreakIfAvailable(levelToPlay);
    }

    private LevelStartDialog CreateAppropriateLevelStartDialog(LevelProxy levelToPlay)
    {
        ScheduledBoosters scheduledBoosters = ManagerRepository.Get<ScheduledBoosterSystem>().ScheduledBoosters;
        bool flag = scheduledBoosters.HasBoosterForLocation(ScheduledBoosterLocation.PreGame, levelToPlay);
        UIElement uielement = (!flag) ? UnityEngine.Object.Instantiate<UIElement>(this.normalDialogPrefab) : UnityEngine.Object.Instantiate<UIElement>(this.labDialogPrefab);
        uielement.transform.parent = base.transform;
        uielement.SetSizeAndDoLayout(base.GetElementSize());
        uielement.transform.localPosition = Vector3.zero;
        uielement.gameObject.SetLayerRecursively(base.gameObject.layer);
        LevelStartDialog componentInChildren = uielement.GetComponentInChildren<LevelStartDialog>();
        componentInChildren.ClickedPlay += this.OnDialogOnClickedPlay;
        componentInChildren.ClickedDismiss += this.OnDialogOnClickedDismiss;
        componentInChildren.ClickedCheat += this.OnDialogOnClickedCheat;
        return componentInChildren;
    }

    private void OnDialogOnClickedCheat(int numStars)
    {
        base.Developer_InvokeCheatCompleteClicked(numStars);
    }

    private void OnDialogOnClickedDismiss()
    {
        base.InvokeDismissButtonClicked();
    }

    private void OnDialogOnClickedPlay()
    {
        List<SelectedBooster> selectedBoosters = new List<SelectedBooster>(this.CollectSelectedBoosters());
        base.InvokePlayButtonClicked(selectedBoosters);
    }

    private void InitializeLevelStartDialog(LevelStartDialog LevelStartDialog, LevelProxy levelToPlay)
    {
        foreach (GameObject gameObject in LevelStartDialog.hardObjects)
        {
            gameObject.SetActive(false);
        }
        LevelStartDialog.slideAndLadders.SetActive(false);
        bool flag = levelToPlay.LevelAsset is EndlessLevel;
        int numStars = levelToPlay.Stars;
        int hardStars = levelToPlay.GetHardStars();
        if (flag)
        {
            numStars = 3;
        }
        LevelStartDialog.SetStars(numStars, hardStars);
        if (levelToPlay.LevelCollection is TreasureHuntCollectionAsset)
        {
            LevelStartDialog.levelLabel.text = string.Format(L.Get("Level {0}"), levelToPlay.DisplayName);
            LevelStartDialog.nextGoalLabel.text = this.GetGoalText(numStars, levelToPlay);
        }
        else if (levelToPlay.LevelCollection is GateAsset)
        {
            LevelStartDialog.levelLabel.text = L.Get("Gate level");
            LevelStartDialog.nextGoalLabel.text = string.Empty;
        }
        else if (levelToPlay.LevelCollection is SlidesAndLaddersLevelDatabase)
        {
            this.SlidesAndLaddersIsActive(levelToPlay.LevelCollection as SlidesAndLaddersLevelDatabase, levelToPlay.Index, LevelStartDialog);
            LevelStartDialog.levelLabel.text = L.Get("Slides N' Ladders");
            LevelStartDialog.nextGoalLabel.text = string.Empty;
        }
        else if (flag)
        {
            LevelStartDialog.levelLabel.text = L.Get("Endless Challenge");
            LevelStartDialog.nextGoalLabel.text = string.Empty;
        }
        else
        {
            LevelStartDialog.levelLabel.text = string.Format(L.Get("Level {0}"), levelToPlay.DisplayName);
            LevelStartDialog.nextGoalLabel.text = this.GetGoalText(numStars, levelToPlay);
            foreach (GameObject gameObject2 in LevelStartDialog.hardObjects)
            {
                gameObject2.SetActive(levelToPlay.LevelDifficulty == LevelDifficulty.Hard);
            }
        }
        this.InitBoosterButton(LevelStartDialog.boosterButtons[0].GetInstance<BoosterButton>(), "BoosterSuperAim", levelToPlay, this.unlimitedItems.HasUnlimitedTime("BoosterSuperAim"));
        this.InitBoosterButton(LevelStartDialog.boosterButtons[1].GetInstance<BoosterButton>(), "BoosterSuperQueue", levelToPlay, this.unlimitedItems.HasUnlimitedTime("BoosterSuperQueue"));
        this.InitBoosterButton(LevelStartDialog.boosterButtons[2].GetInstance<BoosterButton>(), "BoosterShield", levelToPlay, this.unlimitedItems.HasUnlimitedTime("BoosterShield"));
        ManagerRepository.Get<BoosterManager>().AutoSelectedBoosters = null;
        bool flag2 = false;
        foreach (UIInstantiator uiinstantiator in LevelStartDialog.boosterButtons)
        {
            if (!uiinstantiator.GetInstance<BoosterButton>().Disabled)
            {
                flag2 = true;
                break;
            }
        }
        if (!flag2)
        {
            BoosterMetaData boosterUnlockingEarliest = this.GetBoosterUnlockingEarliest(LevelStartDialog);
            if (boosterUnlockingEarliest != null)
            {
                MainLevelDatabase levelDatabase = this.LevelDatabaseCollection.GetLevelDatabase<MainLevelDatabase>("Main");
                int humanNumber = levelDatabase.GetHumanNumber(levelDatabase.GetLevel(boosterUnlockingEarliest.UnlockAtLevelIndex));
                LevelStartDialog.chooseBoostersLabel.text = string.Format(L.Get("Reach level {0} for boosters"), humanNumber);
            }
            else
            {
                LevelStartDialog.chooseBoostersLabel.text = L.Get("No boosters available");
            }
        }
        this.highScorePanel.GetInstance<HighscorePanel>().Initialize(levelToPlay);
    }

    private string GetGoalText(int numStars, LevelProxy levelToPlay)
    {
        if (numStars >= 3)
        {
            return string.Format(L.Get("Your Best: {0}"), levelToPlay.Points);
        }
        if (numStars > 0)
        {
            return string.Format(L.Get("Next Star: {0}"), levelToPlay.StarThresholds[numStars]);
        }
        return L.Get("Rescue the Kittens");
    }

    private void SlidesAndLaddersIsActive(SlidesAndLaddersLevelDatabase levelDatabase, int levelIndex, LevelStartDialog LevelStartDialog)
    {
        LevelStartDialog.slideAndLadders.SetActive(true);
        bool flag = levelDatabase.IsTreasureLevel(levelIndex) && !levelDatabase.IsChestClaimed(levelIndex);
        bool flag2 = levelDatabase.IsSlideLevel(levelIndex);
        bool flag3 = levelDatabase.IsLadderLevel(levelIndex);
        LevelStartDialog.chest.SetActive(flag);
        LevelStartDialog.slide.SetActive(flag2);
        LevelStartDialog.ladder.SetActive(flag3);
        if (flag2)
        {
            UIElement component = LevelStartDialog.slide.GetComponent<UIElement>();
            component.Size *= ((!flag) ? ((!flag3) ? 1f : 0.85f) : 0.5f);
            if (flag || flag3)
            {
                LevelStartDialog.slide.transform.localPosition += new Vector3(component.Size.x / 2f, (!flag) ? 0f : (-component.Size.y / 2f), 0f);
            }
        }
        if (flag3)
        {
            UIElement component2 = LevelStartDialog.ladder.GetComponent<UIElement>();
            component2.Size *= ((!flag) ? ((!flag2) ? 1f : 0.85f) : 0.5f);
            if (flag)
            {
                LevelStartDialog.ladder.transform.localPosition -= (Vector3)component2.Size / 2f;
            }
        }
    }

    private void InitBoosterButton(BoosterButton btn, InventoryItem boosterId, LevelProxy levelToPlay, bool isUnlimited)
    {
        btn.BoosterId = boosterId;
        btn.Disabled = !BoosterManagerBase<BoosterManager>.Instance.IsBoosterUnlocked(boosterId);
        if (levelToPlay.LevelCollection is MainLevelDatabase && BoosterManagerBase<BoosterManager>.Instance.IsBoosterUnlockedAtLevelIndex(boosterId, levelToPlay.Index))
        {
            btn.Free = true;
            btn.Selected = true;
            btn.gameObject.GetComponent<Collider>().enabled = false;
        }
        if (isUnlimited && BoosterManagerBase<BoosterManager>.Instance.IsBoosterUnlocked(boosterId))
        {
            btn.Selected = true;
            btn.Unlimited = true;
        }
        if (boosterId == "BoosterShield")
        {
            bool flag = LevelUtils.PieceWithTypeExist<MinusPiece>(levelToPlay.LevelAsset as LevelAsset, null);
            bool flag2 = LevelUtils.PieceWithTypeExist<DeathPiece>(levelToPlay.LevelAsset as LevelAsset, null);
            btn.Locked = (!flag && !flag2);
            btn.ConstantWobble = !btn.Locked;
            if (btn.Locked)
            {
                btn.Selected = false;
            }
            if (levelToPlay.LevelCollection is EndlessChallengeLevelDatabase)
            {
                btn.Disabled = false;
            }
        }
        btn.GetButton().Clicked += this.BoosterButtonClicked;
        btn.GetButton().payload = boosterId;
        if (ManagerRepository.Get<BoosterManager>().AutoSelectedBoosters != null && ManagerRepository.Get<BoosterManager>().AutoSelectedBoosters.Contains(boosterId))
        {
            btn.Selected = true;
        }
    }

    private BoosterMetaData GetBoosterUnlockingEarliest(LevelStartDialog LevelStartDialog)
    {
        BoosterMetaData boosterMetaData = null;
        foreach (UIInstantiator uiinstantiator in LevelStartDialog.boosterButtons)
        {
            BoosterButton instance = uiinstantiator.GetInstance<BoosterButton>();
            BoosterMetaData metaData = InventoryManager.Instance.GetMetaData<BoosterMetaData>(instance.BoosterId);
            if (boosterMetaData == null || metaData.UnlockAtLevelIndex < boosterMetaData.UnlockAtLevelIndex)
            {
                boosterMetaData = metaData;
            }
        }
        return boosterMetaData;
    }

    private LevelDatabaseCollection LevelDatabaseCollection
    {
        get
        {
            return ManagerRepository.Get<LevelDatabaseCollection>();
        }
    }

    private IEnumerable<SelectedBooster> CollectSelectedBoosters()
    {
        foreach (UIInstantiator inst in this.levelStartDialog.boosterButtons)
        {
            BoosterButton btn = inst.GetInstance<BoosterButton>();
            if (btn.Selected)
            {
                yield return new SelectedBooster
                {
                    id = btn.BoosterId,
                    isFree = (btn.Free || btn.Unlimited)
                };
            }
        }
        yield break;
    }

    private void ShowHotStreakIfAvailable(LevelProxy levelToPlay)
    {
        HotStreakManager featureHandler = FeatureManager.GetFeatureHandler<HotStreakManager>();
        if (levelToPlay.LevelCollection is MainLevelDatabase && featureHandler.IsHotStreakActive && featureHandler.SecondsLeft > 0)
        {
            GameObject original = Resources.Load<GameObject>("HotStreak/HotStreakCoffeePot");
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(original, this.levelStartDialog.buttonAddonPivot);
            gameObject.layer = this.levelStartDialog.buttonAddonPivot.gameObject.layer;
            SkeletonAnimation componentInChildren = gameObject.GetComponentInChildren<SkeletonAnimation>();
            if (componentInChildren)
            {
                int num = Mathf.Min(featureHandler.Progress, featureHandler.MaxWins);
                string animationName = "Smoke";
                if (num > 0 && num <= featureHandler.MaxWins)
                {
                    animationName = "Jug_Filling_" + num;
                }
                if (componentInChildren.HasAnimation(animationName))
                {
                    componentInChildren.AnimationState.SetAnimation(0, animationName, false);
                }
            }
        }
    }

    private void BoosterButtonClicked(UIButton e)
    {
        BoosterButton component = e.GetComponent<BoosterButton>();
        if (component.Disabled)
        {
            return;
        }
        string iconSpriteName = InventoryManager.Instance.GetMetaData<CPBoosterMetaData>(component.BoosterId).IconSpriteName;
        if (component.Locked)
        {
            UIViewManager.Instance.ShowView<BoosterLockedView>(new object[]
            {
                iconSpriteName
            });
        }
        else
        {
            bool flag = this.unlimitedItems.HasUnlimitedTime(component.BoosterId);
            int numberOfBoosters = BoosterManagerBase<BoosterManager>.Instance.GetNumberOfBoosters(component.BoosterId);
            if (!flag && numberOfBoosters <= 0)
            {
                FiberCtrl.Pool.Run(this.BuyBooster(component), false);
            }
            else
            {
                component.Selected = !component.Selected;
            }
        }
    }

    private IEnumerator BuyBooster(BoosterButton boosterButton)
    {
        BoosterMetaData boosterInfo = InventoryManager.Instance.GetMetaData<BoosterMetaData>(boosterButton.BoosterId);
        ShopItem shopItem = ShopManager.Instance.GetShopItem(boosterInfo.PreGameShopItemIdentifier);
        UIViewManager.UIViewStateGeneric<BuyShopItemView> vs = UIViewManager.Instance.ShowView<BuyShopItemView>(new object[]
        {
            shopItem,
            false
        });
        yield return vs.WaitForClose();
        if ((BuyShopItemView.Result)vs.ClosingResult == BuyShopItemView.Result.SuccessfullyBought)
        {
            boosterButton.Selected = true;
        }
        yield break;
    }

    private void Developer_CheatComplete(UIEvent e)
    {
        int numStars = (int)e.payload;
        base.Developer_InvokeCheatCompleteClicked(numStars);
    }

    [SerializeField]
    private UIInstantiator highScorePanel;

    [SerializeField]
    private UIElement normalDialogPrefab;

    [SerializeField]
    private UIElement labDialogPrefab;

    private LevelStartDialog levelStartDialog;

    private IUnlimitedItems unlimitedItems;
}
