using System;
using System.Collections.Generic;

namespace CollectionUtils
{
	public class IntComparer : IEqualityComparer<int>
	{
		private IntComparer()
		{
		}

		public bool Equals(int x, int y)
		{
			return x == y;
		}

		public int GetHashCode(int obj)
		{
			return obj;
		}

		public static readonly IntComparer Instance = new IntComparer();
	}
}
