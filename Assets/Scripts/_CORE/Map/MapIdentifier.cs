using System;

[Serializable]
public struct MapIdentifier
{
	public MapIdentifier(string ID)
	{
		this.ID = ID;
	}

	public override string ToString()
	{
		return this.ID;
	}

	public static implicit operator string(MapIdentifier mapIdentifier)
	{
		return mapIdentifier.ID;
	}

	public static implicit operator MapIdentifier(string ID)
	{
		return new MapIdentifier(ID);
	}

	public static bool operator ==(MapIdentifier a, MapIdentifier b)
	{
		return a.ID == b.ID;
	}

	public static bool operator !=(MapIdentifier a, MapIdentifier b)
	{
		return !(a == b);
	}

	private bool Equals(MapIdentifier other)
	{
		return string.Equals(this.ID, other.ID);
	}

	public override bool Equals(object obj)
	{
		return !object.ReferenceEquals(null, obj) && obj is MapIdentifier && this.Equals((MapIdentifier)obj);
	}

	public override int GetHashCode()
	{
		return ((this.ID == null) ? 0 : this.ID.GetHashCode()) * 397;
	}

	public const string EndlessChallenge = "EndlessChallenge";

	public static readonly MapIdentifier Empty = new MapIdentifier(null);

	public string ID;

	public const string Main = "Main";

	public const string OneLifeChallenge = "OneLifeChallenge";

	public const string PlayablePostcard = "PlayablePostcard";

	public const string SlidesAndLadders = "SlidesAndLadders";

	public const string Tournament = "Tournament";

	public const string TreasureHunt = "TreasureHunt";

	public const string Daily = "Daily";
}
