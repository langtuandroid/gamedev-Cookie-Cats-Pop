using System;
using System.Collections;
using System.Diagnostics;
using Prime31;

public class ActivityManager : AbstractManager
{
	static ActivityManager()
	{
		AbstractManager.initialize(typeof(ActivityManager));
	}

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action onResumeEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string> onOpenUrlEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action onResumeNeverIgnoredEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action onNewIntentEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<bool> onWindowFocusChangedEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<bool> onRequestPermissionsResultEvent;

	public static void PushIgnoreNextOnResumeEventIfNoNewIntent()
	{
		ActivityManager.ignoreNextOnResumeEventIfNoNewIntent++;
	}

	public static void PopIgnoreNextOnResumeEventIfNoNewIntent()
	{
		FiberCtrl.Pool.Run(ActivityManager.ResetIgnoreNextOnResumeEventIfNoNewIntentFrameDelayed(), false);
	}

	public static bool IgnoreNextOnResumeEventIfNoNewIntent
	{
		get
		{
			return ActivityManager.ignoreNextOnResumeEventIfNoNewIntent > 0;
		}
	}

	private static IEnumerator ResetIgnoreNextOnResumeEventIfNoNewIntentFrameDelayed()
	{
		yield return null;
		ActivityManager.ignoreNextOnResumeEventIfNoNewIntent = Math.Max(0, ActivityManager.ignoreNextOnResumeEventIfNoNewIntent - 1);
		yield break;
	}

	public void onResume()
	{
		if (!ActivityManager.IgnoreNextOnResumeEventIfNoNewIntent && ActivityManager.onResumeEvent != null)
		{
			ActivityManager.onResumeEvent();
		}
		if (ActivityManager.onResumeNeverIgnoredEvent != null)
		{
			ActivityManager.onResumeNeverIgnoredEvent();
		}
		ActivityManager.ignoreNextOnResumeEventIfNoNewIntent = 0;
	}

	public void onOpenUrl(string url)
	{
		if (ActivityManager.onOpenUrlEvent != null)
		{
			ActivityManager.onOpenUrlEvent(url);
		}
	}

	public void onNewIntent(string bundleString)
	{
		if (ActivityManager.onNewIntentEvent != null)
		{
			ActivityManager.onNewIntentEvent();
		}
		ActivityManager.ignoreNextOnResumeEventIfNoNewIntent = 0;
	}

	public void onWindowFocusChanged(string value)
	{
		if (ActivityManager.onWindowFocusChangedEvent != null)
		{
			ActivityManager.onWindowFocusChangedEvent(value == "1");
		}
	}

	public void onRequestPermissionsResult(string granted)
	{
		if (ActivityManager.onRequestPermissionsResultEvent != null)
		{
			ActivityManager.onRequestPermissionsResultEvent(bool.Parse(granted));
		}
	}

	private static int ignoreNextOnResumeEventIfNoNewIntent;
}
