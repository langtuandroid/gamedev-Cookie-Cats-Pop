using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Fibers;
using TactileModules.RuntimeTools;
using TactileModules.UrlCaching.Analytics;
using TactileModules.UrlCaching.Support;
using UnityEngine;

namespace TactileModules.UrlCaching.Caching
{
	public class UrlCacher : IUrlCacher
	{
		public UrlCacher(IAnalyticsReporter analyticsReporter, IFileSystem fileSystem, IWWWFactory wwwFactory, string domain)
		{
			string persistentDataPath = Application.persistentDataPath;
			if (domain.Contains(persistentDataPath))
			{
				throw new Exception("Please dont add Application.persistentDataPath in the UrlCacher domain : " + domain);
			}
			this.cachePath = Path.Combine(Application.persistentDataPath, domain);
			this.analyticsReporter = analyticsReporter;
			this.fileSystem = fileSystem;
			this.wwwFactory = wwwFactory;
			this.domain = domain;
		}

		public IEnumerator Cache(string url, EnumeratorResult<bool> success)
		{
			if (this.IsCached(url))
			{
				success.value = true;
				yield break;
			}
			yield return this.DownloadAndCacheAsset(url, success);
			yield break;
		}

		public string GetCachePath(string url)
		{
			string str = Md5Utilities.CreateMd5(url);
			string fileExtension = this.GetFileExtension(url);
			string result = Path.Combine(this.cachePath, str + fileExtension);
			if (!this.fileSystem.DirectoryExists(this.cachePath))
			{
				this.fileSystem.CreateDirectory(this.cachePath);
			}
			return result;
		}

		private string GetFileExtension(string url)
		{
			return UrlInfo.GetFileExtension(url);
		}

		public bool IsCached(string url)
		{
			return this.fileSystem.FileExists(this.GetCachePath(url));
		}

		public void Delete(string path)
		{
			if (this.fileSystem.FileExists(path))
			{
				this.fileSystem.DeleteFile(path);
			}
		}

		public List<string> GetAllCached()
		{
			if (!this.fileSystem.DirectoryExists(this.cachePath))
			{
				return new List<string>();
			}
			return new List<string>(Directory.GetFiles(this.cachePath, "*", SearchOption.TopDirectoryOnly));
		}

		private IEnumerator DownloadAndCacheAsset(string url, EnumeratorResult<bool> success)
		{
			if (!this.IsValidUrl(url))
			{
				yield break;
			}
			this.analyticsReporter.ReportUrlCachingStarted(url);
			IWWW www = this.wwwFactory.CreateWWW(url);
			yield return www.WaitForCompletion();
			if (www.Error != null)
			{
				this.analyticsReporter.ReportUrlCachingError(url, "WWW Request error", www.Error, null);
				yield break;
			}
			if (www.Bytes == null)
			{
				this.analyticsReporter.ReportUrlCachingError(url, "WWW Request error", "File is null", null);
				yield break;
			}
			this.analyticsReporter.ReportUrlCachingCompleted(url, www.Bytes.Length);
			success.value = this.TryCacheFile(url, www.Bytes);
			yield break;
		}

		private bool TryCacheFile(string url, byte[] bytes)
		{
			bool result;
			try
			{
				this.analyticsReporter.ReportUrlCachingWriteAllBytesStarted(url);
				this.fileSystem.WriteAllBytes(this.GetCachePath(url), bytes);
				this.analyticsReporter.ReportUrlCachingWriteAllBytesCompleted(url);
				result = true;
			}
			catch (Exception exception)
			{
				this.analyticsReporter.ReportUrlCachingError(url, "WriteAllBytes error", "Failed to write file to disk", exception);
				result = false;
			}
			return result;
		}

		private bool IsValidUrl(string url)
		{
			if (!UrlInfo.IsValidUrl(url))
			{
				string errorMessage = "URL not using HTTPS!";
				this.analyticsReporter.ReportUrlCachingError(url, "IsValidUrl error", errorMessage, null);
				return false;
			}
			return true;
		}

		public Texture2D LoadTextureFromCache(string url)
		{
			string path = this.GetCachePath(url);
			byte[] data = this.fileSystem.ReadAllBytes(path);
			Texture2D texture2D = new Texture2D(1, 1);
			texture2D.LoadImage(data);
			return texture2D;
		}

		private readonly string cachePath;

		private IAnalyticsReporter analyticsReporter;

		private readonly IFileSystem fileSystem;

		private readonly IWWWFactory wwwFactory;

		private readonly string domain;
	}
}
