using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tactile
{
	public abstract class ManagerWithMetaData<E, M> where M : ScriptableObject
	{
		protected ManagerWithMetaData()
		{
			this.CacheAllMetaData();
		}

		protected abstract string MetaDataAssetFolder { get; }

		protected void CacheAllMetaData()
		{
			int num = this.MetaDataAssetFolder.IndexOf("Resources/");
			string path = this.MetaDataAssetFolder.Substring(num + 10);
			M[] array = Resources.LoadAll<M>(path);
			foreach (M value in array)
			{
				this.cachedMetaDataPerItem[value.name] = value;
			}
		}

		public int MetaDataCount
		{
			get
			{
				return this.cachedMetaDataPerItem.Count;
			}
		}

		public M GetMetaData(E item)
		{
			return this.GetMetaData<M>(item);
		}

		public T GetMetaData<T>(E item) where T : M
		{
			M m;
			if (!this.cachedMetaDataPerItem.TryGetValue(item.ToString(), out m))
			{
				return (T)((object)null);
			}
			if (m is T)
			{
				return (T)((object)m);
			}
			return (T)((object)null);
		}

		public IEnumerable<M> GetAllMetaData()
		{
			return this.GetAllMetaDataOf<M>();
		}

		public IEnumerable<T> GetAllMetaDataOf<T>() where T : M
		{
			foreach (KeyValuePair<string, M> pair in this.cachedMetaDataPerItem)
			{
				if (pair.Value is T)
				{
					yield return pair.Value as T;
				}
			}
			yield break;
		}

		private readonly Dictionary<string, M> cachedMetaDataPerItem = new Dictionary<string, M>();
	}
}
