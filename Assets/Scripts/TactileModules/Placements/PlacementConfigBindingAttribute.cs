using System;
using JetBrains.Annotations;

namespace TactileModules.Placements
{
	[AttributeUsage(AttributeTargets.Property)]
	[MeansImplicitUse]
	public class PlacementConfigBindingAttribute : Attribute
	{
	}
}
