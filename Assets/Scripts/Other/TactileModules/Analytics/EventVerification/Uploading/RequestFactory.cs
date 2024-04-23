using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using TactileModules.Analytics.CollectorLoadBalancing;
using TactileModules.Analytics.EventVerification.Packaging;

namespace TactileModules.Analytics.EventVerification.Uploading
{
	public class RequestFactory : IRequestFactory
	{
		public RequestFactory(ICollectorLoadBalancer loadBalancer, ServicePath servicePath)
		{
			this.loadBalancer = loadBalancer;
			this.servicePath = servicePath;
		}

		public IUploadRequest CreateRequest(EventCountPackage package)
		{
			string text = JsonSerializer.ObjectToHashtable(package).toJson();
			string signature = this.ComputeSignature(text);
			Dictionary<string, string> headers = this.CreateHeaders(signature);
			string url = this.loadBalancer.ActiveCollector() + this.servicePath.GetPath();
			return new UploadRequest(url, headers, text);
		}

		public void FailoverToNextCollector()
		{
			this.loadBalancer.FailoverToNextCollector();
		}

		private Dictionary<string, string> CreateHeaders(string signature)
		{
			return new Dictionary<string, string>
			{
				{
					"Content-Type",
					"application/json"
				},
				{
					"x-tactile-analytics-secret-id",
					this.secretId
				},
				{
					"x-tactile-analytics-signature",
					signature
				},
				{
					"x-tactile-analytics-bundle-identifier",
					SystemInfoHelper.BundleIdentifier
				},
				{
					"x-tactile-analytics-app-version-name",
					SystemInfoHelper.BundleShortVersion
				},
				{
					"x-tactile-analytics-app-version-code",
					SystemInfoHelper.BundleVersion
				}
			};
		}

		private string ComputeSignature(string jsonText)
		{
			HMACSHA256 hmacsha = new HMACSHA256(this.secret);
			byte[] array = hmacsha.ComputeHash(Encoding.UTF8.GetBytes(jsonText));
			StringBuilder stringBuilder = new StringBuilder(array.Length * 2);
			foreach (byte b in array)
			{
				stringBuilder.Append(b.ToString("x2"));
			}
			return stringBuilder.ToString();
		}

		private readonly ICollectorLoadBalancer loadBalancer;

		private readonly ServicePath servicePath;

		private string secretId = "tactile";

		private readonly byte[] secret = Encoding.UTF8.GetBytes("ba9051e1-84e8-4ee3-b057-2cc40f7fc981");
	}
}
