using System;
using System.Collections.Generic;
using TactileModules.RuntimeTools.Random;
using UnityEngine;

public static class ListExtensions
{
	public static T GetRandom<T>(this List<T> list)
	{
		if (list.Count > 0)
		{
			return list[UnityEngine.Random.Range(0, list.Count)];
		}
		return default(T);
	}

	public static T GetRandom<T>(this List<T> list, IRandom random)
	{
		if (list.Count > 0)
		{
			return list[random.Range(0, list.Count)];
		}
		return default(T);
	}
}
