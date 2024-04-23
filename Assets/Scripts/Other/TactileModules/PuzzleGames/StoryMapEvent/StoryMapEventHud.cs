using System;
using System.Diagnostics;
using TactileModules.ComponentLifecycle;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	public class StoryMapEventHud : LifecycleBroadcaster
	{
		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action ClickedExit;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action ClickedPlay;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action ClickedTasks;



		private void ExitClicked(UIEvent e)
		{
			this.ClickedExit();
		}

		private void PlayClicked(UIEvent e)
		{
			this.ClickedPlay();
		}

		private void TasksClicked(UIEvent e)
		{
			this.ClickedTasks();
		}
	}
}
