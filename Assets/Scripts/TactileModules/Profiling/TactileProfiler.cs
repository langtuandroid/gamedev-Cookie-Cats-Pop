using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace TactileModules.Profiling
{
	public static class TactileProfiler
	{
		public static string OutputPath
		{
			private get
			{
				return TactileProfiler.outputPath;
			}
			set
			{
				TactileProfiler.outputPath = value;
			}
		}

		private static string ConstructProfileFilename()
		{
			DateTime now = DateTime.Now;
			string text = string.Format("profiler_{0}_{1}.log", now.ToShortDateString(), now.ToLongTimeString());
			return text.Replace("/", "-").Replace(":", "-");
		}

		private static void SessionSetup()
		{
			TactileProfiler.profileFilename = TactileProfiler.ConstructProfileFilename();
			TactileProfiler.sessionSetup = true;
			UnityEngine.Object.DontDestroyOnLoad(new GameObject("TactileProfilerLifecycle", new Type[]
			{
				typeof(TactileProfiler.TactileProfilerLifecycle)
			}));
		}

		public static string CurrentProfileFile()
		{
			return TactileProfiler.profileFilename;
		}

		private static void OnApplicationQuit()
		{
			TactileProfiler.Log("On Applicaiton Quit");
		}

		private static void Log(string message)
		{
			UnityEngine.Debug.Log(typeof(TactileProfiler) + " - " + message);
		}

		private static void LogWarning(string message)
		{
			UnityEngine.Debug.LogWarning(typeof(TactileProfiler) + message);
		}

		public static void LogSampleStack()
		{
			TactileProfiler.Log("=========SAMPLE STACK==========");
			foreach (string text in TactileProfiler.sampleStack)
			{
				TactileProfiler.Log(text + " - " + TactileProfiler.stopwatches[text].ElapsedMilliseconds);
			}
			TactileProfiler.Log("===============================");
		}

		private static Stopwatch SetupStopwatch(string name)
		{
			if (!TactileProfiler.stopwatches.ContainsKey(name))
			{
				TactileProfiler.stopwatches[name] = new Stopwatch();
			}
			Stopwatch stopwatch = TactileProfiler.stopwatches[name];
			stopwatch.Reset();
			return stopwatch;
		}

		public static string ConstructProfileData()
		{
			string text = string.Empty;
			foreach (KeyValuePair<string, double> keyValuePair in TactileProfiler.timings)
			{
				text += TactileProfiler.ConstructProfileFileRow(keyValuePair.Key, keyValuePair.Value);
			}
			return text;
		}

		private static string ConstructProfileFileRow(string name, double timing)
		{
			return string.Format("{0} - {1}ms\n", name, timing);
		}

		[Conditional("FALSE")]
		private static void StoreSample(string name, double timing)
		{
			TactileProfiler.timings[name] = timing;
		}

		[Conditional("FALSE")]
		internal static void BeginSample(string name)
		{
			TactileProfiler.BeginSampleInternal(name);
		}

		[Conditional("FALSE")]
		internal static void EndSample()
		{
			TactileProfiler.EndSampleInternal();
		}

		private static void BeginSampleInternal(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				TactileProfiler.LogWarning("Sample name cannot be null or empty");
				return;
			}
			if (TactileProfiler.stopwatches.ContainsKey(name))
			{
				TactileProfiler.LogWarning("Trying to start a new sample when it's already begun");
				return;
			}
			if (!TactileProfiler.sessionSetup)
			{
				TactileProfiler.SessionSetup();
			}
			TactileProfiler.Log("Sample Begin: " + name);
			TactileProfiler.sampleStack.Push(name);
			TactileProfiler.timings[name] = -1.0;
			Stopwatch stopwatch = TactileProfiler.SetupStopwatch(name);
			stopwatch.Start();
		}

		private static void EndSampleInternal()
		{
			if (!TactileProfiler.sessionSetup)
			{
				TactileProfiler.LogWarning("Session not setup for profiler. Call BeginSample to setup.");
				return;
			}
			if (TactileProfiler.sampleStack.Count == 0)
			{
				TactileProfiler.LogWarning("Trying to stop a sample when there are none left");
				return;
			}
			string text = TactileProfiler.sampleStack.Pop();
			Stopwatch stopwatch = TactileProfiler.stopwatches[text];
			stopwatch.Stop();
			double num = (double)stopwatch.ElapsedMilliseconds;
			TactileProfiler.Log(string.Format("Sample Created: {0} {1}ms", text, num));
			TactileProfiler.stopwatches.Remove(text);
		}

		private const string PROFILE_FILENAME_FORMAT = "profiler_{0}_{1}.log";

		public const string DEFAULT_FOLDER = "[Output]/Profiler";

		private static Dictionary<string, Stopwatch> stopwatches = new Dictionary<string, Stopwatch>();

		private static Stack<string> sampleStack = new Stack<string>();

		private static bool sessionSetup;

		private static string profileFilename;

		private static Dictionary<string, double> timings = new Dictionary<string, double>();

		private static string outputPath = "[Output]/Profiler";

		private class TactileProfilerLifecycle : MonoBehaviour
		{
			private void OnApplicationQuit()
			{
				TactileProfiler.OnApplicationQuit();
			}
		}
	}
}
