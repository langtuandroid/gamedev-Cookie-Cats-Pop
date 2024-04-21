using System;
using UnityEngine;

public class WebViewObject : MonoBehaviour
{
	private void OnApplicationPause(bool paused)
	{
		if (this.webView == null)
		{
			return;
		}
		if (paused)
		{
			this.webView.Call("SetVisibility", new object[]
			{
				false
			});
		}
		else
		{
			this.mResumedTimestamp = Time.realtimeSinceStartup;
		}
	}

	private void Update()
	{
		if (this.webView == null)
		{
			return;
		}
		if (this.mResumedTimestamp != 0f && Time.realtimeSinceStartup - this.mResumedTimestamp > 0.5f)
		{
			this.mResumedTimestamp = 0f;
			this.webView.Call("SetVisibility", new object[]
			{
				this.mVisibility
			});
		}
	}

	public void SetKeyboardVisible(string pIsVisible)
	{
		this.mIsKeyboardVisible = (pIsVisible == "true");
		if (this.mIsKeyboardVisible != this.mIsKeyboardVisible0)
		{
			this.mIsKeyboardVisible0 = this.mIsKeyboardVisible;
			this.SetMargins(this.mMarginLeft, this.mMarginTop, this.mMarginRight, this.mMarginBottom);
		}
	}

	public int AdjustBottomMargin(int bottom)
	{
		if (!this.mIsKeyboardVisible)
		{
			return bottom;
		}
		int num = 0;
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity").Get<AndroidJavaObject>("mUnityPlayer").Call<AndroidJavaObject>("getView", new object[0]);
			using (AndroidJavaObject androidJavaObject2 = new AndroidJavaObject("android.graphics.Rect", new object[0]))
			{
				androidJavaObject.Call("getWindowVisibleDisplayFrame", new object[]
				{
					androidJavaObject2
				});
				num = androidJavaObject.Call<int>("getHeight", new object[0]) - androidJavaObject2.Call<int>("height", new object[0]);
			}
		}
		return (bottom <= num) ? num : bottom;
	}

	public bool IsKeyboardVisible
	{
		get
		{
			return this.mIsKeyboardVisible;
		}
	}

	public void Init(Action<string> cb = null, bool transparent = false, string ua = "", Action<string> err = null, Action<string> ld = null, bool enableWKWebView = false)
	{
		this.onJS = cb;
		this.onError = err;
		this.onLoaded = ld;
		this.webView = new AndroidJavaObject("net.gree.unitywebview.CWebViewPlugin", new object[0]);
		this.webView.Call("Init", new object[]
		{
			base.name,
			transparent,
			ua
		});
	}

	protected virtual void OnDestroy()
	{
		if (this.webView == null)
		{
			return;
		}
		this.webView.Call("Destroy", new object[0]);
		this.webView = null;
	}

	public void SetCenterPositionWithScale(Vector2 center, Vector2 scale)
	{
	}

	public void SetMargins(int left, int top, int right, int bottom)
	{
		if (this.webView == null)
		{
			return;
		}
		this.mMarginLeft = left;
		this.mMarginTop = top;
		this.mMarginRight = right;
		this.mMarginBottom = bottom;
		this.webView.Call("SetMargins", new object[]
		{
			left,
			top,
			right,
			this.AdjustBottomMargin(bottom)
		});
	}

	public void SetVisibility(bool v)
	{
		if (this.webView == null)
		{
			return;
		}
		this.mVisibility = v;
		this.webView.Call("SetVisibility", new object[]
		{
			v
		});
		this.visibility = v;
	}

	public bool GetVisibility()
	{
		return this.visibility;
	}

	public void LoadURL(string url)
	{
		if (string.IsNullOrEmpty(url))
		{
			return;
		}
		if (this.webView == null)
		{
			return;
		}
		this.webView.Call("LoadURL", new object[]
		{
			url
		});
	}

	public void LoadHTML(string html, string baseUrl)
	{
		if (string.IsNullOrEmpty(html))
		{
			return;
		}
		if (string.IsNullOrEmpty(baseUrl))
		{
			baseUrl = string.Empty;
		}
		if (this.webView == null)
		{
			return;
		}
		this.webView.Call("LoadHTML", new object[]
		{
			html,
			baseUrl
		});
	}

	public void EvaluateJS(string js)
	{
		if (this.webView == null)
		{
			return;
		}
		this.webView.Call("EvaluateJS", new object[]
		{
			js
		});
	}

	public int Progress()
	{
		if (this.webView == null)
		{
			return 0;
		}
		return this.webView.Get<int>("progress");
	}

	public bool CanGoBack()
	{
		return this.webView != null && this.webView.Get<bool>("canGoBack");
	}

	public bool CanGoForward()
	{
		return this.webView != null && this.webView.Get<bool>("canGoForward");
	}

	public void GoBack()
	{
		if (this.webView == null)
		{
			return;
		}
		this.webView.Call("GoBack", new object[0]);
	}

	public void GoForward()
	{
		if (this.webView == null)
		{
			return;
		}
		this.webView.Call("GoForward", new object[0]);
	}

	public void CallOnError(string error)
	{
		if (this.onError != null)
		{
			this.onError(error);
		}
	}

	public void CallOnLoaded(string url)
	{
		if (this.onLoaded != null)
		{
			this.onLoaded(url);
		}
	}

	public void CallFromJS(string message)
	{
		if (this.onJS != null)
		{
			this.onJS(message);
		}
	}

	public void AddCustomHeader(string headerKey, string headerValue)
	{
		if (this.webView == null)
		{
			return;
		}
		this.webView.Call("AddCustomHeader", new object[]
		{
			headerKey,
			headerValue
		});
	}

	public string GetCustomHeaderValue(string headerKey)
	{
		if (this.webView == null)
		{
			return null;
		}
		return this.webView.Call<string>("GetCustomHeaderValue", new object[]
		{
			headerKey
		});
	}

	public void RemoveCustomHeader(string headerKey)
	{
		if (this.webView == null)
		{
			return;
		}
		this.webView.Call("RemoveCustomHeader", new object[]
		{
			headerKey
		});
	}

	public void ClearCustomHeader()
	{
		if (this.webView == null)
		{
			return;
		}
		this.webView.Call("ClearCustomHeader", new object[0]);
	}

	public void ClearCookies()
	{
		if (this.webView == null)
		{
			return;
		}
		this.webView.Call("ClearCookies", new object[0]);
	}

	private Action<string> onJS;

	private Action<string> onError;

	private Action<string> onLoaded;

	private bool visibility;

	private int mMarginLeft;

	private int mMarginTop;

	private int mMarginRight;

	private int mMarginBottom;

	private AndroidJavaObject webView;

	private bool mVisibility;

	private bool mIsKeyboardVisible0;

	private bool mIsKeyboardVisible;

	private float mResumedTimestamp;
}
