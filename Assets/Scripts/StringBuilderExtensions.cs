using System;
using System.Reflection;
using System.Text;

public static class StringBuilderExtensions
{
	public static string InternalString(this StringBuilder sb)
	{
		return (string)sb.GetType().GetField("_str", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(sb);
	}

	public static void SetFromStringBuilder(this BetterList<char> l, StringBuilder sb)
	{
		string text = sb.InternalString();
		l.Clear();
		for (int i = 0; i < sb.Length; i++)
		{
			l.Add(text[i]);
		}
	}
}
