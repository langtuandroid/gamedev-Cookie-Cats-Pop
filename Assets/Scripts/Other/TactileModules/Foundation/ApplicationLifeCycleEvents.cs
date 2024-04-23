using System;
using System.Diagnostics;

namespace TactileModules.Foundation
{
	public class ApplicationLifeCycleEvents : IApplicationLifeCycleEvents
	{
		public ApplicationLifeCycleEvents()
		{
			this.RegisterEventHandlers();
		}

		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action ApplicationWillEnterForeground = delegate ()
        {
        };



        ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public event Action BootCompleted = delegate ()
        {
        };



        private void RegisterEventHandlers()
		{
			ActivityManager.onResumeEvent += this.AppWillEnterForeground;
		}

		private void AppWillEnterForeground()
		{
			this.ApplicationWillEnterForeground();
		}

		public void BootWasCompleted()
		{
			this.BootCompleted();
		}
	}
}
