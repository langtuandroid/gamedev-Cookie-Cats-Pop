using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Fibers;
using UnityEngine;

public class FacebookPortraitManager
{
	public FacebookPortraitManager(FacebookClient fbClient)
	{
		this.fbClient = fbClient;
	}

	public IEnumerator GetPortrait(string facebookId, Action<object, Texture2D> callback)
	{
		yield return new Fiber.OnTerminate(delegate()
		{
			callback = null;
		});
		while (this.requestsInProgess.IndexOf(facebookId) != -1)
		{
			yield return null;
		}
		if (this.cachedFacebookPortraits.ContainsKey(facebookId))
		{
			callback(null, this.cachedFacebookPortraits[facebookId]);
		}
		else
		{
			Texture2D cachedTex = this.LoadFbPortrait(facebookId);
			if (cachedTex != null)
			{
				this.cachedFacebookPortraits.Add(facebookId, cachedTex);
				callback(null, cachedTex);
			}
			else
			{
				bool internalLoadComplete = false;
				FiberCtrl.Pool.Run(this.LoadPortraitInternal(facebookId, delegate(object err, Texture2D tex)
				{
					internalLoadComplete = true;
					if (callback != null)
					{
						callback(err, tex);
					}
				}), false);
				while (!internalLoadComplete)
				{
					yield return null;
				}
			}
		}
		yield break;
	}

	private IEnumerator LoadPortraitInternal(string facebookId, Action<object, Texture2D> callback)
	{
		this.requestsInProgess.Add(facebookId);
		IEnumerator e = this.fbClient.LoadPortraitNoCache(facebookId, delegate(object error, Texture2D tex)
		{
			this.requestsInProgess.Remove(facebookId);
			if (tex != null && facebookId != null && error == null)
			{
				if (this.cachedFacebookPortraits.ContainsKey(facebookId))
				{
					this.cachedFacebookPortraits.Remove(facebookId);
				}
				this.cachedFacebookPortraits.Add(facebookId, tex);
				this.SaveFbPortrait(facebookId, tex);
				callback(null, tex);
			}
			else
			{
				callback("Failed to get portrait for user", null);
			}
		});
		while (e.MoveNext())
		{
			object obj = e.Current;
			yield return obj;
		}
		yield break;
	}

	private void ValidateFbCache()
	{
		int num = 604800;
		DateTime d = new DateTime(2013, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		int num2 = (int)(DateTime.UtcNow - d).TotalSeconds / num;
		int @int = TactilePlayerPrefs.GetInt("portraitsCacheId", 0);
		if (@int != num2)
		{
			this.DeleteFbCachePath();
			TactilePlayerPrefs.SetInt("portraitsCacheId", num2);
		}
		this.EnsureFbCachePath();
	}

	private void DeleteFbCachePath()
	{
		if (Directory.Exists(this.FbCachePath))
		{
			Directory.Delete(this.FbCachePath, true);
		}
	}

	private void EnsureFbCachePath()
	{
		if (!Directory.Exists(this.FbCachePath))
		{
			Directory.CreateDirectory(this.FbCachePath);
		}
	}

	private Texture2D LoadFbPortrait(string fbId)
	{
		this.ValidateFbCache();
		string path = Path.Combine(this.FbCachePath, "p" + fbId);
		if (File.Exists(path))
		{
			Texture2D texture2D = new Texture2D(1, 1, TextureFormat.RGB24, false);
			texture2D.LoadImage(File.ReadAllBytes(path));
			return texture2D;
		}
		return null;
	}

	private void SaveFbPortrait(string fbId, Texture2D tex)
	{
		this.ValidateFbCache();
		string path = Path.Combine(this.FbCachePath, "p" + fbId);
		File.WriteAllBytes(path, tex.EncodeToPNG());
	}

	private string FbCachePath
	{
		get
		{
			return Path.Combine(Application.persistentDataPath, "fb_cache");
		}
	}

	private Dictionary<string, Texture2D> cachedFacebookPortraits = new Dictionary<string, Texture2D>();

	private List<string> requestsInProgess = new List<string>();

	private FacebookClient fbClient;
}
