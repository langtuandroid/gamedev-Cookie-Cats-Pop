using System;

namespace ConfigSchema
{
	[AttributeUsage(AttributeTargets.Property)]
	public class IgnorePropertyAttribute : Attribute
	{
	}
}
