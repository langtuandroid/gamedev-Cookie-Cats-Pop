using System;

namespace TactileModules.RuntimeTools
{
	public static class SortUtils
	{
		public static int AscendingSort(int a, int b)
		{
			return a - b;
		}

		public static int DescendingSort(int a, int b)
		{
			return b - a;
		}
	}
}
