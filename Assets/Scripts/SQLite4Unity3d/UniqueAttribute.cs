using System;

namespace SQLite4Unity3d
{
	[AttributeUsage(AttributeTargets.Property)]
	public class UniqueAttribute : IndexedAttribute
	{
		public UniqueAttribute()
		{
		}

		public UniqueAttribute(string name, int order) : base(name, order)
		{
		}

		public override bool Unique
		{
			get
			{
				return true;
			}
			set
			{
			}
		}
	}
}
