using System;
using System.Collections.Generic;

namespace ConfigSchema
{
	[AttributeUsage(AttributeTargets.Property)]
	public class PartialStructEnumAttribute : Attribute
	{
		public PartialStructEnumAttribute(Type type)
		{
			this.type = type;
		}

		public List<string> Values()
		{
			return CollectionExtensions.GetNonEmptyConstStringValues(this.type);
		}

		private Type type;
	}
}
