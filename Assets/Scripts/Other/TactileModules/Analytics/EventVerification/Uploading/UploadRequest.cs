using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TactileModules.Analytics.EventVerification.Uploading
{
	public class UploadRequest : IUploadRequest
	{
		public UploadRequest(string url, Dictionary<string, string> headers, string body)
		{
			this.url = url;
			this.headers = headers;
			this.body = body;
		}

		public string GetErrorMessage()
		{
			return this.request.error;
		}

		public IUploadResponse GetResponse()
		{
			return new UploadResponse(this.request.text);
		}

		public IEnumerator Run()
		{
			this.request = new WWW(this.url, Encoding.UTF8.GetBytes(this.body), this.headers);
			yield return this.request;
			yield break;
		}

		private readonly string url;

		private readonly Dictionary<string, string> headers;

		private readonly string body;

		private WWW request;
	}
}
