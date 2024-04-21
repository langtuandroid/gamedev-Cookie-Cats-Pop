using System;
using UnityEngine;

[Serializable]
public struct MatchFlag
{
	public static implicit operator MatchFlag(string val)
	{
		return new MatchFlag
		{
			value = val
		};
	}

	public static implicit operator string(MatchFlag t)
	{
		return t.value;
	}

	public override string ToString()
	{
		return this.value;
	}

	public static bool operator ==(MatchFlag a, MatchFlag b)
	{
		return object.ReferenceEquals(a, b) || ((object)a != (object)null && (object)b != (object)null && a.value == b.value);
	}

	public static bool operator !=(MatchFlag a, MatchFlag b)
	{
		return !(a == b);
	}

	public override bool Equals(object obj)
	{
		return obj is MatchFlag && this.value == ((MatchFlag)obj).value;
	}

	public override int GetHashCode()
	{
		if (this.value == null)
		{
			return 0;
		}
		return this.value.GetHashCode();
	}

	public const string NotMatchable = "";

	[SerializeField]
	[Hashable(null)]
	private string value;

	public const string Red = "Red";

	public const string Blue = "Blue";

	public const string Green = "Green";

	public const string Yellow = "Yellow";

	public const string Purple = "Purple";
}
