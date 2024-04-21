using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

public static class StringExtensions
{
	public static Dictionary<string, string> Base64StringToDictionary(this string data)
	{
		if (!data.IsBase64String())
		{
			return null;
		}
		byte[] array = Convert.FromBase64String(data);
		string @string = Encoding.UTF8.GetString(array, 0, array.Length);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		string[] array2 = @string.Replace("{", string.Empty).Replace("}", string.Empty).Replace("\"", string.Empty).Trim().TrimEnd(new char[]
		{
			';'
		}).Split(new char[]
		{
			';'
		}, StringSplitOptions.RemoveEmptyEntries);
		foreach (string input in array2)
		{
			string text = Regex.Replace(input, "\\s+", string.Empty);
			Regex regex = new Regex("(\r\n|\r|\n)");
			text = regex.Replace(text, string.Empty);
			int num = text.IndexOf("=", StringComparison.Ordinal);
			string key = text.Substring(0, num);
			string value = text.Substring(num + 1, text.Length - num - 1);
			dictionary.Add(key, value);
		}
		return dictionary;
	}

	public static bool IsBase64String(this string s)
	{
		s = s.Trim();
		return s.Length % 4 == 0 && Regex.IsMatch(s, "^[a-zA-Z0-9\\+/]*={0,3}$", RegexOptions.None);
	}
}
