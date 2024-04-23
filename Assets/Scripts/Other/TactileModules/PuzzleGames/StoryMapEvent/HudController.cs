using System;
using System.Collections;
using Fibers;
using Tactile;
using Tactile.GardenGame.MapSystem;
using Tactile.GardenGame.Story;
using TactileModules.ComponentLifecycle;
using TactileModules.PuzzleGames.GameCore;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	public class HudController : ComponentLifecycleHandler<StoryMapEventHud>
	{
		public HudController(IFlowStack flowStack, BrowseTasksFactory browseTasksFactory, IStoryManager storyManager, InventoryManager inventoryManager, ViewFactory viewFactory, UIViewManager uiViewManager) : base(ComponentLifecycleHandler<StoryMapEventHud>.InitializationTiming.Start)
		{
			this.flowStack = flowStack;
			this.browseTasksFactory = browseTasksFactory;
			this.storyManager = storyManager;
			this.inventoryManager = inventoryManager;
			this.viewFactory = viewFactory;
			this.uiViewManager = uiViewManager;
		}

		private IEnumerator ShowTaskViewIfNecessary(MainMapState arg1, EnumeratorResult<bool> breakSequence)
		{
			int num = 0;
			int amount = this.inventoryManager.GetAmount("Star");
			foreach (MapTask mapTask in this.storyManager.GetActiveTasks())
			{
				if (amount >= mapTask.StarsRequired)
				{
					num++;
				}
			}
			if (num > 0 && this.storyManager.TotalPagesCollected > 0)
			{
				this.HandleClickedTasks();
				breakSequence.value = true;
			}
			yield break;
		}

		protected override void InitializeComponent(StoryMapEventHud buttonInstantiator)
		{
			buttonInstantiator.ClickedExit += this.HandleClickedExit;
			buttonInstantiator.ClickedTasks += this.HandleClickedTasks;
			buttonInstantiator.ClickedPlay += this.HandlePlayForMoreStars;
			MainMapState mainMapState = this.flowStack.Find<MainMapState>();
			mainMapState.StartSequenceHooks.Register(new Func<MainMapState, EnumeratorResult<bool>, IEnumerator>(this.ShowTaskViewIfNecessary));
		}

		private void HandleClickedTasks()
		{
			bool flag = this.storyManager.GetActiveTasks().Count > 0;
			if (flag)
			{
				this.StartBrowseTasksFlow();
			}
			else
			{
				this.ShowEndOfContentView();
			}
		}

		private void StartBrowseTasksFlow()
		{
			BrowseTasksFlow c = this.browseTasksFactory.CreateBrowseTasksFlow(new Action(this.HandlePlayForMoreStars));
			this.flowStack.Push(c);
			this.storyManager.BrowseTaskStart();
		}

		private void ShowEndOfContentView()
		{
			StoryMapEventEndOfContentView view = this.viewFactory.CreateEndOfContentView();
			view.ClickedContinue += delegate()
			{
				view.Close(0);
			};
			this.uiViewManager.ShowViewInstance<StoryMapEventEndOfContentView>(view, new object[0]);
		}

		private void HandlePlayForMoreStars()
		{
			this.storyManager.NotEnoughStarsPlayedWasClicked();
			this.HandleClickedExit();
		}

		private void HandleClickedExit()
		{
			MainMapState mainMapState = this.flowStack.Find<MainMapState>();
			mainMapState.EndFlow();
		}

		private readonly IFlowStack flowStack;

		private readonly BrowseTasksFactory browseTasksFactory;

		private readonly IStoryManager storyManager;

		private readonly InventoryManager inventoryManager;

		private readonly ViewFactory viewFactory;

		private readonly UIViewManager uiViewManager;
	}
}
