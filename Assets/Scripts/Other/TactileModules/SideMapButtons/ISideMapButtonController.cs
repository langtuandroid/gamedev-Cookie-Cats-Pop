using System;

namespace TactileModules.SideMapButtons
{
	public interface ISideMapButtonController
	{
		bool VisibilityChecker(object data);

		ISideMapButton GetSideMapButtonInstance();
	}
}
