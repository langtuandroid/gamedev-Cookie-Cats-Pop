using System;
using System.Collections.Generic;

namespace TactileModules.SideMapButtons
{
	public interface ISideMapButtonControllerProvider
	{
		List<ISideMapButtonController> CreateButtonControllers();
	}
}
