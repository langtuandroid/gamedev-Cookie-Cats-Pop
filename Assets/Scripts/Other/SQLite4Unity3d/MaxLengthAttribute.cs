using System;

namespace SQLite4Unity3d
{
	[AttributeUsage(AttributeTargets.Property)]
	public class MaxLengthAttribute : Attribute
	{
		public MaxLengthAttribute(int length)
		{
			this.Value = length;
		}

		public int Value { get; private set; }
	}
}
