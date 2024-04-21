using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using Spine;
using Spine.Unity;
using UnityEngine;

public static class SpineExtensions
{
	public static TrackEntry PlayAnimation(this SkeletonAnimation skeletonAnimation, int track, string animation, bool loop, bool forceUpdate = false)
	{
		TrackEntry result = skeletonAnimation.state.SetAnimation(track, animation, loop);
		if (forceUpdate)
		{
			skeletonAnimation.skeleton.SetToSetupPose();
			skeletonAnimation.Update(0f);
			skeletonAnimation.skeleton.UpdateWorldTransform();
		}
		return result;
	}

	public static TrackEntry AddAnimationInQueue(this SkeletonAnimation skeletonAnimation, int track, string animation, bool loop, float delay, bool forceUpdate = false)
	{
		TrackEntry result = skeletonAnimation.state.AddAnimation(track, animation, loop, delay);
		if (forceUpdate)
		{
			skeletonAnimation.skeleton.SetToSetupPose();
			skeletonAnimation.Update(0f);
			skeletonAnimation.skeleton.UpdateWorldTransform();
		}
		return result;
	}

	public static void SetToSetupPose(this SkeletonAnimation skeletonAnimation)
	{
		skeletonAnimation.skeleton.SetToSetupPose();
	}

	public static Dictionary<Spine.Event, float> GetEvents(this SkeletonAnimation skeletonAnimation, string animationName)
	{
		Spine.Animation animation = skeletonAnimation.state.Data.SkeletonData.FindAnimation(animationName);
		Dictionary<Spine.Event, float> dictionary = new Dictionary<Spine.Event, float>();
		for (int i = 0; i < animation.timelines.Count; i++)
		{
			EventTimeline eventTimeline = animation.timelines[i] as EventTimeline;
			if (eventTimeline != null)
			{
				for (int j = 0; j < eventTimeline.Events.Length; j++)
				{
					dictionary.Add(eventTimeline.Events[j], eventTimeline.Frames[j]);
				}
			}
		}
		return dictionary;
	}

	public static IEnumerator PlayUntilEvent(this SkeletonAnimation skeletonAnimation, string anim, string eventName)
	{
		TrackEntry track = skeletonAnimation.PlayAnimation(0, anim, false, true);
		if (track == null)
		{
			yield break;
		}
		bool finished = false;
		Spine.AnimationState.EventDelegate _event = delegate(Spine.AnimationState state, int trackIndex, Spine.Event e)
		{
			if (e.Data.Name == eventName)
			{
				finished = true;
			}
		};
		track.Event += _event;
		yield return new Fiber.OnExit(delegate()
		{
			track.Event -= _event;
		});
		while (!finished)
		{
			yield return null;
		}
		yield break;
	}

	public static IEnumerator PlayTimeline(this SkeletonAnimation skeletonAnimation, string anim, string startEvent, string endEvent, float duration, float repeatCount = 1f, AnimationCurve animationCurve = null)
	{
		float startTime = skeletonAnimation.GetEventTime(anim, startEvent);
		if (startTime < -0.5f)
		{
			yield break;
		}
		float endTime = skeletonAnimation.GetEventTime(anim, endEvent);
		if (endTime < -0.5f)
		{
			yield break;
		}
		yield return skeletonAnimation.PlayTimeline(anim, startTime, endTime, duration, repeatCount, animationCurve);
		yield break;
	}

	public static IEnumerator PlayTimeline(this SkeletonAnimation skeletonAnimation, string anim, float startTime, float endTime, float duration, float repeatCount = 1f, AnimationCurve animationCurve = null)
	{
		TrackEntry track = skeletonAnimation.PlayAnimation(0, anim, true, true);
		if (track == null)
		{
			yield break;
		}
		skeletonAnimation.Update(0f);
		skeletonAnimation.skeleton.UpdateWorldTransform();
		float currentTime = startTime;
		UpdateBonesDelegate update = delegate(ISkeletonAnimation animatedSkeletonComponent)
		{
			track.time = currentTime;
			skeletonAnimation.skeleton.Update(0f);
			skeletonAnimation.state.Update(0f);
		};
		skeletonAnimation.UpdateLocal += update;
		yield return new Fiber.OnExit(delegate()
		{
			skeletonAnimation.UpdateLocal -= update;
		});
		yield return FiberAnimation.Animate(duration, animationCurve, delegate(float t)
		{
			if (repeatCount > 1f)
			{
				t = Mathf.Repeat(t * repeatCount, 1f);
			}
			currentTime = startTime + (endTime - startTime) * t;
		}, false);
		yield break;
	}

	public static float PlayStartingFromEvent(this SkeletonAnimation skeletonAnimation, string anim, string startEvent)
	{
		Dictionary<Spine.Event, float> events = skeletonAnimation.GetEvents(anim);
		float num = -1f;
		foreach (KeyValuePair<Spine.Event, float> keyValuePair in events)
		{
			if (keyValuePair.Key.Data.Name == startEvent)
			{
				num = keyValuePair.Value;
			}
		}
		if (num < -0.5f)
		{
			return 0f;
		}
		TrackEntry trackEntry = skeletonAnimation.PlayAnimation(0, anim, false, true);
		if (trackEntry == null)
		{
			return 0f;
		}
		trackEntry.time = num;
		skeletonAnimation.Update(0f);
		skeletonAnimation.skeleton.UpdateWorldTransform();
		return num;
	}

	public static string GetRandomStartOfLoopEventName(this SkeletonAnimation skeletonAnimation, string animationName)
	{
		string result = string.Empty;
		Dictionary<Spine.Event, float> events = skeletonAnimation.GetEvents(animationName);
		List<string> list = new List<string>();
		foreach (KeyValuePair<Spine.Event, float> keyValuePair in events)
		{
			string name = keyValuePair.Key.Data.Name;
			if (name.ToLower().EndsWith("start"))
			{
				list.Add(name);
			}
		}
		if (list.Count > 0)
		{
			list.Shuffle<string>();
			result = list[0];
		}
		return result;
	}

