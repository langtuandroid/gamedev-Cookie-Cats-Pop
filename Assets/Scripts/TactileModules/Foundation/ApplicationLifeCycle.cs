using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TactileModules.Foundation
{
	public static class ApplicationLifeCycle
	{
		static ApplicationLifeCycle()
		{
			ApplicationLifeCycle.RegisterEventHandlers();
		}

		[Obsolete("Use IApplicationLifeCycleEvents instead")]
		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action ApplicationWillEnterForeground;

		public static bool IsApplicationInitialized { get; internal set; }

		private static void RegisterEventHandlers()
		{
			if (ApplicationLifeCycle._003C_003Ef__mg_0024cache0 == null)
			{
				ApplicationLifeCycle._003C_003Ef__mg_0024cache0 = new Action(ApplicationLifeCycle.AppWillEnterForeground);
			}
			ActivityManager.onResumeEvent += ApplicationLifeCycle._003C_003Ef__mg_0024cache0;
		}

		private static void AppWillEnterForeground()
		{
			if (ApplicationLifeCycle.ApplicationWillEnterForeground != null)
			{
				ApplicationLifeCycle.ApplicationWillEnterForeground();
			}
		}

		[CompilerGenerated]
		private static Action _003C_003Ef__mg_0024cache0;
	}
}
