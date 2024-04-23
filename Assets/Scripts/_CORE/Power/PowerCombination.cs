using System;

public struct PowerCombination
{
	public PowerCombination(params PowerColor[] enabledColors)
	{
		this.colors = 0;
		for (int i = 0; i < enabledColors.Length; i++)
		{
			this.EnableColor(enabledColors[i]);
		}
	}

	public void EnableColor(PowerColor color)
	{
		this.colors += 1 << (int)color;
	}

	public bool IsColorEnabled(PowerColor color)
	{
		int num = 1 << (int)color;
		return (this.colors & num) == num;
	}

	public override int GetHashCode()
	{
		return this.colors;
	}

	public static bool operator ==(PowerCombination a, PowerCombination b)
	{
		return a.colors == b.colors;
	}

	public static bool operator !=(PowerCombination a, PowerCombination b)
	{
		return !(a == b);
	}

	public override bool Equals(object obj)
	{
		return obj is PowerCombination && this == (PowerCombination)obj;
	}

	public static PowerCombination None
	{
		get
		{
			return default(PowerCombination);
		}
	}

	public static PowerCombination All
	{
		get
		{
			return new PowerCombination(new PowerColor[]
			{
				PowerColor.Yellow,
				PowerColor.Red,
				PowerColor.Blue,
				PowerColor.Green
			});
		}
	}

	public int colors;
}