	public static string GetEndOfLoopEventName(this SkeletonAnimation skeletonAnimation, string animationName, string startOfLoopName, bool getRandomIfMultiple = false)
	{
		string result = string.Empty;
		if (string.IsNullOrEmpty(startOfLoopName))
		{
			return result;
		}
		Dictionary<Spine.Event, float> events = skeletonAnimation.GetEvents(animationName);
		List<string> list = new List<string>();
		foreach (KeyValuePair<Spine.Event, float> keyValuePair in events)
		{
			string name = keyValuePair.Key.Data.Name;
			if (name.ToLower().Contains(startOfLoopName.Replace("start", string.Empty)) && name.ToLower().Contains("end"))
			{
				list.Add(name);
			}
		}
		if (list.Count > 0)
		{
			if (list.Count > 1 && getRandomIfMultiple)
			{
				list.Shuffle<string>();
			}
			result = list[0];
		}
		return result;
	}

	public static IEnumerator PlayLoopBetweenEvents(this SkeletonAnimation skeletonAnimation, string anim, string startEvent, string endEvent, float duration = -1f)
	{
		Dictionary<Spine.Event, float> events = skeletonAnimation.GetEvents(anim);
		float startTime = -1f;
		float endTime = -1f;
		foreach (KeyValuePair<Spine.Event, float> keyValuePair in events)
		{
			if (keyValuePair.Key.Data.Name == startEvent)
			{
				startTime = keyValuePair.Value;
			}
			else if (keyValuePair.Key.Data.Name == endEvent)
			{
				endTime = keyValuePair.Value;
			}
		}
		if (startTime < -0.5f)
		{
			yield break;
		}
		if (endTime < -0.5f)
		{
			yield break;
		}
		TrackEntry track = skeletonAnimation.PlayAnimation(0, anim, true, true);
		if (track == null)
		{
			yield break;
		}
		track.time = startTime;
		skeletonAnimation.Update(0f);
		skeletonAnimation.skeleton.UpdateWorldTransform();
		UpdateBonesDelegate update = delegate(ISkeletonAnimation animatedSkeletonComponent)
		{
			track.time = Mathf.Repeat(track.time - startTime, endTime - startTime) + startTime;
			skeletonAnimation.skeleton.Update(0f);
			skeletonAnimation.state.Update(0f);
		};
		skeletonAnimation.UpdateLocal += update;
		yield return new Fiber.OnExit(delegate()
		{
			skeletonAnimation.UpdateLocal -= update;
		});
		if (duration > 0f)
		{
			while (duration > 0f)
			{
				duration -= Time.deltaTime;
				yield return null;
			}
			yield break;
		}
		for (;;)
		{
			yield return null;
		}
	}

	public static void TrySetSlotColor(this Skeleton skeleton, string slotName, Color c)
	{
		Slot slot = skeleton.FindSlot(slotName);
		if (slot != null)
		{
			slot.SetColor(c);
		}
	}

	public static void TryShowSlot(this Skeleton skeleton, string slotName, bool show)
	{
		skeleton.TrySetSlotColor(slotName, (!show) ? Color.clear : Color.white);
	}

	public static IEnumerator WaitForEvent(this SkeletonAnimation skeletonAnimation, string eventName)
	{
		bool finished = false;
		TrackEntry track = skeletonAnimation.state.GetCurrent(0);
		if (track == null)
		{
			yield break;
		}
		Spine.AnimationState.EventDelegate _event = delegate(Spine.AnimationState state, int trackIndex, Spine.Event e)
		{
			if (e.Data.Name == eventName)
			{
				finished = true;
			}
		};
		track.Event += _event;
		yield return new Fiber.OnExit(delegate()
		{
			track.Event -= _event;
		});
		while (!finished)
		{
			yield return null;
		}
		yield break;
	}

	public static IEnumerator WaitUntilEvent(this SkeletonAnimation skeletonAnimation, string anim, string eventName, Action<float> duration)
	{
		float eventTime = skeletonAnimation.GetEventTime(anim, eventName);
		if (eventTime < -0.5f)
		{
			yield break;
		}
		TrackEntry track = skeletonAnimation.state.GetCurrent(0);
		duration(eventTime - track.time);
		while (track.time < eventTime)
		{
			yield return null;
		}
		yield break;
	}

	public static float GetEventTime(this SkeletonAnimation skeletonAnimation, string anim, string eventName)
	{
		Dictionary<Spine.Event, float> events = skeletonAnimation.GetEvents(anim);
		float result = -1f;
		foreach (KeyValuePair<Spine.Event, float> keyValuePair in events)
		{
			if (keyValuePair.Key.Data.Name == eventName)
			{
				result = keyValuePair.Value;
			}
		}
		return result;
	}

	public static List<float> GetEventTimings(this SkeletonAnimation skeletonAnimation, string anim, string eventName)
	{
		Dictionary<Spine.Event, float> events = skeletonAnimation.GetEvents(anim);
		List<float> list = new List<float>();
		foreach (KeyValuePair<Spine.Event, float> keyValuePair in events)
		{
			if (keyValuePair.Key.Data.Name == eventName)
			{
				list.Add(keyValuePair.Value);
			}
		}
		return list;
	}

	public static bool HasAnimation(this SkeletonAnimation skeletonAnimation, string animationName)
	{
		foreach (Spine.Animation animation in skeletonAnimation.skeleton.data.Animations)
		{
			if (animation.Name == animationName)
			{
				return true;
			}
		}
		return false;
	}
}
