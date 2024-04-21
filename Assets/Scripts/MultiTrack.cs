using System;
using System.Collections.Generic;
using UnityEngine;

public class MultiTrack : ScriptableObject
{
	public string id = string.Empty;

	public List<MultiTrack.Voice> voices = new List<MultiTrack.Voice>();

	public AudioClip backgroundAudio;

	public float mouthThreshold = 0.1f;

	public float delayBeforeTitle;

	[Serializable]
	public class Voice
	{
		public AudioClip clip;

		public float audioStartTime;
	}
}
