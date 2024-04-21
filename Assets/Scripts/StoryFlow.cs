using System;
using System.Collections;
using System.Collections.Generic;
using TactileModules.Foundation;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.SagaCore;

public class StoryFlow : Flow
{
    public StoryFlow(LevelProxy storyLevel)
    {
        this.storyLevel = storyLevel;
    }

    protected override IEnumerator Logic()
    {
        yield return null;
        UIViewManager.Instance.CloseAll(new IUIView[0]);
        List<SelectedBooster> boosters = new List<SelectedBooster>();
        LevelSession session = new LevelSession(this.storyLevel);
        session.SetPreGameBoosters(boosters);
        UIViewManager.Instance.ShowView<GameView>(new object[]
        {
            session
        });
        GameView gameViewTemp = UIViewManager.Instance.FindView<GameView>();
        gameViewTemp.SetPanningPosToTargetPosition();
        gameViewTemp.UnhookLevelSessionListener();
        gameViewTemp.gameHud.PauseButton.SetActive(false);
        gameViewTemp.gameObject.SetActive(false);
        UIViewManager.Instance.FrontFill = 1f;
        bool skipIntro = false;
        bool done = false;
        StoryIntroView.StoryIntroViewParameters param = new StoryIntroView.StoryIntroViewParameters
        {
            done = delegate (bool didSkip)
            {
                done = true;
                skipIntro = didSkip;
            },
            gameView = gameViewTemp,
            session = session
        };
        UIViewManager.Instance.ShowView<StoryIntroView>(new object[]
        {
            param
        });
        yield return UIViewManager.Instance.FadeCameraFrontFill(0f, 0f, 0);
        while (!done)
        {
            yield return null;
        }
        MainMapFlowFactory mainMapFlowFactory = ManagerRepository.Get<MainMapFlowFactory>();
        FlowStack flowStack = ManagerRepository.Get<FlowStack>();
        MainMapFlow mainMapFlow = mainMapFlowFactory.CreateMainMapFlow();
        if (skipIntro)
        {
            this.storyLevel.SaveSessionAccomplishment(9999, false);
            FullScreenManager fullScreenManager = ManagerRepository.Get<FullScreenManager>();
            fullScreenManager.PopInstantly();
            mainMapFlow.autoPushToFullscreen = true;
            flowStack.Push(mainMapFlow);
            yield break;
        }
        this.storyLevel.SaveSessionAccomplishment(session.Points, false);
        flowStack.Push(mainMapFlow);
        while (UIViewManager.Instance.FindView<MainMapButtonsView>() == null)
        {
            yield return null;
        }
        yield return null;
        bool isdone = false;
        StoryOutroView.StoryOutroViewParameters outroparam = new StoryOutroView.StoryOutroViewParameters
        {
            done = delegate ()
            {
                isdone = true;
            },
            mainMapFlow = mainMapFlow
        };
        UIViewManager.Instance.ShowView<StoryOutroView>(new object[]
        {
            outroparam
        });
        yield return UIViewManager.Instance.FadeCameraFrontFill(0f, 0f, 0);
        while (!isdone)
        {
            yield return null;
        }
        AudioManager.Instance.SetMusic(SingletonAsset<SoundDatabase>.Instance.mapMusic, true);
        yield return FiberHelper.Wait(0.4f, (FiberHelper.WaitFlag)0);
        UICamera.EnableInput();
        mainMapFlow.MapContentController.Avatars.ProgressMarker.gameObject.SetActive(true);
        mainMapFlow.StartFlowForDot(1);
        yield break;
    }

    private LevelProxy storyLevel;
}
