using System;
using System.Collections;
using System.Collections.Generic;

public static class MiniJsonExtensions
{
	public static string toJson(this Hashtable obj)
	{
		return MiniJSON.jsonEncode(obj, false, 0);
	}

	public static string toPrettyJson(this Hashtable obj)
	{
		return MiniJSON.jsonEncode(obj, true, 4);
	}

	public static string toJson(this ArrayList obj)
	{
		return MiniJSON.jsonEncode(obj, false, 0);
	}

	public static string toPrettyJson(this ArrayList obj)
	{
		return MiniJSON.jsonEncode(obj, true, 4);
	}

	public static string toJson(this Dictionary<string, string> obj)
	{
		return MiniJSON.jsonEncode(obj, false, 0);
	}

	public static string toPrettyJson(this Dictionary<string, string> obj)
	{
		return MiniJSON.jsonEncode(obj, true, 4);
	}

	public static ArrayList arrayListFromJson(this string json)
	{
		return MiniJSON.jsonDecode(json) as ArrayList;
	}

	public static Hashtable hashtableFromJson(this string json)
	{
		return MiniJSON.jsonDecode(json) as Hashtable;
	}
}
