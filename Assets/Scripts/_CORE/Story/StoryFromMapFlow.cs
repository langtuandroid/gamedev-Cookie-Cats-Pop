using System;
using System.Collections;
using Fibers;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.SagaCore;

public class StoryFromMapFlow : IFlow, INextMapDot, IFiberRunnable
{
	public IEnumerator Run()
	{
		LevelProxy level = MainProgressionManager.Instance.GetDatabase().GetLevel(0);
		Flow.Enqueue(new StoryFlow(level));
		yield break;
	}

	public void OnExit()
	{
	}

	public int NextDotIndexToOpen
	{
		get
		{
			return 1;
		}
	}
}
