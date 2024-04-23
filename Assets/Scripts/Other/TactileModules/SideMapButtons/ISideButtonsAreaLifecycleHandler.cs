using System;

namespace TactileModules.SideMapButtons
{
	public interface ISideButtonsAreaLifecycleHandler
	{
		event Action<ISideMapButtonController> SideMapButtonControllerCreated;
	}
}
