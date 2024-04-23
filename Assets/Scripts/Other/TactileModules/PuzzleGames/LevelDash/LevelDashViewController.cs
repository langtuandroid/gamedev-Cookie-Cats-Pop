using System;
using System.Collections;
using System.Collections.Generic;
using Cloud;
using TactileModules.PuzzleGames.LevelDash.Providers;
using TactileModules.PuzzleGames.LevelDash.Views;
using UnityEngine;

namespace TactileModules.PuzzleGames.LevelDash
{
    public class LevelDashViewController
    {
        public LevelDashViewController(LevelDashManager manager, ILevelDashDataProvider dataProvider, ILevelDashViewProvider viewProvider)
        {
            this.manager = manager;
            this.dataProvider = dataProvider;
            this.viewProvider = viewProvider;
            manager.OnLevelDashStarted += this.Reset;
            manager.OnLevelDashEntriesRefresh += this.Reset;
        }

        private void Reset(LevelDashManager manager)
        {
            this.myLastListIndex = -1;
        }

        public IEnumerator ShowStartPopup(bool isReminder)
        {
            UIViewManager.UIViewStateGeneric<LevelDashStartView> vs = UIViewManager.Instance.ShowView<LevelDashStartView>(new object[0]);
            LevelDashStartView startView = vs.View;
            startView.Init(isReminder, new Func<string>(this.GetFeatureTimeRemainingAsString), this.manager.CustomInstanceData.StartLevel);
            yield return vs.WaitForClose();
            if ((int)vs.ClosingResult > 0)
            {
                yield return this.ShowLeaderboardView();
            }
            yield break;
        }

        public IEnumerator ShowLeaderboardView()
        {
            List<Entry> currentEntries = new List<Entry>(this.manager.GetAllEntries());
            List<Entry> prevEntries = currentEntries;
            int myCurrentListIndex = this.manager.GetMyIndexInList(currentEntries);
            if (this.myLastListIndex < 0 || myCurrentListIndex < 0 || currentEntries[this.myLastListIndex].MaxLevel == currentEntries[myCurrentListIndex].MaxLevel)
            {
                this.myLastListIndex = myCurrentListIndex;
            }
            else if (this.myLastListIndex != myCurrentListIndex)
            {
                prevEntries = new List<Entry>(currentEntries);
                Entry item = prevEntries[myCurrentListIndex];
                prevEntries.RemoveAt(myCurrentListIndex);
                prevEntries.Insert(this.myLastListIndex, item);
            }
            UIViewManager.UIViewStateGeneric<LevelDashLeaderboardView> vs = UIViewManager.Instance.ShowView<LevelDashLeaderboardView>(new object[0]);
            LevelDashLeaderboardView leaderboardView = vs.View;
            leaderboardView.Init(new Func<string>(this.GetFeatureTimeRemainingAsString), prevEntries, new Func<Entry, List<Entry>, GameObject, GameObject>(this.InstantiateLeaderboardCellItem), new Action(this.OnLeaderboardViewInfoButtonClicked), this.myLastListIndex, myCurrentListIndex);
            yield return vs.WaitForClose();
            this.myLastListIndex = myCurrentListIndex;
            yield break;
        }

        public IEnumerator ShowResults()
        {
            RewardStatus status = this.manager.CustomInstanceData.ReceivedRewardStatus;
            object view = null;
            switch (status + 1)
            {
                case RewardStatus.TournamentActive:
                    view = PuzzleGameData.DialogViews.ShowMessageBox(L.Get("Sorry"), L.Get("Level Dash which you were participating is outdated."), L.Get("OK"), null);
                    this.manager.EndFeature();
                    break;
                case RewardStatus.Presented:
                    view = PuzzleGameData.DialogViews.ShowMessageBox(L.Get("Tournament is active"), L.Get("Level Dash is still active, please try again later."), L.Get("OK"), null);
                    break;
                case RewardStatus.PreviouslyPresented:
                    {
                        LevelDashConfig.Reward reward = this.manager.GetMyReward();
                        yield return this.HandleFeatureEnd(reward);
                        break;
                    }
                case (RewardStatus)3:
                    view = PuzzleGameData.DialogViews.ShowMessageBox(L.Get("Sorry"), L.Get("You have already claimed the reward on another device."), L.Get("OK"), null);
                    this.manager.EndFeature();
                    break;
                default:
                    view = PuzzleGameData.DialogViews.ShowMessageBox(L.Get("Sorry"), L.Get("Something went wrong, please try again later."), L.Get("OK"), null);
                    break;
            }
            if (view != null)
            {
                yield return PuzzleGameData.DialogViews.WaitForClosingView(view);
            }
            yield break;
        }

