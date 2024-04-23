using System;
using UnityEngine;

namespace TactileModules.GameCore.Audio
{
	[Serializable]
	public class MusicTrackData
	{
		public MusicTrackData(bool loop, bool exitOnDone, float fadeInDuration = 0.3f, float fadeOutDuration = 0.3f)
		{
			this.loop = loop;
			this.exitOnDone = exitOnDone;
			this.fadeInDuration = fadeInDuration;
			this.fadeOutDuration = fadeOutDuration;
		}

		public bool Loop
		{
			get
			{
				return this.loop;
			}
		}

		public bool ExitOnDone
		{
			get
			{
				return this.exitOnDone;
			}
		}

		public float FadeInDuration
		{
			get
			{
				return this.fadeInDuration;
			}
		}

		public float FadeOutDuration
		{
			get
			{
				return this.fadeOutDuration;
			}
		}

		public bool DestroyImmediate
		{
			get
			{
				return this.destroyImmediate;
			}
		}

		[SerializeField]
		private bool loop;

		[SerializeField]
		private bool exitOnDone;

		[SerializeField]
		private float fadeInDuration = 0.3f;

		[SerializeField]
		private float fadeOutDuration = 0.3f;

		[SerializeField]
		private bool destroyImmediate;
	}
}
