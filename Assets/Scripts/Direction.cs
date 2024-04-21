using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct Direction
{
	public Direction(int value)
	{
		this = default(Direction);
		this.Value = value;
	}

	public int Value { get; private set; }

	public Direction Opposite()
	{
		return new Direction((this.Value + 180) % 360);
	}
}
