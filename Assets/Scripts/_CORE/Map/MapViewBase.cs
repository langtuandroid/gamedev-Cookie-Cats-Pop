using System;
using System.Collections;
using Fibers;
using UnityEngine;

public abstract class MapViewBase : UIView
{
    protected bool didProgress { get; set; }

    protected string lastLevelPlayedType { get; private set; }

    protected bool startupSequenceWasShown { get; private set; }

    public IUIView ButtonView
    {
        get
        {
            if (this.buttonLayerViewState == null)
            {
                return null;
            }
            return this.buttonLayerViewState.View;
        }
    }

    public bool allLevelsCompleted { get; private set; }

    protected virtual bool ClampScrollTop
    {
        get
        {
            return false;
        }
    }

    protected virtual bool ClampScrollBottom
    {
        get
        {
            return true;
        }
    }

    public UIScrollablePanel ScrollPanel
    {
        get
        {
            return this.mapStreamer.ScrollPanel;
        }
    }

    protected override void ViewLoad(object[] parameters)
    {
        this.UpdateNewLevelDatabaseIfAvailable();
        base.ViewLoad(parameters);
        this.mapStreamer.Initialize(this.mapStreamer.MapIdentifier, this.MapStreamerCollection, this.GetMapStreamerDataProvider, null);
        this.mapStreamer.MapDotClicked += this.HandleScrollPanelClicked;
        this.mapAvatarController.Initialize(this.GetMapAvatarDataRetriever());
        if (parameters.Length == 0)
        {
            this.levelToShowAtStartup = LevelProxy.Invalid;
            this.levelToFocus = this.FarthestUnlockedLevel;
            this.lastLevelPlayedType = null;
            this.didProgress = false;
        }
        if (parameters.Length >= 1 && parameters[0] is MapViewBase.MapViewInitData)
        {
            this.initData = (MapViewBase.MapViewInitData)parameters[0];
            this.levelToShowAtStartup = this.initData.levelToShowAtStartup;
            this.levelToFocus = this.initData.levelToFocus;
            this.didProgress = this.initData.didProgress;
            this.lastLevelPlayedType = this.initData.lastLevelPlayedType;
        }
        else if (parameters.Length < 1 || !(parameters[0] is MapViewBase.MapViewInitData))
        {
        }
        if (parameters.Length >= 2)
        {
        }
        this.allLevelsCompleted = (this.FarthestUnlockedLevel == this.FarthestCompletedLevel);
        this.ScrollPanel.CustomScrollChecker = delegate (Vector2 s)
        {
            s.y = Mathf.Min(s.y, -this.ScrollPanel.Size.y * 0.5f);
            return s;
        };
        ActivityManager.onResumeEvent += this.OnApplicationWillEnterForeground;
        GC.Collect();
        Resources.UnloadUnusedAssets();
        LevelStub.UnloadAllLevels();
    }

    protected override void ViewWillAppear()
    {
        base.ViewWillAppear();
        this.UpdateDots();
        this.ShowMapButtonViewInternal();
        this.ScrollPanel.pixelPerfectCamera = base.ViewCamera;
        if (this.didProgress && this.LastPlayedLevelBelongsToCurrentMap() && !this.allLevelsCompleted)
        {
            LevelProxy levelProxy = (!this.levelToShowAtStartup.IsValid) ? this.levelToFocus.PreviousLevel : this.levelToShowAtStartup.PreviousLevel;
            this.mapAvatarController.UpdatePlayerAvatar(levelProxy.Index);
            this.mapAvatarController.LockPlayerAvatarPositionUntilAnimation();
        }
        else
        {
            this.mapAvatarController.UpdatePlayerAvatar(-1);
        }
        this.mapAvatarController.UpdateOtherAvatars(-1);
        this.RefocusCamera();
    }

