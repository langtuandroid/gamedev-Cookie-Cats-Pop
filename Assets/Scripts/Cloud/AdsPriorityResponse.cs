using System;
using System.Collections;
using System.Collections.Generic;

namespace Cloud
{
	public class AdsPriorityResponse : Response
	{
		public Dictionary<string, Dictionary<string, List<string>>> Priorities
		{
			get
			{
				Hashtable hashtable = base.data["priorities"] as Hashtable;
				Dictionary<string, Dictionary<string, List<string>>> dictionary = new Dictionary<string, Dictionary<string, List<string>>>();
				IDictionaryEnumerator enumerator = hashtable.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
						dictionary.Add((string)dictionaryEntry.Key, new Dictionary<string, List<string>>());
						IDictionaryEnumerator enumerator2 = ((Hashtable)dictionaryEntry.Value).GetEnumerator();
						try
						{
							while (enumerator2.MoveNext())
							{
								object obj2 = enumerator2.Current;
								DictionaryEntry dictionaryEntry2 = (DictionaryEntry)obj2;
								dictionary[(string)dictionaryEntry.Key].Add((string)dictionaryEntry2.Key, new List<string>());
								IEnumerator enumerator3 = ((ArrayList)dictionaryEntry2.Value).GetEnumerator();
								try
								{
									while (enumerator3.MoveNext())
									{
										object obj3 = enumerator3.Current;
										string item = (string)obj3;
										dictionary[(string)dictionaryEntry.Key][(string)dictionaryEntry2.Key].Add(item);
									}
								}
								finally
								{
									IDisposable disposable;
									if ((disposable = (enumerator3 as IDisposable)) != null)
									{
										disposable.Dispose();
									}
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
					}
				}
				finally
				{
					IDisposable disposable3;
					if ((disposable3 = (enumerator as IDisposable)) != null)
					{
						disposable3.Dispose();
					}
				}
				return dictionary;
			}
		}
	}
}
