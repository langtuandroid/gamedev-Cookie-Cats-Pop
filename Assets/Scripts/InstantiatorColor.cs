using System;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct InstantiatorColor
{
	[JsonSerializable("R", null)]
	public float R { get; set; }

	[JsonSerializable("G", null)]
	public float G { get; set; }

	[JsonSerializable("B", null)]
	public float B { get; set; }

	[JsonSerializable("A", null)]
	public float A { get; set; }

	public static implicit operator InstantiatorColor(Color val)
	{
		return new InstantiatorColor
		{
			R = val.r,
			G = val.g,
			B = val.b,
			A = val.a
		};
	}

	public static implicit operator Color(InstantiatorColor t)
	{
		return new Color(t.R, t.G, t.B, t.A);
	}
}
