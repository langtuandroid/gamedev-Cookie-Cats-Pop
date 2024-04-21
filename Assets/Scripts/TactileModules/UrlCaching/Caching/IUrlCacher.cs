using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using UnityEngine;

namespace TactileModules.UrlCaching.Caching
{
	public interface IUrlCacher
	{
		IEnumerator Cache(string url, EnumeratorResult<bool> success);

		string GetCachePath(string url);

		bool IsCached(string url);

		void Delete(string path);

		List<string> GetAllCached();

		Texture2D LoadTextureFromCache(string url);
	}
}
