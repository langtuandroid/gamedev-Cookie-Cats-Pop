using System;
using System.Collections;

namespace TactileModules.Placements
{
	public interface IPlacementRunnableNoBreak : IPlacementRunnable
	{
		IEnumerator Run(IPlacementViewMediator placementViewMediator);
	}
}
