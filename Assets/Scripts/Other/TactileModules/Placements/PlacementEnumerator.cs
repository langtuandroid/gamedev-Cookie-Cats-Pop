using System;
using System.Collections.Generic;

namespace TactileModules.Placements
{
	public class PlacementEnumerator : IPlacementEnumerator
	{
		public IEnumerable<PlacementIdentifier> GetPlacements()
		{
			List<string> list = new List<string>();
			List<PlacementIdentifier> result = new List<PlacementIdentifier>();
			CollectionExtensions.GetStaticReadonlyNamesAndValues<PlacementIdentifier>(out list, out result);
			return result;
		}
	}
}
