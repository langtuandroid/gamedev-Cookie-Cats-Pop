using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class HashableAttribute : Attribute
{
	public HashableAttribute(object defaultValue = null)
	{
		this.defaultValue = defaultValue;
	}

	public readonly object defaultValue;
}
