using System;

namespace ConfigSchema
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
	public class DescriptionAttribute : Attribute
	{
		public DescriptionAttribute(string description)
		{
			this.Description = description;
		}

		public string Description { get; private set; }
	}
}
