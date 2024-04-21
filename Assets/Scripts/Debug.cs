using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Debug
{
	public static Hashtable DebugHashTable
	{
		get
		{
			return global::Debug.debugHashTable;
		}
	}

	[Conditional("ENABLE_DEBUG_LOG")]
	public static void LogWarning(object log)
	{
		global::Debug.StoreLog(log);
		UnityEngine.Debug.LogWarning(log);
	}

	[Conditional("ENABLE_DEBUG_LOG")]
	public static void LogWarning(object log, UnityEngine.Object context)
	{
		global::Debug.StoreLog(log);
		UnityEngine.Debug.LogWarning(log, context);
	}

	[Conditional("ENABLE_DEBUG_LOG")]
	public static void LogWarningFormat(string format, params object[] args)
	{
		global::Debug.StoreLog(string.Format(format, args));
		UnityEngine.Debug.LogWarningFormat(format, args);
	}

	[Conditional("ENABLE_DEBUG_LOG")]
	public static void LogWarningFormat(UnityEngine.Object context, string format, params object[] args)
	{
		global::Debug.StoreLog(string.Format(format, args));
		UnityEngine.Debug.LogWarningFormat(context, format, args);
	}

	[Conditional("ENABLE_DEBUG_LOG")]
	public static void Log(object log)
	{
		global::Debug.StoreLog(log);
		UnityEngine.Debug.Log(log);
	}

	[Conditional("ENABLE_DEBUG_LOG")]
	public static void Log(object log, UnityEngine.Object context)
	{
		global::Debug.StoreLog(log);
		UnityEngine.Debug.Log(log, context);
	}

	[Conditional("ENABLE_DEBUG_LOG")]
	public static void LogFormat(string format, params object[] args)
	{
		global::Debug.StoreLog(string.Format(format, args));
		UnityEngine.Debug.LogFormat(format, args);
	}

	[Conditional("ENABLE_DEBUG_LOG")]
	public static void LogFormat(UnityEngine.Object context, string format, params object[] args)
	{
		global::Debug.StoreLog(string.Format(format, args));
		UnityEngine.Debug.LogFormat(context, format, args);
	}

	[Conditional("ENABLE_DEBUG_LOG")]
	public static void LogError(object log)
	{
		global::Debug.StoreLog(log);
		UnityEngine.Debug.LogError(log);
	}

	[Conditional("ENABLE_DEBUG_LOG")]
	public static void LogError(object log, UnityEngine.Object context)
	{
		global::Debug.StoreLog(log);
		UnityEngine.Debug.LogError(log, context);
	}

	[Conditional("ENABLE_DEBUG_LOG")]
	public static void LogErrorFormat(string format, params object[] args)
	{
		global::Debug.StoreLog(string.Format(format, args));
		UnityEngine.Debug.LogErrorFormat(format, args);
	}

	[Conditional("ENABLE_DEBUG_LOG")]
	public static void LogErrorFormat(UnityEngine.Object context, string format, params object[] args)
	{
		global::Debug.StoreLog(string.Format(format, args));
		UnityEngine.Debug.LogErrorFormat(context, format, args);
	}

	[Conditional("ENABLE_DEBUG_LOG")]
	public static void LogAssertion(object log)
	{
		global::Debug.StoreLog(log);
	}

	[Conditional("ENABLE_DEBUG_LOG")]
	public static void LogAssertion(object log, UnityEngine.Object context)
	{
		global::Debug.StoreLog(log);
	}

	[Conditional("ENABLE_DEBUG_LOG")]
	public static void LogAssertionFormat(string format, params object[] args)
	{
		global::Debug.StoreLog(string.Format(format, args));
	}

	[Conditional("ENABLE_DEBUG_LOG")]
	public static void LogAssertionFormat(UnityEngine.Object context, string format, params object[] args)
	{
		global::Debug.StoreLog(string.Format(format, args));
	}

	[Conditional("ENABLE_DEBUG_LOG")]
	public static void LogException(Exception e)
	{
		global::Debug.StoreLog(e);
		UnityEngine.Debug.LogException(e);
	}

	public static void ClearDeveloperConsole()
	{
		UnityEngine.Debug.ClearDeveloperConsole();
	}

	public static void Break()
	{
		UnityEngine.Debug.Break();
	}

	public static void DrawLine(Vector3 start, Vector3 end)
	{
		UnityEngine.Debug.DrawLine(start, end);
	}

	public static void DrawLine(Vector3 start, Vector3 end, Color color)
	{
		UnityEngine.Debug.DrawLine(start, end, color);
	}

	public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
	{
		UnityEngine.Debug.DrawLine(start, end, color, duration);
	}

	public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration, bool depthTest)
	{
		UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);
	}

	public static void DrawRay(Vector3 start, Vector3 dir)
	{
		UnityEngine.Debug.DrawRay(start, dir);
	}

	public static bool isDebugBuild
	{
		get
		{
			return UnityEngine.Debug.isDebugBuild;
		}
	}

	private static void StoreLog(object log)
	{
		if (global::Debug.debugHashTable == null)
		{
			global::Debug.debugHashTable = new Hashtable();
		}
		global::Debug.logQueue.Enqueue(log.ToString());
		if (global::Debug.logQueue.Count > 25)
		{
			global::Debug.logQueue.Dequeue();
		}
		ArrayList arrayList = new ArrayList();
		global::Debug.debugHashTable["logs"] = arrayList;
		foreach (string value in global::Debug.logQueue)
		{
			arrayList.Add(value);
		}
	}

	private const int LOG_QUEUE_SIZE = 25;

	private static readonly Queue<string> logQueue = new Queue<string>();

	private static Hashtable debugHashTable;
}
