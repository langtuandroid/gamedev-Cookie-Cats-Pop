using System;
using System.Collections;
using System.Collections.Generic;
using TactileModules.TactileCloud.AssetBundles;

namespace Cloud
{
	public class AssetBundleResponse : Response
	{
		public int Version
		{
			get
			{
				return (int)((double)base.data["version"]);
			}
		}

		public Dictionary<string, AssetBundleInfo> AssetBundles
		{
			get
			{
				Hashtable hashtable = base.data["assetBundles"] as Hashtable;
				Dictionary<string, AssetBundleInfo> dictionary = new Dictionary<string, AssetBundleInfo>();
				IDictionaryEnumerator enumerator = hashtable.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
						dictionary.Add((string)dictionaryEntry.Key, JsonSerializer.HashtableToObject<AssetBundleInfo>((Hashtable)dictionaryEntry.Value));
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
				return dictionary;
			}
		}
	}
}
