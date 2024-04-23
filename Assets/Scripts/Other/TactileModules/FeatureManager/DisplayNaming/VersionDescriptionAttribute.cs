using System;

namespace TactileModules.FeatureManager.DisplayNaming
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class VersionDescriptionAttribute : Attribute
	{
		public VersionDescriptionAttribute(string description)
		{
			this.description = description;
		}

		public readonly string description;
	}
}
