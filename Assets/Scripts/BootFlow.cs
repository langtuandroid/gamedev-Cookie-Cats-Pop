using System;
using System.Collections;
using Fibers;
using TactileModules.AgeInfo;
using TactileModules.Foundation;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.SagaCore;

public class BootFlow : Flow
{
	private LevelDatabaseCollection LevelDatabaseCollection
	{
		get
		{
			return ManagerRepository.Get<LevelDatabaseCollection>();
		}
	}

	protected override IEnumerator Logic()
	{
		Boot.IsRequestsBlocked += true;
		yield return new Fiber.OnExit(delegate()
		{
			Boot.IsRequestsBlocked += false;
		});
		yield return ManagerRepository.Get<AgeInfoManager>().TryShowPopup();
		LevelProxy firstLevel = this.LevelDatabaseCollection.GetLevelDatabase<MainLevelDatabase>("Main").GetLevel(0);
        if (firstLevel.IsCompleted)
        {
            UIViewManager.Instance.FadeAndSwitchViewWithAnimation<IntroView>(new object[0]);
            yield return UIViewManager.Instance.WaitForFadeDown();
            yield return UIViewManager.Instance.WaitForFadeUp();
            IntroView v = UIViewManager.Instance.FindView<IntroView>();
            bool isClosed = false;
            Action<BootFlow.IntroResult> introViewClosed = delegate (BootFlow.IntroResult r)
            {
                isClosed = true;
            };
            IntroView introView = v;
            introView.ViewClosed = (Action<BootFlow.IntroResult>)Delegate.Combine(introView.ViewClosed, introViewClosed);
            while (!isClosed)
            {
                yield return null;
            }
            IntroView introView2 = v;
            introView2.ViewClosed = (Action<BootFlow.IntroResult>)Delegate.Remove(introView2.ViewClosed, introViewClosed);
            MainMapFlowFactory mainLevels = ManagerRepository.Get<MainMapFlowFactory>();
            FlowStack flowStack = ManagerRepository.Get<FlowStack>();
            flowStack.Push(mainLevels.CreateMainMapFlow());
        }
        else
        {
            Flow.Enqueue(new StoryFlow(firstLevel));
        }
        yield break;
	}

	public enum IntroResult
	{
		Play,
		PlayDeveloper
	}
}
