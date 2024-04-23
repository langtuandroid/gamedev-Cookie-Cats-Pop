using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace TactileModules.UserSupport.WebViews
{
	public class WebView
	{
		public WebView()
		{
			this.Create();
		}

		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<string> Loaded;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<string> JavascriptCallback;



		private void Create()
		{
			this.webViewObject = new GameObject("WebViewObject_" + Guid.NewGuid()).AddComponent<WebViewObject>();
			this.webViewObject.Init(new Action<string>(this.OnJavascriptCallback), false, string.Empty, new Action<string>(this.OnError), new Action<string>(this.OnLoaded), true);
		}

		protected virtual void OnJavascriptCallback(string message)
		{
			this.JavascriptCallback(message);
		}

		protected virtual void OnError(string message)
		{
		}

		protected virtual void OnLoaded(string message)
		{
			this.Loaded(message);
		}

		public void SetLocalResources(WebViewLocalResources resources)
		{
			this.localResources = resources;
		}

		protected IEnumerator CopyFromStreamingAssetsToPersistentPath()
		{
			List<string> localFiles = this.localResources.GetResources();
			foreach (string file in localFiles)
			{
				string src = Path.Combine(Application.streamingAssetsPath, file);
				string dst = Path.Combine(Application.persistentDataPath, file);
				byte[] result = null;
				if (src.Contains("://"))
				{
					WWW www = new WWW(src);
					yield return www;
					result = www.bytes;
				}
				else
				{
					result = File.ReadAllBytes(src);
				}
				File.WriteAllBytes(dst, result);
				if (file.Contains(".html"))
				{
					this.webViewObject.LoadURL("file://" + dst.Replace(" ", "%20"));
					break;
				}
			}
			yield break;
		}

		public IEnumerator Show()
		{
			this.webViewObject.SetVisibility(true);
			yield return this.CopyFromStreamingAssetsToPersistentPath();
			yield break;
		}

		public void Hide()
		{
			this.webViewObject.SetVisibility(false);
		}

		public void Dispose()
		{
			this.webViewObject.SetVisibility(false);
			UnityEngine.Object.Destroy(this.webViewObject.gameObject);
		}

		public void EvaluateJS(string javascript)
		{
			this.webViewObject.EvaluateJS(javascript);
		}

		protected WebViewObject webViewObject;

		protected WebViewLocalResources localResources;
	}
}
