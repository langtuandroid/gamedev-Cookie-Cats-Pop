using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Fibers;
using TactileModules.Analytics.EventStorage;
using UnityEngine;

namespace TactileModules.Analytics
{
	internal class EventsUploader
	{
		public EventsUploader(string environment, string appId, string userId, List<string> collectUrls)
		{
			this.environment = environment;
			this.appId = appId;
			this.CreateEventsStore();
			this.UpdateCollectUrls(userId, collectUrls);
		}

		public int MaxEventListLength
		{
			get
			{
				return (this.maxEventListLength > 0) ? this.maxEventListLength : 50;
			}
			set
			{
				this.maxEventListLength = value;
			}
		}

		private void CreateEventsStore()
		{
			this.persistedEvents = new EventsStore("TactileAnalytics/" + this.environment + "/PersistedEvents", 1000);
		}

		private string CollectUrl
		{
			get
			{
				return this.collectUrls[this.collectUrlIdx] + this.appId;
			}
		}

		internal EventsStore PersistedEvents
		{
			get
			{
				return this.persistedEvents;
			}
		}

		public void UpdateCollectUrls(string userId, List<string> collectUrls)
		{
			this.collectUrls = collectUrls;
			this.ResetCollectUrl(userId);
		}

		public void ResetCollectUrl(string userId)
		{
			this.collectUrlIdx = Math.Abs(userId.GetHashCode()) % this.collectUrls.Count;
		}

		public void StoreEvent(Hashtable eventHt)
		{
			string eventData = MiniJSON.jsonEncode(eventHt, false, 0);
			try
			{
				this.persistedEvents.AddEvent(eventData);
			}
			catch (Exception e)
			{
				if (!EventsStore.IsStorageException(e))
				{
					throw;
				}
			}
		}

		public IEnumerator DoStoreEvent(Hashtable eventHt)
		{
			string eventData = MiniJSON.jsonEncode(eventHt, false, 0);
			int tries = 0;
			while (tries < 10)
			{
				tries++;
				try
				{
					this.persistedEvents.AddEvent(eventData);
					yield break;
				}
				catch (Exception e)
				{
					if (!EventsStore.IsStorageException(e))
					{
						throw;
					}
					if (tries >= 10)
					{
						throw;
					}
				}
				while (this.uploadEventsInProgress)
				{
					yield return null;
				}
				this.CreateEventsStore();
				yield return null;
			}
			yield break;
		}

		public IEnumerator UploadEvents()
		{
			if (this.uploadEventsInProgress)
			{
				yield break;
			}
			this.uploadEventsInProgress = true;
			yield return new Fiber.OnExit(delegate()
			{
				this.uploadEventsInProgress = false;
			});
			Fiber fiber = new Fiber(this.UploadEventsInternal(), FiberBucket.Manual);
			for (;;)
			{
				try
				{
					if (!fiber.Step())
					{
						yield break;
					}
				}
				catch (Exception e)
				{
					if (!EventsStore.IsStorageException(e))
					{
						throw;
					}
					break;
				}
				yield return null;
			}
			yield break;
		}

		private IEnumerator UploadEventsInternal()
		{
			EventsReadResult readResult = new EventsReadResult();
			yield return this.GetEventsToUpload(readResult);
			string eventsToUpload = readResult.Data;
			while (!string.IsNullOrEmpty(eventsToUpload))
			{
				string jsonText = "{\"eventList\":" + EventsStore.ConvertToJsonList(eventsToUpload) + "}";
				byte[] jsonAsBytes = Encoding.UTF8.GetBytes(jsonText);
				StringBuilder signature = this.ComputeRequestSignature(jsonAsBytes);
				Dictionary<string, string> httpHeaders = this.ConfigureHTTPHeaders(signature);
				WWW request = new WWW(this.CollectUrl, jsonAsBytes, httpHeaders);
				yield return request;
				Hashtable response;
				if (request.error != null)
				{
					response = new Hashtable
					{
						{
							"rc",
							-10000.0
						}
					};
				}
				else
				{
					response = (MiniJSON.jsonDecode(request.text) as Hashtable);
					if (response == null)
					{
						response = new Hashtable
						{
							{
								"rc",
								-20000.0
							}
						};
					}
				}
				ReturnCode returnCode = (ReturnCode)((double)response["rc"]);
				if (returnCode == ReturnCode.NoError)
				{
					this.persistedEvents.RemovePool(readResult.Pool);
				}
				if (returnCode <= ReturnCode.RetryLater)
				{
					this.collectUrlIdx = (this.collectUrlIdx + 1) % this.collectUrls.Count;
					this.persistedEvents.UnlockPool(readResult.Pool);
					break;
				}
				readResult = new EventsReadResult();
				yield return this.GetEventsToUpload(readResult);
				eventsToUpload = readResult.Data;
			}
			yield break;
		}

		private IEnumerator GetEventsToUpload(EventsReadResult result)
		{
			EventsPool eventsPool = this.persistedEvents.LockNextReadableEventsPool();
			result.Data = string.Empty;
			if (eventsPool == null || eventsPool.IsEmpty())
			{
				yield break;
			}
			result.Pool = eventsPool;
			yield return eventsPool.ReadTextAsync(result);
			yield break;
		}

		private StringBuilder ComputeRequestSignature(byte[] jsonAsBytes)
		{
			HMACSHA256 hmacsha = new HMACSHA256(EventsUploader.tactileAnalyticsSecret);
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
					"x-tactile-analytics-secret-id",
					EventsUploader.tactileAnalyticsSecretId
				},
				{
					"x-tactile-analytics-signature",
					signature.ToString()
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

		private const int MAX_TRIES = 10;

		private static string tactileAnalyticsSecretId = "tactile";

		private static byte[] tactileAnalyticsSecret = Encoding.UTF8.GetBytes("ba9051e1-84e8-4ee3-b057-2cc40f7fc981");

		private string environment;

		private string appId;

		private List<string> collectUrls;

		private int collectUrlIdx;

		private bool uploadEventsInProgress;

		private const int defaultEventListLength = 50;

		private int maxEventListLength;

		private EventsStore persistedEvents;
	}
}