        public IEnumerator HandleFeatureEnd(LevelDashConfig.Reward reward)
        {
            yield return this.ShowLeaderboardView();
            this.dataProvider.ClaimReward(reward);
            this.manager.EndFeature();
            if (reward != null)
            {
                yield return this.viewProvider.ShowRewardView(reward);
            }
            yield break;
        }

        public IEnumerator ShowResultsForPreviousLevelDash()
        {
            int rank = this.manager.TypeCustomData.MyRankForPreviousLevelDash;
            this.manager.TypeCustomData.MyRankForPreviousLevelDash = -1;
            LevelDashConfig.Reward reward = this.manager.GetReward(rank);
            if (reward != null)
            {
                this.dataProvider.ClaimReward(reward);
                yield return this.viewProvider.ShowRewardViewForPreviousLevelDash(rank, reward);
            }
            yield break;
        }

        public IEnumerator PerformEndFlow()
        {
            if (this.manager.GetMyReward() == null)
            {
                yield return this.HandleFeatureEnd(null);
                yield break;
            }
            while (this.manager.IsActiveOnClient)
            {
                UIViewManager.UIViewStateGeneric<LevelDashEndedView> viewState = UIViewManager.Instance.ShowView<LevelDashEndedView>(new object[0]);
                yield return viewState.WaitForClose();
                if ((int)viewState.ClosingResult <= 0)
                {
                    yield break;
                }
                this.DoSyncStatus();
                yield return this.manager.WaitForSyncingEnd();
            }
            yield break;
        }

        private void DoSyncStatus()
        {
            this.loadingView = PuzzleGameData.DialogViews.ShowProgressView(L.Get("Loading..."));
            this.manager.SyncStatus(new Func<LevelDashViewController.SyncResultView, ReturnCode, IEnumerator>(this.OnSyncFinished));
        }

        public void OnSideButtonClicked(UIButton uiButton)
        {
            this.DoSyncStatus();
        }

        private IEnumerator ShowErrorMessage(ReturnCode error)
        {
            if (error == ReturnCode.ClientConnectionError)
            {
                object v = PuzzleGameData.DialogViews.ShowMessageBox(L.Get("Connection Failed"), L.Get("Please check your internet connection and try again."), L.Get("OK"), null);
                yield return PuzzleGameData.DialogViews.WaitForClosingView(v);
            }
            else
            {
                object v2 = PuzzleGameData.DialogViews.ShowMessageBox(L.Get("Sorry"), L.Get("Something went wrong, please try again later."), L.Get("OK"), null);
                yield return PuzzleGameData.DialogViews.WaitForClosingView(v2);
            }
            yield break;
        }

        private string GetFeatureTimeRemainingAsString()
        {
            return this.manager.GetTimeRemainingAsString();
        }

        private GameObject InstantiateLeaderboardCellItem(Entry entry, List<Entry> entries, GameObject cellItemPrefab)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(cellItemPrefab);
            gameObject.SetActive(true);
            this.viewProvider.InitializeLeaderboardItem(entry, gameObject);
            return gameObject;
        }

        private void OnLeaderboardViewInfoButtonClicked()
        {
            UIViewManager.UIViewStateGeneric<LevelDashStartView> uiviewStateGeneric = UIViewManager.Instance.ShowView<LevelDashStartView>(new object[0]);
            LevelDashStartView view = uiviewStateGeneric.View;
            view.Init(true, new Func<string>(this.GetFeatureTimeRemainingAsString), this.manager.CustomInstanceData.StartLevel);
        }

        private IEnumerator OnSyncFinished(LevelDashViewController.SyncResultView syncResultView, ReturnCode error)
        {
            if (this.loadingView != null)
            {
                PuzzleGameData.DialogViews.CloseView(this.loadingView);
                yield return PuzzleGameData.DialogViews.WaitForClosingView(this.loadingView);
            }
            switch (syncResultView)
            {
                case LevelDashViewController.SyncResultView.Leaderboard:
                    yield return this.ShowLeaderboardView();
                    break;
                case LevelDashViewController.SyncResultView.Results:
                    yield return this.ShowResults();
                    break;
                case LevelDashViewController.SyncResultView.Error:
                    yield return this.ShowErrorMessage(error);
                    break;
            }
            yield break;
        }

        private LevelDashManager manager;

        private ILevelDashDataProvider dataProvider;

        private ILevelDashViewProvider viewProvider;

        private int myLastListIndex = -1;

        private object loadingView;

        public enum SyncResultView
        {
            Leaderboard,
            Results,
            Error
        }
    }
}
