using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TactileModules.UserSupport.WebViews
{
	public class WebViewIntegrationTest : MonoBehaviour
	{
		private void Start()
		{
			this.webView = new WebView();
			this.webView.JavascriptCallback += this.WebViewOnJavascriptCallback;
			this.webView.SetLocalResources(new WebViewLocalResources
			{
				HTMLFiles = new List<string>
				{
					"test.html"
				},
				JavaScriptFiles = new List<string>
				{
					"test.js"
				}
			});
			Coroutine coroutine = base.StartCoroutine(this.Run());
		}

		private void WebViewOnJavascriptCallback(string message)
		{
		}

		private IEnumerator Run()
		{
			yield return base.StartCoroutine(this.webView.Show());
			yield return new WaitForSeconds(3f);
			yield return new WaitForSeconds(5f);
			this.webView.EvaluateJS("\n                    SetParam('name','Martin Traberg');\n                    SetParam('email','martint@tactile.dk');\n\t\t\t\t");
			yield break;
		}

		private WebView webView;
	}
}
