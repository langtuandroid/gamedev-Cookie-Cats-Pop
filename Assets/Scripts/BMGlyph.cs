using System;
using System.Collections.Generic;

[Serializable]
public class BMGlyph
{
	public int GetKerning(int previousChar)
	{
		if (this.kerning != null)
		{
			int i = 0;
			int count = this.kerning.Count;
			while (i < count)
			{
				BMGlyph.Kerning kerning = this.kerning[i];
				if (kerning.previousChar == previousChar)
				{
					return kerning.amount;
				}
				i++;
			}
		}
		return 0;
	}

	public void SetKerning(int previousChar, int amount)
	{
		if (this.kerning == null)
		{
			this.kerning = new List<BMGlyph.Kerning>();
		}
		for (int i = 0; i < this.kerning.Count; i++)
		{
			if (this.kerning[i].previousChar == previousChar)
			{
				BMGlyph.Kerning value = this.kerning[i];
				value.amount = amount;
				this.kerning[i] = value;
				return;
			}
		}
		BMGlyph.Kerning item = default(BMGlyph.Kerning);
		item.previousChar = previousChar;
		item.amount = amount;
		this.kerning.Add(item);
	}

	public void Trim(int xMin, int yMin, int xMax, int yMax)
	{
		int num = this.x + this.width;
		int num2 = this.y + this.height;
		if (this.x < xMin)
		{
			int num3 = xMin - this.x;
			this.x += num3;
			this.width -= num3;
			this.offsetX += num3;
		}
		if (this.y < yMin)
		{
			int num4 = yMin - this.y;
			this.y += num4;
			this.height -= num4;
			this.offsetY += num4;
		}
		if (num > xMax)
		{
			this.width -= num - xMax;
		}
		if (num2 > yMax)
		{
			this.height -= num2 - yMax;
		}
	}

	public int index;

	public int x;

	public int y;

	public int width;

	public int height;

	public int offsetX;

	public int offsetY;

	public int advance;

	public List<BMGlyph.Kerning> kerning;

	public struct Kerning
	{
		public int previousChar;

		public int amount;
	}
}
