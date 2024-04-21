using System;
using System.Collections;
using Fibers;

namespace TactileModules.Placements
{
	public interface IPlacementRunnableCanBreak : IPlacementRunnable
	{
		IEnumerator Run(IPlacementViewMediator placementViewMediator, EnumeratorResult<bool> breakFlow);
	}
}
