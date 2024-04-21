using System;

namespace ConfigSchema
{
	[AttributeUsage(AttributeTargets.Property)]
	public class PlatformAttribute : Attribute
	{
		public PlatformAttribute(params PlatformAttribute.Platform[] platforms)
		{
			if (platforms.Length == 0)
			{
				throw new Exception("Amount of platforms can not be 0!");
			}
			this.Platforms = platforms;
		}

		public readonly PlatformAttribute.Platform[] Platforms;

		public enum Platform
		{
			Default,
			ios,
			android,
			fireos,
			webgl,
			windows,
			osx
		}
	}
}
