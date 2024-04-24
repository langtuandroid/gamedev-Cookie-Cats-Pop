using System;
using System.Collections;
using System.Diagnostics;
using Tactile;
using TactileModules.Foundation;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGames.GameCore;

public class PuzzleGameImplementation : PuzzleGame.IPuzzleGame
{
    private PuzzleGameImplementation()
    {
        this.PlayerState = new PuzzleGameImplementation.PlayerStateProvider();
        this.DialogViews = new DialogViewProvider();
        this.LevelFlow = new PuzzleGameImplementation.LevelFlowProvider();
        this.UserSettings = new PuzzleGameImplementation.UserSettingsProvider();
        this.SyncPoints = new PuzzleGameImplementation.SyncPointsProvider();
    }

    public IPlayerState PlayerState { get; private set; }

    public IDialogViewProvider DialogViews { get; private set; }

    public PuzzleGame.ILevelFlow LevelFlow { get; private set; }

    public PuzzleGame.IUserSettings UserSettings { get; private set; }

    public PuzzleGame.ISyncPoints SyncPoints { get; private set; }

  

    private class UserSettingsProvider : PuzzleGame.IUserSettings
    {
        public void SaveLocalAndSyncCloud()
        {
            this.SaveLocal();
            ManagerRepository.Get<CloudSynchronizer>().SyncCloud();
        }

        public void SaveLocal()
        {
            UserSettingsManager.Instance.SaveLocalSettings();
        }
    }

    public class PlayerStateProvider : IPlayerState
    {
        public bool IsPayingUser
        {
            get
            {
                return true;
            }
        }

        int IPlayerState.FarthestUnlockedLevelIndex
        {
            get
            {
                return MainProgressionManager.Instance.GetFarthestUnlockedLevelIndex();
            }
        }

        int IPlayerState.FarthestUnlockedLevelHumanNumber
        {
            get
            {
                return MainProgressionManager.Instance.GetFarthestUnlockedLevelHumanNumber();
            }
        }

        bool IPlayerState.IsVIP
        {
            get
            {
                return VipManager.Instance.UserIsVip();
            }
        }

        int IPlayerState.Lives
        {
            get
            {
                return InventoryManager.Instance.Lives;
            }
        }
    }

    public class LevelFlowProvider : PuzzleGame.ILevelFlow
    {
        public LevelFlowProvider()
        {
            GameEventManager.Instance.OnGameEvent += delegate (GameEvent e)
            {
                if (e.type == 5)
                {
                    this.LevelUnlocked((LevelProxy)e.context);
                }
            };
        }

        ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public event Action<LevelProxy> LevelUnlocked = delegate (LevelProxy A_0)
        {
        };


    }

    public class SyncPointsProvider : PuzzleGame.ISyncPoints
    {
        public SyncPointsProvider()
        {
            ActivityManager.onResumeEvent += this.InvokeForegroundEvent;
            UIViewManager.Instance.OnFadedToBlack += this.InvokeFadeEvent;
        }

        //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public event Action OnApplicationWillEnterForeground;

        //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public event Action OnFadedToBlack;

        public void InjectDependencies(IFullScreenManager fullScreenManager)
        {
            fullScreenManager.MidChange.Register(new Func<ChangeInfo, IEnumerator>(this.HandleFullscreenMidChange));
        }

        private IEnumerator HandleFullscreenMidChange(ChangeInfo info)
        {
            if (this.OnFadedToBlack != null)
            {
                this.OnFadedToBlack();
            }
            yield break;
        }

        private void InvokeForegroundEvent()
        {
            if (this.OnApplicationWillEnterForeground != null)
            {
                this.OnApplicationWillEnterForeground();
            }
        }

        private void InvokeFadeEvent()
        {
            if (this.OnFadedToBlack != null)
            {
                this.OnFadedToBlack();
            }
        }
    }
}
