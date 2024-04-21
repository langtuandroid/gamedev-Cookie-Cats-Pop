using System;

namespace TactileModules.TactileLogger
{
	public struct Category
	{
		public Category(string ID)
		{
			this.ID = ID;
		}

		public override string ToString()
		{
			return this.ID;
		}

		public static implicit operator string(Category category)
		{
			return category.ID;
		}

		public static implicit operator Category(string ID)
		{
			return new Category(ID);
		}

		public static bool operator ==(Category a, Category b)
		{
			return a.ID == b.ID;
		}

		public static bool operator !=(Category a, Category b)
		{
			return !(a == b);
		}

		private bool Equals(Category other)
		{
			return string.Equals(this.ID, other.ID);
		}

		public override bool Equals(object obj)
		{
			return !object.ReferenceEquals(null, obj) && obj is Category && this.Equals((Category)obj);
		}

		public override int GetHashCode()
		{
			return ((this.ID == null) ? 0 : this.ID.GetHashCode()) * 397;
		}

		public static readonly string Ads = "Ads";

		public static string CrossPromotion = "CrossPromotion";

		public static readonly Category Empty = new Category(null);

		public string ID;
	}
}
