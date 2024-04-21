using System;

namespace TactileModules.GameCore.ButtonArea
{
	[Serializable]
	public struct ButtonAreaCategory
	{
		public ButtonAreaCategory(string ID)
		{
			this.ID = ID;
		}

		public override string ToString()
		{
			return this.ID;
		}

		public static implicit operator string(ButtonAreaCategory buttonLocation)
		{
			return buttonLocation.ID;
		}

		public static implicit operator ButtonAreaCategory(string ID)
		{
			return new ButtonAreaCategory(ID);
		}

		public static bool operator ==(ButtonAreaCategory a, ButtonAreaCategory b)
		{
			return a.ID == b.ID;
		}

		public static bool operator !=(ButtonAreaCategory a, ButtonAreaCategory b)
		{
			return !(a == b);
		}

		private bool Equals(ButtonAreaCategory other)
		{
			return string.Equals(this.ID, other.ID);
		}

		public override bool Equals(object obj)
		{
			return !object.ReferenceEquals(null, obj) && obj is ButtonAreaCategory && this.Equals((ButtonAreaCategory)obj);
		}

		public override int GetHashCode()
		{
			return ((this.ID == null) ? 0 : this.ID.GetHashCode()) * 397;
		}

		public const string None = "";

		public const string Lives = "Lives";

		public const string Coins = "Coins";

		public const string Stars = "Stars";

		public const string MainLevel = "MainLevel";

		public const string Tasks = "Tasks";

		public const string Features = "Features";

		public const string MapLeft = "MapLeft";

		public const string Settings = "Settings";

		public string ID;
	}
}
