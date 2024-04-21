using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using UnityEngine;

public static class TactileRequest
{
	public static void Configure(Func<bool> pauseRequests, float requestTimeout, int maxRequests)
	{
		TactileRequest.Configure(pauseRequests, requestTimeout, requestTimeout, maxRequests);
	}

	public static void Configure(Func<bool> pauseRequests, float requestTimeout, float interactiveRequestTimeout, int maxRequests)
	{
		TactileRequest.pauseRequests = pauseRequests;
		TactileRequest.requestTimeout = requestTimeout;
		TactileRequest.interactiveRequestTimeout = interactiveRequestTimeout;
		TactileRequest.maxRequests = maxRequests;
		FiberCtrl.Pool.Run(TactileRequest.Update(), false);
	}

	public static IEnumerator Run(string url, Action<WWW> callback, TactileRequest.RequestPriority requestPriority = TactileRequest.RequestPriority.Default)
	{
		yield return TactileRequest.WaitWhilePauseRequests(requestPriority);
		yield return TactileRequest.EnterQueue(requestPriority);
		WWW www = new WWW(url);
		yield return TactileRequest.InternalRun(www, requestPriority, callback);
		yield break;
	}

	public static IEnumerator Run(string url, WWWForm form, Action<WWW> callback, TactileRequest.RequestPriority requestPriority = TactileRequest.RequestPriority.Default)
	{
		yield return TactileRequest.WaitWhilePauseRequests(requestPriority);
		yield return TactileRequest.EnterQueue(requestPriority);
		WWW www = new WWW(url, form);
		yield return TactileRequest.InternalRun(www, requestPriority, callback);
		yield break;
	}

	public static IEnumerator Run(string url, byte[] postData, Action<WWW> callback, TactileRequest.RequestPriority requestPriority = TactileRequest.RequestPriority.Default)
	{
		yield return TactileRequest.WaitWhilePauseRequests(requestPriority);
		yield return TactileRequest.EnterQueue(requestPriority);
		WWW www = new WWW(url, postData);
		yield return TactileRequest.InternalRun(www, requestPriority, callback);
		yield break;
	}

	public static IEnumerator Run(string url, byte[] postData, Dictionary<string, string> headers, Action<WWW> callback, TactileRequest.RequestPriority requestPriority = TactileRequest.RequestPriority.Default)
	{
		yield return TactileRequest.Run(url, postData, headers, requestPriority, callback);
		yield break;
	}

	public static IEnumerator Run(string url, byte[] postData, Dictionary<string, string> headers, TactileRequest.RequestPriority requestPriority, Action<WWW> callback)
	{
		yield return TactileRequest.WaitWhilePauseRequests(requestPriority);
		yield return TactileRequest.EnterQueue(requestPriority);
		WWW www = new WWW(url, postData, headers);
		yield return TactileRequest.InternalRun(www, requestPriority, callback);
		yield break;
	}

	private static IEnumerator InternalRun(WWW www, TactileRequest.RequestPriority requestPriority, Action<WWW> callback)
	{
		yield return new Fiber.OnTerminate(delegate()
		{
			TactileRequest.ExitQueue(www, requestPriority);
		});
		float startTime = Time.realtimeSinceStartup;
		float requestTime = 0f;
		float timeoutForThisRequest = TactileRequest.GetRequestTimeoutForPriority(requestPriority);
		while (!www.isDone && requestTime < timeoutForThisRequest)
		{
			yield return null;
			requestTime = Time.realtimeSinceStartup - startTime;
		}
		if (requestTime >= timeoutForThisRequest)
		{
		}
		yield return TactileRequest.WaitWhilePauseRequests(requestPriority);
		callback((!www.isDone) ? null : www);
		yield return null;
		TactileRequest.ExitQueue(www, requestPriority);
		yield break;
	}

	private static float GetRequestTimeoutForPriority(TactileRequest.RequestPriority requestPriority)
	{
		if (requestPriority == TactileRequest.RequestPriority.Interactive)
		{
			return TactileRequest.interactiveRequestTimeout;
		}
		return TactileRequest.requestTimeout;
	}

	private static IEnumerator WaitWhilePauseRequests(TactileRequest.RequestPriority requestPriority)
	{
		while (TactileRequest.pauseRequests != null && TactileRequest.pauseRequests())
		{
			yield return null;
		}
		yield break;
	}

	private static IEnumerator EnterQueue(TactileRequest.RequestPriority requestPriority)
	{
		if (requestPriority == TactileRequest.RequestPriority.Interactive)
		{
			yield break;
		}
		if (TactileRequest.maxRequests == 0)
		{
		}
		while (TactileRequest.requestsRunning >= TactileRequest.maxRequests)
		{
			yield return null;
		}
		TactileRequest.requestsRunning++;
		yield break;
	}

	private static void ExitQueue(WWW www, TactileRequest.RequestPriority requestPriority)
	{
		TactileRequest.wwwsToDispose.Add(www);
		if (requestPriority != TactileRequest.RequestPriority.Interactive)
		{
			TactileRequest.requestsRunning--;
		}
	}

	private static IEnumerator Update()
	{
		for (;;)
		{
			yield return TactileRequest.WaitWhilePauseRequests(TactileRequest.RequestPriority.Default);
			for (int i = 0; i < TactileRequest.wwwsToDispose.Count; i++)
			{
				if (TactileRequest.wwwsToDispose[i].isDone)
				{
					TactileRequest.wwwsToDispose[i].Dispose();
					TactileRequest.wwwsToDispose.RemoveAt(i);
					break;
				}
			}
			yield return null;
		}
		yield break;
	}

	private static Func<bool> pauseRequests;

	private static float requestTimeout;

	private static float interactiveRequestTimeout;

	private static int maxRequests;

	private static List<WWW> wwwsToDispose = new List<WWW>();

	private static int requestsRunning;

	public enum RequestPriority
	{
		Default,
		Interactive = 100
	}
}