    protected override void ViewWillDisappear()
    {
        base.ViewWillDisappear();
        this.mapStreamer.MapDotClicked -= this.HandleScrollPanelClicked;
        if (!this.startupSequenceFiber.IsTerminated)
        {
            this.startupSequenceFiber.Terminate();
        }
        ActivityManager.onResumeEvent -= this.OnApplicationWillEnterForeground;
    }
    

    public override Vector2 CalculateViewSizeForScreen(Vector2 screenSize)
    {
        float num = screenSize.Aspect();
        Vector2 originalSize = base.OriginalSize;
        if (num > 1f)
        {
            originalSize.x = 960f;
            originalSize.y = originalSize.x / num;
        }
        else
        {
            originalSize.x = 640f;
            originalSize.y = originalSize.x / num;
        }
        return originalSize;
    }

    public void ResetLevelInitializationData()
    {
        this.initData = null;
        this.levelToShowAtStartup = LevelProxy.Invalid;
        this.levelToFocus = LevelProxy.Invalid;
        this.didProgress = false;
        this.lastLevelPlayedType = null;
    }

    protected void RefocusCamera()
    {
        if (this.levelToShowAtStartup.IsValid && this.LastPlayedLevelBelongsToCurrentMap())
        {
            this.mapStreamer.FocusCameraOnLevel(this.levelToShowAtStartup.Index);
        }
        else if (this.levelToFocus.IsValid)
        {
            this.mapStreamer.FocusCameraOnLevel(this.levelToFocus.Index);
        }
        else
        {
            this.mapStreamer.FocusCameraOnLevel(this.FarthestUnlockedLevel.Index);
        }
    }

    public void FocusOnLevel(int dotIndex)
    {
        this.mapStreamer.FocusCameraOnLevel(dotIndex);
    }

    public IEnumerator FocusOnLevel(int dotIndex, float duration, AnimationCurve curve = null)
    {
        yield return this.mapStreamer.FocusCameraOnLevel(dotIndex, duration, curve);
        yield break;
    }

    public void UpdateDots()
    {
        MapDotBase[] componentsInChildren = base.transform.GetComponentsInChildren<MapDotBase>(true);
        foreach (MapDotBase mapDotBase in componentsInChildren)
        {
            mapDotBase.UpdateUI();
        }
    }

    private void InitializeDots()
    {
        foreach (MapDotBase mapDotBase in base.transform.GetComponentsInChildren<MapDotBase>(true))
        {
            mapDotBase.Initialize();
        }
    }

    protected void UpdatePlayerAvatar(int overrideId = -1)
    {
        this.mapAvatarController.UpdatePlayerAvatar(overrideId);
    }

    protected void UpdateOtherAvatars(int overrideId)
    {
        this.mapAvatarController.UpdateOtherAvatars(overrideId);
    }

    protected void UpdateOtherAvatars()
    {
        this.mapAvatarController.UpdateOtherAvatars(-1);
    }

    protected void UpdateAllAvatars()
    {
        this.mapAvatarController.UpdateAllAvatars();
    }

    private void UpdateNewLevelDatabaseIfAvailable()
    {
        this.LevelDatabaseCollection.UpdateLevelDatabasesIfAvailable();
        this.MapStreamerCollection.UpdateLevelDatabasesIfAvailable();
    }

    protected IEnumerator MoveAvatar(LevelProxy source, LevelProxy dest, MapAvatar avatar)
    {
        yield return this.mapAvatarController.MoveAvatar(source, dest, avatar);
        yield break;
    }

    public IEnumerator MovePlayerAvatar(LevelProxy source, LevelProxy dest)
    {
        yield return this.mapAvatarController.MoveAvatar(source, dest, this.mapAvatarController.playerAvatar);
        yield break;
    }

    protected IEnumerator HandleOutOfLives()
    {
        UIViewManager.UIViewState vs = this.ShowNoMoreLivesView();
        if (vs == null)
        {
            yield break;
        }
        yield return vs.WaitForClose();
        if (PuzzleGameData.PlayerState.Lives <= 0)
        {
            yield return this.ShowCrossPromotionView();
        }
        yield break;
    }

