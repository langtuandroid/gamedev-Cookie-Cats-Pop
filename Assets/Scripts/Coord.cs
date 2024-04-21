using System;

public struct Coord
{
	public Coord(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public static Coord operator +(Coord c1, Coord c2)
	{
		return new Coord(c1.x + c2.x, c1.y + c2.y);
	}

	public static Coord operator -(Coord c1, Coord c2)
	{
		return new Coord(c1.x - c2.x, c1.y - c2.y);
	}

	public static bool operator ==(Coord c1, Coord c2)
	{
		return c1.x == c2.x && c1.y == c2.y;
	}

	public static bool operator !=(Coord c1, Coord c2)
	{
		return !(c1 == c2);
	}

	public override string ToString()
	{
		return string.Format("[{0},{1}]", this.x, this.y);
	}

	public int x;

	public int y;
}
