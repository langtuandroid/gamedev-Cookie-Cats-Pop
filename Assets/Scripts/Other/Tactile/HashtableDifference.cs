using System;
using System.Collections;

namespace Tactile
{
	public class HashtableDifference
	{
		public static bool AnyDifference(Hashtable currentTable, Hashtable newTable)
		{
			bool result = false;
			HashtableDifference.EnumerateDifferences(currentTable, newTable, string.Empty, delegate(string path, bool isDeleted, object value)
			{
				result = true;
				return HashtableDifference.CallbackResult.Break;
			});
			return result;
		}

		public static void CollectDiffPaths(Hashtable currentTable, Hashtable newTable, string rootPath, Hashtable pathsToChange, Hashtable pathsToRemove)
		{
			HashtableDifference.EnumerateDifferences(currentTable, newTable, rootPath, delegate(string pathToObject, bool isDeleted, object value)
			{
				if (isDeleted)
				{
					pathsToRemove[pathToObject] = true;
				}
				else
				{
					pathsToChange[pathToObject] = value;
				}
				return HashtableDifference.CallbackResult.Continue;
			});
		}

		private static bool EnumerateDifferences(Hashtable currentTable, Hashtable newTable, string rootPath, Func<string, bool, object, HashtableDifference.CallbackResult> callback)
		{
			Hashtable hashtable = currentTable.Clone() as Hashtable;
			IDictionaryEnumerator enumerator = newTable.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
					hashtable.Remove(dictionaryEntry.Key);
					string text = rootPath + dictionaryEntry.Key;
					if (!currentTable.ContainsKey(dictionaryEntry.Key))
					{
						if (callback(text, false, dictionaryEntry.Value) == HashtableDifference.CallbackResult.Break)
						{
							return false;
						}
					}
					else
					{
						object obj2 = currentTable[dictionaryEntry.Key];
						if (dictionaryEntry.Value is Hashtable)
						{
							Hashtable hashtable2 = obj2 as Hashtable;
							if (hashtable2 == null)
							{
								if (callback(text, false, dictionaryEntry.Value) == HashtableDifference.CallbackResult.Break)
								{
									return false;
								}
							}
							else if (!HashtableDifference.EnumerateDifferences(hashtable2, dictionaryEntry.Value as Hashtable, text + ".", callback))
							{
								return false;
							}
						}
						else if (dictionaryEntry.Value is ArrayList)
						{
							if (!HashtableDifference.IsArrayListsEqual(dictionaryEntry.Value as ArrayList, obj2 as ArrayList) && callback(text, false, dictionaryEntry.Value) == HashtableDifference.CallbackResult.Break)
							{
								return false;
							}
						}
						else if (!HashtableDifference.IsValuesEqual(dictionaryEntry.Value, obj2) && callback(text, false, dictionaryEntry.Value) == HashtableDifference.CallbackResult.Break)
						{
							return false;
						}
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			IDictionaryEnumerator enumerator2 = hashtable.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					object obj3 = enumerator2.Current;
					if (callback(rootPath + ((DictionaryEntry)obj3).Key, true, true) == HashtableDifference.CallbackResult.Break)
					{
						return false;
					}
				}
			}
			finally
			{
				IDisposable disposable2;
				if ((disposable2 = (enumerator2 as IDisposable)) != null)
				{
					disposable2.Dispose();
				}
			}
			return true;
		}

		private static bool IsValuesEqual(object va, object vb)
		{
			return (va == null || vb != null) && (va != null || vb == null) && ((va == null && vb == null) || va.ToString() == vb.ToString());
		}

		private static bool IsArrayListsEqual(ArrayList la, ArrayList lb)
		{
			if (la.Count != lb.Count)
			{
				return false;
			}
			for (int i = 0; i < la.Count; i++)
			{
				if (!object.Equals(la[i], lb[i]))
				{
					return false;
				}
			}
			return true;
		}

		public enum CallbackResult
		{
			Continue,
			Break
		}
	}
}
