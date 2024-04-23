using System;
using System.Reflection;

namespace TactileModules.RuntimeTools
{
	internal static class AssemblyUtilities
	{
		public static Assembly GetPlayAssembly()
		{
			return typeof(AssemblyUtilities).Assembly;
		}
	}
}
