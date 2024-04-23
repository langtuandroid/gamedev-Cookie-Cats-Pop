using System;
using UnityEngine;

public static class UITextEncoding
{
	public static Color ParseColor(string text, int offset)
	{
		int num = UIMath.HexToDecimal(text[offset]) << 4 | UIMath.HexToDecimal(text[offset + 1]);
		int num2 = UIMath.HexToDecimal(text[offset + 2]) << 4 | UIMath.HexToDecimal(text[offset + 3]);
		int num3 = UIMath.HexToDecimal(text[offset + 4]) << 4 | UIMath.HexToDecimal(text[offset + 5]);
		float num4 = 0.003921569f;
		return new Color(num4 * (float)num, num4 * (float)num2, num4 * (float)num3);
	}

	public static Color ParseColor(BetterList<char> text, int offset)
	{
		int num = UIMath.HexToDecimal(text[offset]) << 4 | UIMath.HexToDecimal(text[offset + 1]);
		int num2 = UIMath.HexToDecimal(text[offset + 2]) << 4 | UIMath.HexToDecimal(text[offset + 3]);
		int num3 = UIMath.HexToDecimal(text[offset + 4]) << 4 | UIMath.HexToDecimal(text[offset + 5]);
		float num4 = 0.003921569f;
		return new Color(num4 * (float)num, num4 * (float)num2, num4 * (float)num3);
	}

	public static string EncodeColor(Color c)
	{
		int num = 16777215 & UIMath.ColorToInt(c) >> 8;
		return UIMath.DecimalToHex(num);
	}

	public static BetterList<UITextEncoding.Block> CreateSymbolList(string text)
	{
		int num = 0;
		int length = text.Length;
		int num2 = 0;
		BetterList<UITextEncoding.Block> betterList = UITextEncoding.sharedBlockList;
		betterList.Clear();
		UITextEncoding.Block block = null;
		int num3 = 0;
		while (num < length && num3++ < 10000)
		{
			char c = text[num];
			if (block != null)
			{
				if (block.type == UITextEncoding.SymbolType.Unknown)
				{
					if (c == '@')
					{
						block.type = UITextEncoding.SymbolType.Sprite;
						num++;
						block.startIndex = num;
					}
					else if (c == '#')
					{
						block.type = UITextEncoding.SymbolType.SetColor;
						num++;
						block.startIndex = num;
					}
					else if (c == 'i')
					{
						block.type = UITextEncoding.SymbolType.Italic;
						num++;
						block.startIndex = num;
					}
				}
				else if (c == ']')
				{
					block.length = num - block.startIndex;
					betterList.Add(block);
					num++;
					num2 = num;
					block = null;
				}
				else
				{
					num++;
				}
			}
			else if (c == '[')
			{
				UITextEncoding.Block item = new UITextEncoding.Block
				{
					type = UITextEncoding.SymbolType.Text,
					startIndex = num2,
					length = num - num2
				};
				betterList.Add(item);
				block = new UITextEncoding.Block();
				num++;
			}
			else
			{
				num++;
			}
		}
		if (block == null && num > num2)
		{
			UITextEncoding.Block item2 = new UITextEncoding.Block
			{
				type = UITextEncoding.SymbolType.Text,
				startIndex = num2,
				length = num - num2
			};
			betterList.Add(item2);
		}
		return betterList;
	}

	public static int ParseSymbol(string text, int index, BetterList<Color> colors)
	{
		int length = text.Length;
		if (index + 2 < length)
		{
			if (text[index + 1] == '-')
			{
				if (text[index + 2] == ']')
				{
					if (colors != null && colors.size > 1)
					{
						colors.RemoveAt(colors.size - 1);
					}
					return 3;
				}
			}
			else if (index + 7 < length && text[index + 7] == ']')
			{
				if (colors != null)
				{
					Color item = UITextEncoding.ParseColor(text, index + 1);
					item.a = colors[colors.size - 1].a;
					colors.Add(item);
				}
				return 8;
			}
		}
		return 0;
	}

	public static int ParseSymbol(BetterList<char> text, int index, BetterList<Color> colors)
	{
		int size = text.size;
		if (index + 2 < size)
		{
			if (text[index + 1] == '-')
			{
				if (text[index + 2] == ']')
				{
					if (colors != null && colors.size > 1)
					{
						colors.RemoveAt(colors.size - 1);
					}
					return 3;
				}
			}
			else if (index + 7 < size && text[index + 7] == ']')
			{
				if (colors != null)
				{
					Color item = UITextEncoding.ParseColor(text, index + 1);
					item.a = colors[colors.size - 1].a;
					colors.Add(item);
				}
				return 8;
			}
		}
		return 0;
	}

	public static string StripSymbols(string text)
	{
		if (text != null)
		{
			text = text.Replace("\\n", "\n");
			int i = 0;
			int length = text.Length;
			while (i < length)
			{
				char c = text[i];
				if (c == '[')
				{
					int num = UITextEncoding.ParseSymbol(text, i, null);
					if (num > 0)
					{
						text = text.Remove(i, num);
						length = text.Length;
						continue;
					}
				}
				i++;
			}
		}
		return text;
	}

	private static BetterList<UITextEncoding.Block> sharedBlockList = new BetterList<UITextEncoding.Block>();

	public enum SymbolType
	{
		Unknown,
		Text,
		SetColor,
		Sprite,
		Italic
	}

	public class Block
	{
		public UITextEncoding.SymbolType type;

		public int startIndex;

		public int length;
	}
}
