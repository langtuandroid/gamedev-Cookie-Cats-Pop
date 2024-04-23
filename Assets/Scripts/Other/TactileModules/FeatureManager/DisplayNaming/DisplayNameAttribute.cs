using System;

namespace TactileModules.FeatureManager.DisplayNaming
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class DisplayNameAttribute : Attribute
	{
		public DisplayNameAttribute(string displayName)
		{
			this.displayName = displayName;
		}

		public readonly string displayName;
	}
}
