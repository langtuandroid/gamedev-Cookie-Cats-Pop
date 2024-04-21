using System;
using System.Diagnostics;
using Tactile.GardenGame.Story;
using TactileModules.GameCore.MainProgression;

internal class TutorialProgression : IMainProgressionModel
{
	public TutorialProgression(IStoryManager storyManager)
	{
		this.storyManager = storyManager;
		storyManager.ProgressionChanged += this.ProgressionChangedHandler;
	}

	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action ProgressChanged;



	private void ProgressionChangedHandler()
	{
		this.ProgressChanged();
	}

	public int Progress
	{
		get
		{
			int chapterProgression = this.storyManager.GetChapterProgression();
			int totalPagesCollected = this.storyManager.TotalPagesCollected;
			if (chapterProgression < 1)
			{
				return totalPagesCollected;
			}
			if (this.storyManager.State.CurrentChapter > 1)
			{
				return 999999;
			}
			return chapterProgression + 1;
		}
	}

	private readonly IStoryManager storyManager;
}
