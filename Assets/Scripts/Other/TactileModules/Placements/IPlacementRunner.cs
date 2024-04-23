using System;
using System.Collections;
using Fibers;

namespace TactileModules.Placements
{
	public interface IPlacementRunner
	{
		IEnumerator Run(NonBreakablePlacementIdentifier placement);

		IEnumerator Run(BreakablePlacementIdentifier placement, EnumeratorResult<bool> wasFlowBroken);
	}
}
