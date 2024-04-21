using System;

namespace TactileModules.Foundation
{
	public interface IApplicationLifeCycleEvents
	{
		event Action ApplicationWillEnterForeground;

		event Action BootCompleted;
	}
}
