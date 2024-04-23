using System;

namespace SQLite4Unity3d
{
	[AttributeUsage(AttributeTargets.Property)]
	public class CollationAttribute : Attribute
	{
		public CollationAttribute(string collation)
		{
			this.Value = collation;
		}

		public string Value { get; private set; }
	}
}