    protected void Update()
    {
        IUIView buttonView = this.ButtonView;
        if (buttonView != null && UIViewManager.Instance.IsEscapeKeyDownAndAvailable(buttonView.gameObject.layer))
        {
            this.OnAndroidBackButton();
        }
        this.OnUpdate();
    }

    protected void LateUpdate()
    {
        this.OnLateUpdate();
    }

    protected virtual void OnUpdate()
    {
    }

    protected virtual void OnLateUpdate()
    {
    }

    protected virtual void OnApplicationWillEnterForeground()
    {
    }

    protected virtual void OnFacebookLoginSync()
    {
    }

    protected virtual IEnumerator ShowCrossPromotionView()
    {
        yield break;
    }

    public virtual void UpdateAll()
    {
        this.UpdateDots();
        this.UpdateAllAvatars();
    }

    protected override void ViewDidAppear()
    {
        base.ViewDidAppear();
        if (this.startupSequenceFiber.IsTerminated)
        {
            this.RunStartupSequence();
        }
        this.InitializeDots();
    }

    public void RunStartupSequence()
    {
        this.startupSequenceFiber.Start(this.StartupSequenceRunner());
    }

    private IEnumerator StartupSequenceRunner()
    {
        if (this.PreStartupSequenceCustomLogic != null)
        {
            yield return this.PreStartupSequenceCustomLogic();
        }
        yield return this.StartupSequence();
        this.startupSequenceWasShown = true;
        this.ResetLevelInitializationData();
        yield break;
    }

    protected void ShowMapButtonViewInternal()
    {
        this.buttonLayerViewState = this.ShowMapButtonView();
    }

    protected abstract LevelProxy FarthestUnlockedLevel { get; }

    protected abstract LevelProxy FarthestCompletedLevel { get; }

    protected abstract void OnAndroidBackButton();

    protected abstract void HandleScrollPanelClicked(MapDotBase mapDot);

    protected abstract MapAvatarController.IDataRetriever GetMapAvatarDataRetriever();

    protected abstract IEnumerator StartupSequence();

    protected abstract bool LastPlayedLevelBelongsToCurrentMap();

    protected abstract LevelDatabaseCollection LevelDatabaseCollection { get; }

    protected abstract MapStreamerCollection MapStreamerCollection { get; }

    protected abstract UIViewManager.UIViewState ShowMapButtonView();

    protected virtual MapStreamer.IDataProvider GetMapStreamerDataProvider
    {
        get
        {
            return null;
        }
    }

    protected abstract void SubscribeToVIPStateChange(Action<bool> callback);

    protected abstract void UnsubscribeToVIPStateChange(Action<bool> callback);

    protected abstract UIViewManager.UIViewState ShowNoMoreLivesView();

    [SerializeField]
    public MapStreamer mapStreamer;

    [SerializeField]
    public MapAvatarController mapAvatarController;

    public Func<IEnumerator> PreStartupSequenceCustomLogic;

    protected LevelProxy levelToShowAtStartup = LevelProxy.Invalid;

    protected LevelProxy levelToFocus = LevelProxy.Invalid;

    private MapViewBase.MapViewInitData initData;

    private UIViewManager.UIViewState buttonLayerViewState;

    protected readonly Fiber startupSequenceFiber = new Fiber();

    public class MapViewInitData
    {
        public MapViewInitData()
        {
        }

        public MapViewInitData(LevelProxy levelToShowAtStartup, LevelProxy levelToFocus, string lastLevelPlayedType, bool didProgress)
        {
            this.levelToShowAtStartup = levelToShowAtStartup;
            this.levelToFocus = levelToFocus;
            this.lastLevelPlayedType = lastLevelPlayedType;
            this.didProgress = didProgress;
        }

        public LevelProxy levelToShowAtStartup;

        public LevelProxy levelToFocus;

        public string lastLevelPlayedType;

        public bool didProgress;
    }
}
