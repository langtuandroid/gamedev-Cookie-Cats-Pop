using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Cloud;
using UnityEngine;

namespace TactileModules.CrossPromotion.Cloud.CloudInterface
{
	public class AdServerCloudInterface : IAdServerCloudInterface
	{
		public IEnumerator CrossPromotionAdConfig(Response result)
		{
			Hashtable hashtable = new Hashtable();
			return this.StartRequest("/api/ad/config-v3", hashtable, result);
		}

		public IEnumerator CrossPromotionAdInterstitial(string[] installedGames, int userProgress, Response result)
		{
			Hashtable hashtable = new Hashtable
			{
				{
					"installedApps",
					installedGames
				},
				{
					"userProgress",
					userProgress
				},
				{
					"screenDpi",
					Screen.dpi
				},
				{
					"screenWidth",
					Screen.width
				},
				{
					"screenHeight",
					Screen.height
				}
			};
			return this.StartRequest("/api/ad/interstitial/request-v3", hashtable, result);
		}

		public IEnumerator CrossPromotionAdRewardedVideo(string[] installedGames, int userProgress, Response result)
		{
			Hashtable hashtable = new Hashtable
			{
				{
					"installedApps",
					installedGames
				},
				{
					"userProgress",
					userProgress
				},
				{
					"screenDpi",
					Screen.dpi
				},
				{
					"screenWidth",
					Screen.width
				},
				{
					"screenHeight",
					Screen.height
				}
			};
			return this.StartRequest("/api/ad/rewarded/request-v3", hashtable, result);
		}

		private IEnumerator StartRequest(string endPoint, Hashtable hashtable, Response response)
		{
			hashtable.Add("deviceId", SystemInfoHelper.DeviceID);
			hashtable.Add("packageName", SystemInfoHelper.BundleIdentifier);
			hashtable.Add("versionCode", SystemInfoHelper.BundleVersion);
			hashtable.Add("versionName", SystemInfoHelper.BundleShortVersion);
			hashtable.Add("platform", SystemInfoHelper.DeviceType);
			string jsonText = hashtable.toJson();
			byte[] jsonContentAsBytes = Encoding.UTF8.GetBytes(jsonText);
			byte[] bytesForSignature = Encoding.UTF8.GetBytes(endPoint + jsonText);
			StringBuilder signature = this.ComputeRequestSignature(bytesForSignature);
			Dictionary<string, string> headers = this.ConfigureHTTPHeaders(signature);
			yield return TactileRequest.Run("https://adserver.tactilews.com" + endPoint, jsonContentAsBytes, headers, delegate(WWW request)
			{
				response.FillResponse(request);
			}, TactileRequest.RequestPriority.Default);
			yield break;
		}

		private StringBuilder ComputeRequestSignature(byte[] jsonAsBytes)
		{
			byte[] bytes = Encoding.UTF8.GetBytes("YGXJ6UVwR6XJ6BjUPsdK5X9tFd6n4xSrhZTkDFyYexDGdf7ZjVpg42mVs7MMQ4Hv");
			HMACSHA256 hmacsha = new HMACSHA256(bytes);
			byte[] array = hmacsha.ComputeHash(jsonAsBytes);
			StringBuilder stringBuilder = new StringBuilder(array.Length * 2);
			foreach (byte b in array)
			{
				stringBuilder.Append(b.ToString("x2"));
			}
			return stringBuilder;
		}

		private Dictionary<string, string> ConfigureHTTPHeaders(StringBuilder signature)
		{
			return new Dictionary<string, string>
			{
				{
					"Content-Type",
					"application/json"
				},
				{
					"x-tactile-secret-id",
					"tactile"
				},
				{
					"x-client",
					"true"
				},
				{
					"x-tactile-signature",
					signature.ToString()
				}
			};
		}

		private const string SERVER_URL = "https://adserver.tactilews.com";

		private const string SECRET = "YGXJ6UVwR6XJ6BjUPsdK5X9tFd6n4xSrhZTkDFyYexDGdf7ZjVpg42mVs7MMQ4Hv";

		private const string SECRET_ID = "tactile";
	}
}
