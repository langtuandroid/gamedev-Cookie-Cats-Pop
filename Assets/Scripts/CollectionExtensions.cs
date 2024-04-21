using System;
using System.Collections.Generic;
using System.Reflection;
using TactileModules.RuntimeTools.Random;

public static class CollectionExtensions
{
	public static void Shuffle<T>(this IList<T> list)
	{
		list.Shuffle(CollectionExtensions.defaultRandom);
	}

	public static void ShuffleNoSameIndices<T>(this IList<T> list)
	{
		list.ShuffleNoSameIndices(CollectionExtensions.defaultRandom);
	}

	public static void Shuffle<T>(this IList<T> list, IRandom random)
	{
		for (int i = 0; i < list.Count - 1; i++)
		{
			int index = random.Range(i, list.Count);
			list.Swap(i, index);
		}
	}

	public static void ShuffleNoSameIndices<T>(this IList<T> list, IRandom random)
	{
		for (int i = 0; i < list.Count - 1; i++)
		{
			int index = random.Range(i + 1, list.Count);
			list.Swap(i, index);
		}
	}

	public static T First<T>(this IList<T> list)
	{
		return list[0];
	}

	public static T Last<T>(this IList<T> list)
	{
		if (list.Count == 0)
		{
			throw new Exception("Trying to call Last on an empty collection.");
		}
		return list[list.Count - 1];
	}

	public static V GetValueOrDefault<K, V>(this IDictionary<K, V> dict, K key)
	{
		V result = default(V);
		dict.TryGetValue(key, out result);
		return result;
	}

	public static void IncreaseSafely<K>(this IDictionary<K, int> dict, K key, int byAmount = 1)
	{
		if (dict.ContainsKey(key))
		{
			dict[key] += byAmount;
		}
		else
		{
			dict[key] = byAmount;
		}
	}

	public static void Swap<T>(this IList<T> list, T element0, T element1)
	{
		int num = list.IndexOf(element0);
		int num2 = list.IndexOf(element1);
		if (num < 0 || num2 < 0)
		{
			return;
		}
		list[num] = element1;
		list[num2] = element0;
	}

	public static void Swap<T>(this IList<T> list, int index0, int index1)
	{
		T value = list[index0];
		list[index0] = list[index1];
		list[index1] = value;
	}

	public static List<T> Filter<T>(this IList<T> list, Func<T, bool> method)
	{
		List<T> list2 = new List<T>();
		foreach (T t in list)
		{
			if (method(t))
			{
				list2.Add(t);
			}
		}
		return list2;
	}

	public static void GetConstNamesAndValues<T, V>(out List<string> names, out List<V> values)
	{
		Type typeFromHandle = typeof(T);
		CollectionExtensions.GetConstNamesAndValues<V>(typeFromHandle, out names, out values);
	}

	public static void GetConstNamesAndValues<V>(Type type, out List<string> names, out List<V> values)
	{
		List<FieldInfo> list = new List<FieldInfo>();
		FieldInfo[] fields = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
		foreach (FieldInfo fieldInfo in fields)
		{
			if (fieldInfo.IsLiteral && !fieldInfo.IsInitOnly)
			{
				list.Add(fieldInfo);
			}
		}
		names = new List<string>();
		values = new List<V>();
		int num = 0;
		foreach (FieldInfo fieldInfo2 in list)
		{
			names.Add(fieldInfo2.Name);
			values.Add((V)((object)fieldInfo2.GetRawConstantValue()));
			num++;
		}
	}

	public static List<string> GetNonEmptyConstStringValues(Type type)
	{
		List<string> list;
		List<string> list2;
		CollectionExtensions.GetConstNamesAndValues<string>(type, out list, out list2);
		list2.RemoveAll((string s) => string.IsNullOrEmpty(s));
		return list2;
	}

	public static void GetStaticReadonlyNamesAndValues<T>(out List<string> names, out List<T> values)
	{
		Type typeFromHandle = typeof(T);
		List<FieldInfo> list = new List<FieldInfo>();
		FieldInfo[] fields = typeFromHandle.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
		foreach (FieldInfo fieldInfo in fields)
		{
			if (!fieldInfo.IsLiteral && fieldInfo.IsInitOnly)
			{
				list.Add(fieldInfo);
			}
		}
		names = new List<string>();
		values = new List<T>();
		int num = 0;
		foreach (FieldInfo fieldInfo2 in list)
		{
			names.Add(fieldInfo2.Name);
			T item = (T)((object)fieldInfo2.GetValue(null));
			values.Add(item);
			num++;
		}
	}

	public static void AddNewAndPruneOld<T>(this List<T> current, List<T> newList, Func<T, T, bool> compareFunc) where T : class
	{
		List<T> list = new List<T>(current);
		using (List<T>.Enumerator enumerator = newList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				T n = enumerator.Current;
				T t = current.Find((T item) => compareFunc(item, n));
				if (t != null)
				{
					list.Remove(t);
				}
				else
				{
					current.Add(n);
				}
			}
		}
		foreach (T item2 in list)
		{
			current.Remove(item2);
		}
	}

	public static int Count<T>(this IEnumerable<T> source)
	{
		int num = 0;
		using (IEnumerator<T> enumerator = source.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				num++;
			}
		}
		return num;
	}

	public static List<T2> Map<T1, T2>(this List<T1> list, Func<T1, T2> convert)
	{
		List<T2> list2 = new List<T2>();
		foreach (T1 arg in list)
		{
			list2.Add(convert(arg));
		}
		return list2;
	}

	public static int[] Range(int start, int count)
	{
		int[] array = new int[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = start + i;
		}
		return array;
	}

	private static IRandom defaultRandom = new UnityRandom();
}
