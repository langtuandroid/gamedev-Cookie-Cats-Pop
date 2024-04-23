using System;

namespace ConfigSchema
{
	[AttributeUsage(AttributeTargets.Property)]
	public class ArrayFormatAttribute : Attribute
	{
		public ArrayFormatAttribute(ArrayFormatAttribute.ArrayFormat arrayFormat)
		{
			this.format = arrayFormat;
		}

		public readonly ArrayFormatAttribute.ArrayFormat format;

		public enum ArrayFormat
		{
			tabs,
			table
		}
	}
}
