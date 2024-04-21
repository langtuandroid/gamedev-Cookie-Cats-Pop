using System;
using System.Collections.Generic;

namespace com.amazon.device.iap.cpt
{
	public abstract class Jsonable
	{
		public static Dictionary<string, object> unrollObjectIntoMap<T>(Dictionary<string, T> obj) where T : Jsonable
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			foreach (KeyValuePair<string, T> keyValuePair in obj)
			{
				dictionary.Add(keyValuePair.Key, keyValuePair.Value.GetObjectDictionary());
			}
			return dictionary;
		}

		public static List<object> unrollObjectIntoList<T>(List<T> obj) where T : Jsonable
		{
			List<object> list = new List<object>();
			foreach (T t in obj)
			{
				Jsonable jsonable = t;
				list.Add(jsonable.GetObjectDictionary());
			}
			return list;
		}

		public abstract Dictionary<string, object> GetObjectDictionary();

		public static void CheckForErrors(Dictionary<string, object> jsonMap)
		{
			object obj;
			if (jsonMap.TryGetValue("error", out obj))
			{
				throw new AmazonException(obj as string);
			}
		}
	}
}
