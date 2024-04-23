using System;
using System.Collections.Generic;

namespace TactileModules.Placements
{
	public interface IPlacementEnumerator
	{
		IEnumerable<PlacementIdentifier> GetPlacements();
	}
}
